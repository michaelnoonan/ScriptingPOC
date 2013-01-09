using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using SharpDevelopRemoteControl.Contracts;

namespace SharpDevelopRemoteControl
{
    public class RemoteControl : IRemoteControl
    {
        public void LogMessage(string message)
        {
            LoggingService.InfoFormatted("Message Recieved: {0}", message);
        }

        public void LoadProject(string projectFilePath)
        {
            LoggingService.InfoFormatted("Load project command received: '{0}'", projectFilePath);
            ProjectService.LoadSolutionOrProject(projectFilePath);
        }

        public void ShutDown()
        {
            LoggingService.Info("Shutdown command received...");
            WorkbenchSingleton.MainWindow.Close();
        }
    }
}