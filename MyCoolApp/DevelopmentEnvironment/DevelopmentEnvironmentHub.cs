using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.ServiceModel;
using System.Threading;
using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.DevelopmentEnvironment
{
    public class DevelopmentEnvironmentHub : IDevelopmentEnvironmentHub, IDisposable
    {
        public static readonly DevelopmentEnvironmentHub Instance = new DevelopmentEnvironmentHub();

        private ServiceHost _serviceHost;
        private readonly ChannelFactory<IRemoteControlService> _clientChannelFactory;

        public DevelopmentEnvironmentHub()
        {
            StartAnnouncementHost();

            _clientChannelFactory = new ChannelFactory<IRemoteControlService>(
                new NetNamedPipeBinding());
        }

        private void StartAnnouncementHost()
        {
            _serviceHost = new ServiceHost(
                typeof (RemoteControlAnnouncementService),
                new Uri("net.pipe://localhost/ScriptingPOC"));

            _serviceHost.AddServiceEndpoint(
                typeof (IRemoteControlAnnouncementService),
                new NetNamedPipeBinding(),
                "RemoteControlAnnouncement");

            _serviceHost.Open();
        }

        public void StartDevelopmentEnvironment()
        {
            Process.Start(Path.Combine(Environment.CurrentDirectory, "SharpDevelop\\bin\\SharpDevelop.exe"));
        }

        private IRemoteControlService GetRemoteControlService()
        {
            if (RemoteControlUri == null) throw new InvalidOperationException("The URI for the remote control service has not been announced.");
            return _clientChannelFactory.CreateChannel(new EndpointAddress(RemoteControlUri));
        }

        public void Dispose()
        {
            try
            {
                _serviceHost.Close();
            }
            catch {}
        }

        public string RemoteControlUri { get; private set; }

        public void SetRemoteControlUri(string remoteControlUri)
        {
            RemoteControlUri = remoteControlUri;
        }

        public void LoadProject(string projectFilePath)
        {
            GetRemoteControlService().LoadProject(projectFilePath);
        }
    }

    public interface IDevelopmentEnvironmentHub
    {
        string RemoteControlUri { get; }
    }
}