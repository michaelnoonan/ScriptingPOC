using System;
using System.Diagnostics;
using System.ServiceModel;
using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.Development
{
    public class EventListenerHost : IDisposable
    {
        public static readonly EventListenerHost Instance = new EventListenerHost();

        private const string BaseUri = "net.pipe://localhost/HostApplication";
        public string ListenUri { get { return BaseUri + "/EventListener/" + _processId; } }

        private readonly ServiceHost _serviceHost;
        private readonly int _processId;

        public EventListenerHost()
        {
            _processId = Process.GetCurrentProcess().Id;
            _serviceHost = new ServiceHost(typeof(EventListener));
        }

        public void StartListening()
        {
            _serviceHost.AddServiceEndpoint(
                typeof(IDevelopmentEnvironmentEventListener),
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