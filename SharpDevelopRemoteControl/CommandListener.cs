using System;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Description;
using ICSharpCode.Core;
using SharpDevelopRemoteControl.Contracts;

namespace SharpDevelopRemoteControl
{
    public class CommandListener : IDisposable
    {
        public static readonly CommandListener Instance = new CommandListener();
        
        private const string BaseUri = "net.pipe://localhost/SharpDevelop";
        public string ListenUri { get { return BaseUri + "/RemoteControl/" + _processId; } }

        private readonly ServiceHost _serviceHost;
        private bool _isDisposed;
        private readonly int _processId;

        public CommandListener()
        {
            // Use the process Id to create a unique name for the NamedPipe
            _processId = Process.GetCurrentProcess().Id;
            _serviceHost = new ServiceHost(typeof(RemoteControl));
            ConfigureServiceHost(_serviceHost);
        }

        private void ConfigureServiceHost(ServiceHost serviceHost)
        {
            var debug = serviceHost.Description.Behaviors.Find<ServiceDebugBehavior>();

            // if not found - add behavior with setting turned on 
            if (debug == null)
            {
                serviceHost.Description.Behaviors.Add(
                     new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });
            }
            else
            {
                // make sure setting is turned ON
                if (!debug.IncludeExceptionDetailInFaults)
                {
                    debug.IncludeExceptionDetailInFaults = true;
                }
            }

        }

        public void Start()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("The CommandListener has already been disposed.");

            LoggingService.Debug("Starting remote control interface...");
            _serviceHost.AddServiceEndpoint(
                typeof (IRemoteControl),
                new NetNamedPipeBinding(),
                new Uri(ListenUri));

            _serviceHost.Open();
        }

        public void Stop()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;

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