using MyCoolApp.Events.DevelopmentEnvironment;
using MyCoolApp.Projects;
using MyCoolApp.Scripting;
using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.Development
{
    public class HostApplicationService : IHostApplicationService
    {
        public void RemoteControlAvailable(string listenUri)
        {
            Program.GlobalEventAggregator.Publish(new DevelopmentEnvironmentConnected(listenUri));
        }

        public void DevelopmentEnvironmentShuttingDown()
        {
            Program.GlobalEventAggregator.Publish(new DevelopmentEnvironmentDisconnected());
        }

        public ScriptResult ExecuteFileAsScript(string scriptFilePath)
        {
            var executor = new ScriptExecutor(ProjectManager.Instance.Project);
            return executor.ExecuteScriptFile(scriptFilePath);
        }
    }
}