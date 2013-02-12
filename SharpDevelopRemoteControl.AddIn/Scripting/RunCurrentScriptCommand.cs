using System;
using ICSharpCode.AvalonEdit.AddIn;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;

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
                var caretLocation = avalonEditViewContent.CodeEditor.ActiveTextEditorAdapter.Caret.Position;

                // Parse the file
                var parseInfo = ParserService.GetExistingParseInformation(avalonEditViewContent.PrimaryFileName) ??
                                ParserService.ParseFile(avalonEditViewContent.PrimaryFileName);

                var bestMatchingClass = GetBestMatchingClassFromCurrentCaretPosition(parseInfo, caretLocation);

                LoggingService.Info(bestMatchingClass.FullyQualifiedName);
            }
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