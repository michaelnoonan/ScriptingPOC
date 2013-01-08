using System;
using System.ServiceModel;
using ICSharpCode.Core;
using SharpDevelopRemoteControl.Contracts;

namespace SharpDevelopRemoteControl
{
    public class CommandReceiver : IDisposable
    {
        private const string BaseUri = "net.pipe://localhost/SharpDevelop";
        public string Uri { get { return BaseUri + "RemoteControl"; } }
        
        public static readonly CommandReceiver Instance = new CommandReceiver();

        private readonly ServiceHost _serviceHost =
            new ServiceHost(typeof(RemoteControlService));

        private bool isDisposed;

        public void Start()
        {
            if (isDisposed)
                throw new ObjectDisposedException("The CommandReceiver has already been disposed.");

            LoggingService.Debug("Starting remote control interface...");
            _serviceHost.AddServiceEndpoint(
                typeof (IRemoteControlService),
                new NetNamedPipeBinding(),
                new Uri(Uri));

            _serviceHost.Open();
        }

        public void Stop()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (isDisposed) return;

            try
            {
                _serviceHost.Close();
            }
            catch
            {
                // Don't throw in Dispose method
            }
        }
    }
}