using System;
using System.Linq;
using ICSharpCode.AvalonEdit.AddIn;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace SharpDevelopRemoteControl.AddIn.Scripting
{
    public class ScriptContextBuilder
    {
        public bool TryBuildContext(AvalonEditViewContent activeView, out ScriptContext context)
        {
            context = null;

            var currentFileName = activeView.PrimaryFileName;
            var caretLocation = activeView.CodeEditor.ActiveTextEditorAdapter.Caret.Position;

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
                return false;
            }

            var currentProject = ProjectService.CurrentProject;
            context = new ScriptContext(
                currentProject,
                currentFileName,
                caretLocation,
                bestMatchingClass,
                mainMethod);

            return true;
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