using System.Linq;
using System.ServiceModel;
using System.Threading;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
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

        public void DebugScript(string className)
        {
            LoggingService.Info("Debug Script: " + className);
            var classToDebug = ParserService.AllProjectContents.Select(p => p.GetClass(className, 0)).FirstOrDefault(c => c != null);
            LoggingService.Info("Class to debug: " + (classToDebug != null ? classToDebug.FullyQualifiedName : "NOT FOUND!!!!"));
            NavigationService.NavigateTo(classToDebug);
        }
    }
}