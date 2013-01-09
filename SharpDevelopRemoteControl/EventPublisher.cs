using System;
using System.ServiceModel;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using SharpDevelopRemoteControl.Contracts;

namespace SharpDevelopRemoteControl
{
    public class EventPublisher : IDisposable
    {
        public static readonly EventPublisher Instance = new EventPublisher();
        private readonly ChannelFactory<IRemoteControlEventSubscriber> _channelFactory;
        private readonly string _hostApplicationListenUri;

        public EventPublisher()
        {
            foreach (var arg in SharpDevelopMain.CommandLineArgs)
            {
                LoggingService.Info(arg);
                if (arg.StartsWith(Constant.HostApplicationListenUriParameterToken))
                {
                    _hostApplicationListenUri = arg.Replace(Constant.HostApplicationListenUriParameterToken, "").Trim();

                    LoggingService.InfoFormatted("The host application is listening for events on '{0}'",
                                 _hostApplicationListenUri);
                }
            }

            if (_hostApplicationListenUri == null)
            {
                LoggingService.Warn("The HostApplicationListenUri was not specified - not starting the EventPublisher.");
                return;
            }

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

        public bool IsEnabled { get { return _hostApplicationListenUri != null; } }

        public void Start()
        {
            if (IsEnabled)
            {
                WorkbenchSingleton.WorkbenchCreated += AnnounceRemoteControlInterfaceIsReady;
                WorkbenchSingleton.WorkbenchUnloaded += AnnounceRemoteControlInterfaceShuttingDown;
            }
        }

        private void ExecuteOperation(Action<IRemoteControlEventSubscriber> operation)
        {
            if (IsEnabled == false) return;

            var serviceClient = _channelFactory.CreateChannel(new EndpointAddress(_hostApplicationListenUri));

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

        private void AnnounceRemoteControlInterfaceIsReady(object sender, EventArgs e)
        {
            WorkbenchSingleton.WorkbenchCreated -= AnnounceRemoteControlInterfaceIsReady;

            LoggingService.InfoFormatted("Announcing remote control ready on {0}...", CommandListener.Instance.ListenUri);
            ExecuteOperation(c => c.RemoteControlAvailable(CommandListener.Instance.ListenUri));
        }

        private void AnnounceRemoteControlInterfaceShuttingDown(object sender, EventArgs e)
        {
            ExecuteOperation(c => c.ShuttingDown());
        }
    }
}