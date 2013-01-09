using System;
using System.Diagnostics;
using System.ServiceModel;
using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.Development
{
    public class EventListener : IDisposable
    {
        public static readonly EventListener Instance = new EventListener();

        private const string BaseUri = "net.pipe://localhost/HostApplication";
        public string ListenUri { get { return BaseUri + "/EventSubscriber/" + _processId; } }

        private readonly ServiceHost _serviceHost;
        private readonly int _processId;

        public EventListener()
        {
            _processId = Process.GetCurrentProcess().Id;
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