using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using MyCoolApp.Diagnostics;
using MyCoolApp.Events;
using MyCoolApp.Events.DevelopmentEnvironment;
using MyCoolApp.Projects;
using MyCoolApp.Scripting;
using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.Development
{
    public class SharpDevelopIntegrationService :
        ISharpDevelopIntegrationService,
        IDisposable,
        IHandle<ProjectLoaded>,
        IHandle<ProjectUnloaded>,
        IHandle<DevelopmentEnvironmentConnected>,
        IHandle<DevelopmentEnvironmentDisconnected>,
        IHandle<ScriptingProjectLoadedInDevelopmentEnvironment>,
        IHandle<ScriptingProjectUnloadedInDevelopmentEnvironment>
    {
        public static readonly SharpDevelopIntegrationService Instance =
            new SharpDevelopIntegrationService(
                new ChannelFactory<IRemoteControl>(new NetNamedPipeBinding()),
                new HostApplicationServiceHost(),
                new ScriptingProjectBuilder(),
                ProjectManager.Instance,
                Program.GlobalEventAggregator,
                Logger.Instance);

        private const string SharpDevelopExecutablePath = "SharpDevelop\\bin\\SharpDevelop.exe";
        private readonly IChannelFactory<IRemoteControl> _channelFactory;
        private readonly IHostApplicationServiceHost _hostApplicationServiceHost;
        private readonly IScriptingProjectBuilder _scriptingProjectBuilder;
        private readonly IProjectManager _projectManager;
        private readonly IEventAggregator _globalEventAggregator;
        private readonly ILogger _logger;
        private Process _sharpDevelopProcess;

        public string RemoteControlUri { get; private set; }
        public bool IsConnectionEstablished { get { return RemoteControlUri != null; } }

        private LoadScriptingProjectResult _loadScriptingProjectResult;

        public SharpDevelopIntegrationService(
            IChannelFactory<IRemoteControl> channelFactory,
            IHostApplicationServiceHost hostApplicationServiceHost,
            IScriptingProjectBuilder scriptingProjectBuilder,
            IProjectManager projectManager,
            IEventAggregator globalEventAggregator,
            ILogger logger)
        {
            _channelFactory = channelFactory;
            _hostApplicationServiceHost = hostApplicationServiceHost;
            _scriptingProjectBuilder = scriptingProjectBuilder;
            _projectManager = projectManager;
            _globalEventAggregator = globalEventAggregator;
            _logger = logger;
            _globalEventAggregator.Subscribe(this);
        }

        public async Task StartDebuggingScriptAsync(string className)
        {
            var result = await LoadAndBuildScriptingProjectAsync();
            if (result.IsProjectBuildingSuccessfully)
            {
                var client = _channelFactory.CreateChannel(new EndpointAddress(RemoteControlUri));
                client.StartDebuggingScript(className);
            }
        }

        public async Task<LoadScriptingProjectResult> LoadAndBuildScriptingProjectAsync()
        {
            var project = _projectManager.Project;
            var scriptingProjectFilePath = project.ScriptingProjectFilePath;
            var scriptingProjectName = project.Name;

            if (File.Exists(scriptingProjectFilePath) == false)
            {
                _scriptingProjectBuilder.BuildScriptingProject(scriptingProjectName, scriptingProjectFilePath);
            }

            await StartDevelopmentEnvironment();
            var result = await LoadProjectInDevelopmentEnvironment(scriptingProjectFilePath);
            return result;
        }

        private async Task<LoadScriptingProjectResult> LoadProjectInDevelopmentEnvironment(string scriptingProjectFilePath)
        {
            var client = _channelFactory.CreateChannel(new EndpointAddress(RemoteControlUri));
            client.LoadScriptingProject(scriptingProjectFilePath);

            // I can't seem to get a reliable synchronous response from the result of a Build in SharpDevelop
            // Instead I will wait asynchronously... with some sleeping.
            while (_loadScriptingProjectResult == null)
            {
                _logger.Info("Waiting for project to load and finish building...");
                await SleepAsync(1000);
            }

            var result = _loadScriptingProjectResult;
            _loadScriptingProjectResult = null;
            return result;
        }

        private async Task StartDevelopmentEnvironment()
        {
            if (IsConnectionEstablished) return;

            _sharpDevelopProcess = Process.Start(
                BuildSharpDevelopExecutablePath(),
                BuildSharpDevelopArgumentString());

            _sharpDevelopProcess.EnableRaisingEvents = true;
            _sharpDevelopProcess.Exited += SharpDevelopProcessExited;

            while (IsConnectionEstablished == false)
            {
                _logger.Info("Waiting for development environment to finish starting...");
                await SleepAsync(1000);
            }
        }

        public Task SleepAsync(int millisecondsTimeout)
        {
            TaskCompletionSource<bool> tcs = null;
            var t = new Timer(unusedState => tcs.TrySetResult(true), null, -1, -1);
            tcs = new TaskCompletionSource<bool>(t);
            t.Change(millisecondsTimeout, -1);
            return tcs.Task;
        }

        private string BuildSharpDevelopArgumentString()
        {
            var args = new List<string>
                           {
                               //string.Format(_projectManager.Project.ScriptingProjectFilePath),
                               string.Format(
                                   Constant.HostApplicationListenUriParameterFormat,
                                   _hostApplicationServiceHost.ListenUri),
                               string.Format(
                                   Constant.HostApplicationProcessIdParameterFormat,
                                   Process.GetCurrentProcess().Id)
                           };

            return string.Join(" ", args);
        }

        private static string BuildSharpDevelopExecutablePath()
        {
            return Path.Combine(Environment.CurrentDirectory, SharpDevelopExecutablePath);
        }

        private void SetRemoteControlUri(string remoteControlUri)
        {
            RemoteControlUri = remoteControlUri;
        }

        public void Handle(ProjectLoaded message)
        {
            if (IsConnectionEstablished)
            {
                var client = _channelFactory.CreateChannel(new EndpointAddress(RemoteControlUri));
                client.LoadScriptingProject(message.LoadedProject.ScriptingProjectFilePath);
            }
        }

        public void Handle(ProjectUnloaded message)
        {
            ShutDownDevelopmentEnvironmentSafely();
        }

        public void Handle(DevelopmentEnvironmentConnected message)
        {
            SetRemoteControlUri(message.ListenUri);
        }

        public void Handle(DevelopmentEnvironmentDisconnected message)
        {
            ShutDownDevelopmentEnvironmentSafely();
        }

        private void SharpDevelopProcessExited(object sender, EventArgs e)
        {
            if (IsConnectionEstablished)
            {
                _globalEventAggregator.Publish(new DevelopmentEnvironmentDisconnected());
            }

            try
            {
                ShutDownDevelopmentEnvironmentSafely();
            }
            finally
            {
                _sharpDevelopProcess.Dispose();
                _sharpDevelopProcess = null;
            }
        }

        public void Handle(ScriptingProjectLoadedInDevelopmentEnvironment message)
        {
            if (message.Result.ScriptingProjectFilePath != _projectManager.Project.ScriptingProjectFilePath)
                throw new Exception(string.Format("The loaded project doesn't match the expected one. Expected: {0} Actual {1}",
                    _projectManager.Project.ScriptingProjectFilePath, message.Result.ScriptingProjectFilePath));
            _loadScriptingProjectResult = message.Result;
        }

        public void Handle(ScriptingProjectUnloadedInDevelopmentEnvironment message)
        {
            ShutDownDevelopmentEnvironmentSafely();
        }

        private void ShutDownDevelopmentEnvironmentSafely()
        {
            if (IsConnectionEstablished)
            {
                try
                {
                    var client = _channelFactory.CreateChannel(new EndpointAddress(RemoteControlUri));
                    client.ShutDown();
                    SetRemoteControlUri(null);
                }
                catch
                {
                    // Deliberately ignore any exception here since the most likely one is the environment is already gone!
                }
            }
        }

        public void Dispose()
        {
            ShutDownDevelopmentEnvironmentSafely();

            if (_channelFactory != null)
            {
                try
                {
                    _channelFactory.Close();
                }
                catch
                {
                    // Don't throw in dispose
                }
            }
        }
    }
}