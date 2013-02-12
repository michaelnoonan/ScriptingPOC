using System;
using Caliburn.Micro;
using MyCoolApp.Events.DevelopmentEnvironment;
using MyCoolApp.Projects;
using MyCoolApp.Scripting;
using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.Development
{
    public class HostApplicationService : IHostApplicationService
    {
        private readonly EventAggregator _globalEventAggregator;

        public HostApplicationService()
        {
            _globalEventAggregator = Program.GlobalEventAggregator;
        }

        public void RemoteControlAvailable(string listenUri)
        {
            _globalEventAggregator.Publish(new DevelopmentEnvironmentConnected(listenUri));
        }

        public void DevelopmentEnvironmentShuttingDown()
        {
            _globalEventAggregator.Publish(new DevelopmentEnvironmentDisconnected());
        }

        public ScriptResult ExecuteScript(string assemblyPath, string scriptMethodPath)
        {
            Logger.Instance.Info("Execute Script in {0} at path {1}", assemblyPath, scriptMethodPath);
            return new ScriptResult(true, "Ready", TimeSpan.FromSeconds(5));
        }
    }
}