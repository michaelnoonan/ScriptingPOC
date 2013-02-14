using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.AddIn;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Commands;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using SharpDevelopRemoteControl.Contracts;

namespace SharpDevelopRemoteControl.AddIn.Scripting
{
    public class DebugCurrentScriptCommand : AbstractCommand
    {
        public override void Run()
        {
            // Ensure everything is saved to disk before doing anything else
            ProjectService.SaveSolution();
            SaveAllFiles.SaveAll();
            var activeView = WorkbenchSingleton.Workbench.ActiveViewContent as AvalonEditViewContent;

            if (activeView == null) return;

            ScriptContext context;
            if (new ScriptContextBuilder().TryBuildContext(activeView, out context))
            {
                BuildProject(context);
            }
        }

        private static void BuildProject(ScriptContext context)
        {
            BuildEngine.BuildInGui(context.CurrentProject,
                new BuildOptions(BuildTarget.Rebuild, buildResults => HandleBuildResult(context, buildResults)));
        }

        private static void HandleBuildResult(ScriptContext context, BuildResults buildResults)
        {
            if (buildResults.Result == BuildResultCode.Success)
            {
                AttachDebugger(context);
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
            var assembly = Assembly.Load(File.ReadAllBytes(context.CurrentProject.OutputAssemblyFullPath));
            var assemblyName = assembly.GetName().FullName;

            WorkbenchSingleton.StatusBar.SetMessage("Executing script in " + assemblyName);

            var task = new Task<ScriptExecutionResult>(
                () => HostApplicationAdapter.Instance.ExecuteScriptForDebugging(
                    assemblyName,
                    context.DeclaringClass.FullyQualifiedName,
                    context.Method.Name));
            task.ContinueWith(completedTask => HandleScriptExecutionComplete(completedTask, context));
            task.Start(TaskScheduler.Default);
        }

        private static void HandleScriptExecutionComplete(Task<ScriptExecutionResult> completedTask, ScriptContext context)
        {
            if (DebuggerService.CurrentDebugger.IsAttached)
            {
                WorkbenchSingleton.SafeThreadCall(() => DebuggerService.CurrentDebugger.Detach());
            }

            if (completedTask.IsFaulted && completedTask.Exception != null)
            {
                LogError(context, (completedTask.Exception.InnerExceptions.FirstOrDefault() ??
                                   completedTask.Exception).Message);
                WorkbenchSingleton.StatusBar.SetMessage("The script failed with an exception");
                return;
            }

            var result = completedTask.Result;
            if (result != null)
            {
                if (result.Successful == false)
                {
                    LogError(context, result.FailureReason);
                    WorkbenchSingleton.StatusBar.SetMessage("The script failed after running for " + result.ElapsedTime.ToString());
                }
                else
                {
                    WorkbenchSingleton.StatusBar.SetMessage("The script completed successfully after " + result.ElapsedTime.ToString());
                }
            }
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
    }
}