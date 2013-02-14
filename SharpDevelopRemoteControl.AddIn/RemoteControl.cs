using System.Linq;
using System.ServiceModel;
using ICSharpCode.Core;
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

            if (ProjectService.OpenSolution == null ||
                (ProjectService.CurrentProject != null && ProjectService.CurrentProject.FileName != projectFilePath))
            {
                ProjectService.LoadSolutionOrProject(projectFilePath);
                var project = ProjectService.OpenSolution.Projects.First();
                ProjectService.CurrentProject = project;
            }
            else
            {
                LoggingService.Info("The project is already loaded.");
            }
        }

        public void ShutDown()
        {
            LoggingService.Info("Shutdown command received...");
            WorkbenchSingleton.MainWindow.Close();
        }
    }
}