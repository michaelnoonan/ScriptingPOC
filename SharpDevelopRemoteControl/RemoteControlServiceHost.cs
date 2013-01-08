using System;
using System.ServiceModel;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using SharpDevelopRemoteControl.Contracts;

namespace SharpDevelopRemoteControl
{
    public class RemoteControlServiceHost : IDisposable
    {
        private const string BaseUri = "net.pipe://localhost/SharpDevelop";
        public static readonly RemoteControlServiceHost Instance = new RemoteControlServiceHost();
        private readonly ServiceHost ServiceHost =
            new ServiceHost(
                typeof(RemoteControlService),
                new Uri(BaseUri));

        private bool isDisposed;

        public void Start()
        {
            if (isDisposed)throw new ObjectDisposedException("The RemoteControlServiceHost has already been disposed.");

            LoggingService.Debug("Starting remote control interface...");
            ServiceHost.AddServiceEndpoint(
                typeof (IRemoteControlService),
                new NetNamedPipeBinding(),
                "RemoteControl");
            ServiceHost.Open();

            WorkbenchSingleton.WorkbenchCreated += RockAndRoll;
        }

        private void RockAndRoll(object sender, EventArgs e)
        {
            LoggingService.Debug("Announcing...");
            var channelFactory = new ChannelFactory<IRemoteControlAnnouncementService>(
                new NetNamedPipeBinding(),
                new EndpointAddress(new Uri("net.pipe://localhost/ScriptingPOC/RemoteControlAnnouncement")));

            try
            {
                var proxy = channelFactory.CreateChannel();
                proxy.RemoteControlAvailable(BaseUri + "/RemoteControl");
                LoggingService.Debug("Announced");
            }
            finally
            {
                channelFactory.Close();
            }
        }

        public void Stop()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (isDisposed) return;

            if (ServiceHost.State == CommunicationState.Opened)
            {
                try
                {
                    ServiceHost.Close();
                }
                catch
                {
                    // Don't throw in Dispose method
                }
            }
        }
    }
}