using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.AddIn;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using SharpDevelopRemoteControl.Contracts;


namespace SharpDevelopRemoteControl.AddIn.Scripting
{
    public class DebugCurrentScriptCommand : AbstractCommand
    {
        public class ScriptContext
        {
            public ScriptContext(
                IProject currentProject,
                FileName currentFileName,
                Location caretLocation,
                IClass declaringClass,
                IMethod method)
            {
                CurrentProject = currentProject;
                CurrentFileName = currentFileName;
                CaretLocation = caretLocation;
                DeclaringClass = declaringClass;
                Method = method;
            }

            public IProject CurrentProject { get; private set; }
            public FileName CurrentFileName { get; private set; }
            public Location CaretLocation { get; private set; }
            public IClass DeclaringClass { get; private set; }
            public IMethod Method { get; private set; }
        }

        public override void Run()
        {
            // TODO: This should be done asynchronously to stop blocking the UI thread!
            var avalonEditViewContent = WorkbenchSingleton.Workbench.ActiveViewContent as AvalonEditViewContent;

            if (avalonEditViewContent != null)
            {
                var currentFileName = avalonEditViewContent.PrimaryFileName;
                var caretLocation = avalonEditViewContent.CodeEditor.ActiveTextEditorAdapter.Caret.Position;

                // Parse the file
                var parseInfo = ParserService.GetExistingParseInformation(currentFileName) ??
                                ParserService.ParseFile(currentFileName);

                var bestMatchingClass = GetBestMatchingClassFromCurrentCaretPosition(parseInfo, caretLocation);
                IMethod mainMethod = null;
                if (bestMatchingClass != null)
                {
                    LoggingService.Info(bestMatchingClass.FullyQualifiedName);
                    mainMethod = bestMatchingClass.Methods.FirstOrDefault(m => m.Name == "Main" && m.Parameters.Count == 0);
                    if (mainMethod != null)
                    {
                        LoggingService.Info(mainMethod.FullyQualifiedName);
                    }
                }

                if (mainMethod == null)
                {
                    TaskService.Add(new ICSharpCode.SharpDevelop.Task(currentFileName,
                         "Which script do you want to run? Please move the text cursor inside a Module or Class with a Subroutine called 'Main' with no parameters.",
                         caretLocation.Column, caretLocation.Line, TaskType.Error));
                    return;
                }

                var currentProject = ProjectService.CurrentProject;
                BuildEngine.BuildInGui(
                    currentProject,
                    new BuildOptions(BuildTarget.Build,
                                     r =>
                                         {
                                             if (r.Result == BuildResultCode.Success)
                                             {
                                                 var context = new ScriptContext(
                                                     currentProject,
                                                     currentFileName,
                                                     caretLocation,
                                                     bestMatchingClass,
                                                     mainMethod);
                                                 LoadScriptIntoHostApplication(context);
                                             }
                                         }));
            }
        }

        private static void LoadScriptIntoHostApplication(ScriptContext context)
        {
            WorkbenchSingleton.StatusBar.SetMessage("Loading script into host application...");

            var task = new Task<ScriptLoadResult>(
                () => HostApplicationAdapter.Instance.LoadScript(
                    context.CurrentProject.OutputAssemblyFullPath,
                    context.DeclaringClass.FullyQualifiedName,
                    context.Method.Name));

            task.ContinueWith(completedTask => HandleScriptLoaded(completedTask, context));
            task.Start(TaskScheduler.Default);
        }

        private static void HandleScriptLoaded(Task<ScriptLoadResult> completedTask, ScriptContext context)
        {
            if (completedTask.IsFaulted && completedTask.Exception != null)
            {
                LogError(context, (completedTask.Exception.InnerExceptions.FirstOrDefault() ??
                                   completedTask.Exception).Message);
                WorkbenchSingleton.StatusBar.SetMessage("Failed to load script in host application");
                return;
            }

            var result = completedTask.Result;
            if (result != null)
            {
                if (result.Successful == false)
                {
                    LogError(context, result.FailureReason);
                    WorkbenchSingleton.StatusBar.SetMessage("Failed to load script in host application");
                }
                else
                {
                    AttachDebugger(context);
                }
            }
        }

        private static void AttachDebugger(ScriptContext context)
        {
            WorkbenchSingleton.StatusBar.SetMessage("Attaching the debugger...");
            LoggingService.Info("Attaching to process with ID: " + HostApplicationAdapter.Instance.HostApplicationProcessId);
            var process = Process.GetProcessById(HostApplicationAdapter.Instance.HostApplicationProcessId);
            LoggingService.Info("Process Name is: " + process.ProcessName);
            if (process.HasExited)
            {
                LogError(context, "The host application has exited and cannot be debugged.");
                WorkbenchSingleton.StatusBar.SetMessage("Failed to attach the debugger");
                return;
            }

            WorkbenchSingleton.SafeThreadAsyncCall(
                () =>
                    {
                        try
                        {
                            DebuggerService.CurrentDebugger.Attach(process);
                        }
                        catch (Exception e)
                        {
                            LogError(context, "Failed to attach the debugger: {0}", e.Message);
                            WorkbenchSingleton.StatusBar.SetMessage("Failed to attach the debugger");
                            throw;
                        }

                        ExecuteScript(context);
                    });
        }

        private static void ExecuteScript(ScriptContext context)
        {
            WorkbenchSingleton.StatusBar.SetMessage("Executing script");

            var task = new Task<ScriptExecutionResult>(
                () => HostApplicationAdapter.Instance.ExecuteScript(
                    context.CurrentProject.OutputAssemblyFullPath,
                    context.DeclaringClass.FullyQualifiedName,
                    context.Method.Name));
            task.Start(TaskScheduler.Default);
        }

        private static void LogError(
            ScriptContext context,
            string messageFormat,
            params object[] args)
        {
            LoggingService.ErrorFormatted(messageFormat, args);
            TaskService.Add(
                    new ICSharpCode.SharpDevelop.Task(
                        context.CurrentFileName,
                        string.Format(messageFormat, args),
                        context.CaretLocation.Column,
                        context.CaretLocation.Line,
                        TaskType.Error));
        }

        private IClass GetBestMatchingClassFromCurrentCaretPosition(ParseInformation parseInfo, Location caretLocation)
        {
            var availableClasses = parseInfo.CompilationUnit.Classes;

            IClass matchInside = null;
            IClass nearestMatch = null;
            var nearestMatchDistance = int.MaxValue;
            foreach (var availableClass in availableClasses)
            {
                if (availableClass.Region.IsInside(caretLocation.Line, caretLocation.Column))
                {
                    matchInside = availableClass;
                    // when there are multiple matches inside (nested classes), use the last one
                }
                else
                {
                    // Not a perfect match?
                    // Try to first the nearest match. We want the classes combo box to always
                    // have a class selected if possible.
                    int matchDistance = Math.Min(Math.Abs(caretLocation.Line - availableClass.Region.BeginLine),
                                                 Math.Abs(caretLocation.Line - availableClass.Region.EndLine));
                    if (matchDistance < nearestMatchDistance)
                    {
                        nearestMatchDistance = matchDistance;
                        nearestMatch = availableClass;
                    }
                }
            }

            return matchInside ?? nearestMatch;
        }
    }
}