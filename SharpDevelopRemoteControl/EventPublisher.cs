using System;
using System.ServiceModel;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using SharpDevelopRemoteControl.Contracts;

namespace SharpDevelopRemoteControl
{
    public class EventPublisher : IDisposable
    {
        public static readonly EventPublisher Instance = new EventPublisher();
        private const string SubscriberBaseUri = "net.pipe://localhost/HostApplication/EventSubscriber";
        private string SubscriberUri { get { return SubscriberBaseUri; } }
        private readonly ChannelFactory<IRemoteControlEventSubscriber> _channelFactory;

        public EventPublisher()
        {
            _channelFactory =
                new ChannelFactory<IRemoteControlEventSubscriber>(
                    new NetNamedPipeBinding());
        }

        public void Dispose()
        {
            if (_channelFactory != null)
            {
                try
                {
                    _channelFactory.Close();
                }
                catch
                {
                    // Don't throw in Dispose method
                }
            }
        }

        public void Start()
        {
            WorkbenchSingleton.WorkbenchCreated += AnnounceRemoteControlInterfaceIsReady;
            WorkbenchSingleton.WorkbenchUnloaded += AnnounceRemoteControlInterfaceShuttingDown;
        }

        private void ExecuteOperation(Action<IRemoteControlEventSubscriber> operation)
        {
            var serviceClient = _channelFactory.CreateChannel(new EndpointAddress(SubscriberUri));

            try
            {
                operation(serviceClient);
            }
            catch (Exception ex)
            {
                LoggingService.Error("Failed to execute operation on RemoteControlHostService", ex);
                throw;
            }
        }

        private void AnnounceRemoteControlInterfaceShuttingDown(object sender, EventArgs e)
        {
            ExecuteOperation(c => c.ShuttingDown());
        }

        private void AnnounceRemoteControlInterfaceIsReady(object sender, EventArgs e)
        {
            WorkbenchSingleton.WorkbenchCreated -= AnnounceRemoteControlInterfaceIsReady;

            LoggingService.Debug("Announcing remote control ready...");
            ExecuteOperation(c => c.RemoteControlAvailable(CommandReceiver.Instance.Uri));
            LoggingService.Info("Remote control service is ready.");
        }
    }
}