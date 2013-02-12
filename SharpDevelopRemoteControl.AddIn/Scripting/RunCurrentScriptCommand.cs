using System;
using System.Linq;
using ICSharpCode.AvalonEdit.AddIn;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace SharpDevelopRemoteControl.AddIn.Scripting
{
    public class RunCurrentScriptCommand : AbstractCommand
    {
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
                    TaskService.Add(new Task(currentFileName,
                         "Which script do you want to run? Please move the text cursor inside a Module or Class with a Subroutine called 'Main' with no parameters.",
                         caretLocation.Column, caretLocation.Line, TaskType.Error));
                    return;
                }

                BuildEngine.BuildInGui(
                    ProjectService.CurrentProject,
                    new BuildOptions(BuildTarget.Build,
                                     r =>
                                         {
                                             if (r.Result == BuildResultCode.Success)
                                             {
                                                 ExecuteScriptInHostApplication(mainMethod);
                                             }
                                         }));
            }
        }

        private static void ExecuteScriptInHostApplication(IMethod mainMethod)
        {
            WorkbenchSingleton.StatusBar.SetMessage(
                "Executing script in host application...");
            var result =
                HostApplicationAdapter.Instance.ExecuteScript(
                    ProjectService.CurrentProject.OutputAssemblyFullPath,
                    mainMethod.DeclaringType.FullyQualifiedName,
                    mainMethod.Name);
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