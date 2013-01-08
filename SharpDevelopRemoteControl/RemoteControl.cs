using System.Diagnostics;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using SharpDevelopRemoteControl.Contracts;

namespace SharpDevelopRemoteControl
{
    public class RemoteControlService : IRemoteControlService
    {
        public PingResponse Ping(PingRequest request)
        {
            return new PingResponse();
        }

        public void LogMessage(string message)
        {
            LoggingService.InfoFormatted("Message Recieved: {0}", message);
        }

        public void LoadProject(string projectFilePath)
        {
            ProjectService.LoadSolutionOrProject(projectFilePath);
        }
    }
}