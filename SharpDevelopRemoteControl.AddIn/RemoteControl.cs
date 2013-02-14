using System.Linq;
using System.ServiceModel;
using ICSharpCode.AvalonEdit.AddIn;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Commands;
using SharpDevelopRemoteControl.AddIn.Scripting;
using SharpDevelopRemoteControl.Contracts;

namespace SharpDevelopRemoteControl.AddIn
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class RemoteControl : IRemoteControl
    {
        public void LogMessage(string message)
        {
            LoggingService.InfoFormatted("Message Recieved: {0}", message);
        }

        public void LoadScriptingProject(string projectFilePath)
        {
            LoggingService.InfoFormatted("Load project command received: '{0}'", projectFilePath);
            LoggingService.InfoFormatted("Current project is '{0}'", ProjectService.CurrentProject != null ? ProjectService.CurrentProject.FileName : "{null}");

            IProject project;
            if (ProjectService.OpenSolution == null ||
                (ProjectService.CurrentProject != null && ProjectService.CurrentProject.FileName != projectFilePath))
            {
                ProjectService.LoadSolutionOrProject(projectFilePath);
                project = ProjectService.OpenSolution.Projects.Single(p => p.FileName == projectFilePath);
                ProjectService.CurrentProject = project;
            }
            else
            {
                project = ProjectService.CurrentProject;
                LoggingService.Info("The project is already loaded.");
            }

            BuildEngine.BuildInGui(
                project,
                new BuildOptions(BuildTarget.Rebuild,
                                 buildResults => HostApplicationAdapter.Instance.AnnounceScriptingProjectLoaded(
                                     new LoadScriptingProjectResult(projectFilePath, buildResults.Result == BuildResultCode.Success))));
        }

        public void ShutDown()
        {
            LoggingService.Info("Shutdown command received...");
            WorkbenchSingleton.MainWindow.Close();
        }

        public void StartDebuggingScript(string className)
        {
            LoggingService.Info("Debug Script: " + className);
            var classToDebug = ParserService.AllProjectContents.Select(p => p.GetClass(className, 0)).FirstOrDefault(c => c != null);
            LoggingService.Info("Class to debug: " + (classToDebug != null ? classToDebug.FullyQualifiedName : "NOT FOUND!!!!"));
            var methodToDebug = classToDebug.Methods.FirstOrDefault(m => m.Name == "Main");
            NavigationService.NavigateTo(methodToDebug);
            EnsureBreakpointAtCurrentLocation();
            new DebugCurrentScriptCommand().Run();
        }

        private static void EnsureBreakpointAtCurrentLocation()
        {
            var activeView = (AvalonEditViewContent) WorkbenchSingleton.Workbench.ActiveViewContent;
            var filename = activeView.PrimaryFileName;
            var methodPosition = activeView.CodeEditor.ActiveTextEditorAdapter.Caret.Position;
            var matchingBreakpoint =
                BookmarkManager.GetBookmarks(filename)
                               .OfType<BreakpointBookmark>()
                               .FirstOrDefault(bp => bp.LineNumber == methodPosition.Line);
            if (matchingBreakpoint == null)
            {
                new ToggleBreakpointCommand().Run();
            }
        }
    }
}