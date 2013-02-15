using System;
using System.Diagnostics;
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

        private readonly string _hostApplicationListenUri;
        private readonly int? _hostApplicationProcessId;
        private readonly ICommandListener _commandListener;
        private readonly ChannelFactory<IHostApplicationService> _channelFactory;
        private Process _hostApplicationProcess;

        public int? HostApplicationProcessId
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

            if (_hostApplicationProcessId == null)
            {
                LoggingService.Warn("The HostApplicationProcessId was not specified - not starting the HostApplicationAdapter.");
                return;
            }

            _hostApplicationProcess = Process.GetProcessById(_hostApplicationProcessId.Value);
            _hostApplicationProcess.EnableRaisingEvents = true;
            _hostApplicationProcess.Exited += HostApplicationProcessExited;

            _channelFactory =
                new ChannelFactory<IHostApplicationService>(
                    new NetNamedPipeBinding());
        }

        private void HostApplicationProcessExited(object sender, EventArgs e)
        {
            LoggingService.Info("The host process has terminated so we will close too!");
            WorkbenchSingleton.SafeThreadAsyncCall(WorkbenchSingleton.MainWindow.Close);
        }

        public void Dispose()
        {
            if (_hostApplicationProcess != null)
            {
                _hostApplicationProcess.Dispose();
                _hostApplicationProcess = null;
            }
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

        public void PublishEvent(Action<IHostApplicationService> operation)
        {
            if (IsEnabled == false) return;

            var serviceClient = _channelFactory.CreateChannel(new EndpointAddress(_hostApplicationListenUri));

            try
            {
                operation(serviceClient);
            }
            catch (Exception ex)
            {
                LoggingService.Error("Failed to publish the event...", ex);
            }
        }

        private void AnnounceRemoteControlInterfaceIsReady(object sender, EventArgs e)
        {
            WorkbenchSingleton.WorkbenchCreated -= AnnounceRemoteControlInterfaceIsReady;

            var listenUri = _commandListener.ListenUri;
            LoggingService.InfoFormatted("Announcing remote control ready on {0}...", listenUri);
            PublishEvent(c => c.RemoteControlAvailable(listenUri));
        }

        private void AnnounceRemoteControlInterfaceShuttingDownSafely(object sender, EventArgs e)
        {
            PublishEvent(c => c.DevelopmentEnvironmentShuttingDown());
        }

        private void AnnounceSolutionClosed(object sender, EventArgs eventArgs)
        {
            PublishEvent(c => c.ScriptingProjectUnloaded());
        }

        public void AnnounceScriptingProjectLoaded(LoadScriptingProjectResult result)
        {
            PublishEvent(c => c.ScriptingProjectLoaded(result));
        }

        public ScriptExecutionResult ExecuteScriptForDebugging(string assemblyName, string className, string methodName)
        {
            return ExecuteOperation(c => c.ExecuteScriptForDebugging(assemblyName, className, methodName));
        }
    }
}