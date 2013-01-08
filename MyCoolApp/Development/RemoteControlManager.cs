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

        private IRemoteControlService GetClient()
        {
            if (RemoteControlUri == null)
                throw new InvalidOperationException("The URI for the remote control service has not been announced.");
            return _clientChannelFactory.CreateChannel(new EndpointAddress(RemoteControlUri));
        }

        public string RemoteControlUri { get; private set; }

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
            GetClient().LoadProject(projectFilePath);
        }

        public void Dispose()
        {
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