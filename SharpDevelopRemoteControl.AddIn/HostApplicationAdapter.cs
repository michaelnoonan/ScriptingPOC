using System;
using System.ServiceModel;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using SharpDevelopRemoteControl.Contracts;

namespace SharpDevelopRemoteControl.AddIn
{
    public class HostApplicationAdapter : IDisposable
    {
        public static readonly HostApplicationAdapter Instance = new HostApplicationAdapter(new CommandListener());

        private readonly ICommandListener _commandListener;
        private readonly ChannelFactory<IHostApplicationService> _channelFactory;
        private readonly string _hostApplicationListenUri;
        private readonly int _hostApplicationProcessId;

        public int HostApplicationProcessId
        {
            get { return _hostApplicationProcessId; }
        }

        public HostApplicationAdapter(ICommandListener commandListener)
        {
            _commandListener = commandListener;
            foreach (var arg in SharpDevelopMain.CommandLineArgs)
            {
                LoggingService.Info(arg);
                if (arg.StartsWith(Constant.HostApplicationListenUriParameterToken))
                {
                    _hostApplicationListenUri = arg.Replace(Constant.HostApplicationListenUriParameterToken, "").Trim();

                    LoggingService.InfoFormatted("The host application is listening for events on '{0}'",
                                 _hostApplicationListenUri);
                }

                if (arg.StartsWith(Constant.HostApplicationProcessIdParameterToken))
                {
                    _hostApplicationProcessId = int.Parse(arg.Replace(Constant.HostApplicationProcessIdParameterToken, "").Trim());

                    LoggingService.InfoFormatted("The host application ProcessId is '{0}'", _hostApplicationProcessId);
                }
            }

            if (_hostApplicationListenUri == null)
            {
                LoggingService.Warn("The HostApplicationListenUri was not specified - not starting the HostApplicationAdapter.");
                return;
            }

            _channelFactory =
                new ChannelFactory<IHostApplicationService>(
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
                WorkbenchSingleton.WorkbenchUnloaded += AnnounceRemoteControlInterfaceShuttingDownSafely;
                ProjectService.SolutionClosed += AnnounceSolutionClosed;
            }
        }

        private TResult ExecuteOperation<TResult>(Func<IHostApplicationService, TResult> operation)
        {
            if (IsEnabled == false) return default(TResult);

            var serviceClient = _channelFactory.CreateChannel(new EndpointAddress(_hostApplicationListenUri));

            try
            {
                return operation(serviceClient);
            }
            catch (Exception ex)
            {
                LoggingService.Error("Failed to execute operation on RemoteControlHostService", ex);
                throw;
            }
        }

        private void ExecuteOperation(Action<IHostApplicationService> operation)
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

            var listenUri = _commandListener.ListenUri;
            LoggingService.InfoFormatted("Announcing remote control ready on {0}...", listenUri);
            ExecuteOperation(c => c.RemoteControlAvailable(listenUri));
        }

        private void AnnounceRemoteControlInterfaceShuttingDownSafely(object sender, EventArgs e)
        {
            try
            {
                ExecuteOperation(c => c.DevelopmentEnvironmentShuttingDown());
            }
            catch
            {
                // Deliberately discard any exception on shutdown
                // It's likely the host application has gone away...
            }
        }

        private void AnnounceSolutionClosed(object sender, EventArgs eventArgs)
        {
            try
            {
                ExecuteOperation(c => c.ScriptingProjectUnloaded());
            }
            catch
            {
                // Deliberately discard any exception on shutdown
                // It's likely the host application has gone away...
            }
        }

        public void AnnounceScriptingProjectLoaded(LoadScriptingProjectResult result)
        {
            ExecuteOperation(c => c.ScriptingProjectLoaded(result));
        }

        public ScriptExecutionResult ExecuteScriptForDebugging(string assemblyName, string className, string methodName)
        {
            return ExecuteOperation(c => c.ExecuteScriptForDebugging(assemblyName, className, methodName));
        }
    }
}