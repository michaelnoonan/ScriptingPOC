using System;
using System.ServiceModel;
using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.Development
{
    public class RemoteControlEventListener : IDisposable
    {
        public static readonly RemoteControlEventListener Instance = new RemoteControlEventListener();

        private const string BaseUri = "net.pipe://localhost/HostApplication";
        public string ListenUri { get { return BaseUri + "/EventSubscriber"; } }

        private readonly ServiceHost _serviceHost;

        public RemoteControlEventListener()
        {
            _serviceHost = new ServiceHost(typeof(RemoteControlEventSubscriber));
        }

        public void StartListening()
        {
            _serviceHost.AddServiceEndpoint(
                typeof(IRemoteControlEventSubscriber),
                new NetNamedPipeBinding(),
                new Uri(ListenUri));

            _serviceHost.Open();
        }

        public void Dispose()
        {
            try
            {
                _serviceHost.Close();
            }
            catch
            {
                // Don't throw in dispose
            }
        }
    }
}