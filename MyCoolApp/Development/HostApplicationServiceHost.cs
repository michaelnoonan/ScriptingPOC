using System;
using System.Diagnostics;
using System.ServiceModel;
using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.Development
{
    public class HostApplicationServiceHost : IHostApplicationServiceHost
    {
        public static readonly HostApplicationServiceHost Instance = new HostApplicationServiceHost();

        private const string BaseUri = "net.pipe://localhost/HostApplication";
        public string ListenUri { get { return BaseUri + "/HostApplicationService/" + _processId; } }

        private readonly ServiceHost _serviceHost;
        private readonly int _processId;

        public HostApplicationServiceHost()
        {
            _processId = Process.GetCurrentProcess().Id;
            _serviceHost = new ServiceHost(typeof(HostApplicationService));
        }

        public void StartListening()
        {
            _serviceHost.AddServiceEndpoint(
                typeof(IHostApplicationService),
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