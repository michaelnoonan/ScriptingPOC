using System;
using System.Diagnostics;
using System.IO;
using System.ServiceModel;
using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.Development
{
    public class RemoteControlManager : IDisposable
    {
        public static readonly RemoteControlManager Instance = new RemoteControlManager();
        
        private const string SharpDevelopExecutablePath = "SharpDevelop\\bin\\SharpDevelop.exe";

        private readonly ChannelFactory<IRemoteControlService> _clientChannelFactory;

        public RemoteControlManager()
        {
            _clientChannelFactory =
                new ChannelFactory<IRemoteControlService>(
                    new NetNamedPipeBinding());
        }

        public void StartDevelopmentEnvironment()
        {
            Process.Start(Path.Combine(Environment.CurrentDirectory, SharpDevelopExecutablePath));
        }

        private void ExecuteOperation(Action<IRemoteControlService> operation)
        {
            if (IsConnectionEstablished == false)
                throw new InvalidOperationException("There is no remote development environment connected...");
            var client = _clientChannelFactory.CreateChannel(new EndpointAddress(RemoteControlUri));
            try
            {
                operation(client);
            }
            catch (Exception ex)
            {
                // TODO: Log this sucker
                throw;
            }
        }

        public string RemoteControlUri { get; private set; }

        public bool IsConnectionEstablished { get { return RemoteControlUri != null; } }

        private void SetRemoteControlUri(string remoteControlUri)
        {
            RemoteControlUri = remoteControlUri;
        }

        public void RemoteControlAvailableAt(string uri)
        {
            SetRemoteControlUri(uri);
        }

        public void RemoteControlShutDown()
        {
            SetRemoteControlUri(null);
        }

        public void LoadProject(string projectFilePath)
        {
            ExecuteOperation(c => c.LoadProject(projectFilePath));
        }

        public void ShutDownDevelopmentEnvironment()
        {
            ExecuteOperation(c => c.ShutDown());
        }

        public void Dispose()
        {
            ShutDownDevelopmentEnvironment();

            if (_clientChannelFactory != null)
            {
                try
                {
                    _clientChannelFactory.Close();
                }
                catch
                {
                    // Don't throw in dispose
                }
            }
        }
    }
}