using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Caliburn.Micro;
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
        IHandle<ScriptingProjectUnloadedInDevelopmentEnvironment>
    {
        public static readonly SharpDevelopIntegrationService Instance =
            new SharpDevelopIntegrationService(
                new ChannelFactory<IRemoteControl>(new NetNamedPipeBinding()),
                new HostApplicationServiceHost(),
                new ScriptingProjectBuilder(),
                ProjectManager.Instance, 
                Program.GlobalEventAggregator);

        private const string SharpDevelopExecutablePath = "SharpDevelop\\bin\\SharpDevelop.exe";
        private readonly IChannelFactory<IRemoteControl> _channelFactory;
        private readonly IHostApplicationServiceHost _hostApplicationServiceHost;
        private readonly IScriptingProjectBuilder _scriptingProjectBuilder;
        private readonly IProjectManager _projectManager;
        private readonly IEventAggregator _globalEventAggregator;
        private Action<IRemoteControl> _operationToRunAfterDevelopmentEnvironmentIsLoaded;
        private Process _sharpDevelopProcess;

        public string RemoteControlUri { get; set; }
        public bool IsConnectionEstablished { get { return RemoteControlUri != null; } }

        public SharpDevelopIntegrationService(
            IChannelFactory<IRemoteControl> channelFactory,
            IHostApplicationServiceHost hostApplicationServiceHost,
            IScriptingProjectBuilder scriptingProjectBuilder,
            IProjectManager projectManager,
            IEventAggregator globalEventAggregator)
        {
            _channelFactory = channelFactory;
            _hostApplicationServiceHost = hostApplicationServiceHost;
            _scriptingProjectBuilder = scriptingProjectBuilder;
            _projectManager = projectManager;
            _globalEventAggregator = globalEventAggregator;
            _globalEventAggregator.Subscribe(this);
        }

        public void LoadScriptingProject(Project project, Action<IRemoteControl> whenProjectHasLoaded = null)
        {
            var scriptingProjectFilePath = project.ScriptingProjectFilePath;
            var scriptingProjectName = project.Name;

            if (File.Exists(scriptingProjectFilePath) == false)
            {
                _scriptingProjectBuilder.BuildScriptingProject(scriptingProjectName, scriptingProjectFilePath);
            }

            if (IsConnectionEstablished)
            {
                ExecuteOperation(c => c.LoadScriptingProject(scriptingProjectFilePath));
            }
            else
            {
                StartDevelopmentEnvironment();
            }
        }

        private void StartDevelopmentEnvironment()
        {
            if (IsConnectionEstablished) return;

            _sharpDevelopProcess = Process.Start(
                BuildSharpDevelopExecutablePath(),
                BuildSharpDevelopArgumentString());

            _sharpDevelopProcess.EnableRaisingEvents = true;
            _sharpDevelopProcess.Exited += SharpDevelopProcessExited;
        }

        private string BuildSharpDevelopArgumentString()
        {
            var args = new List<string>
                           {
                               string.Format(_projectManager.Project.ScriptingProjectFilePath),
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

        private void ExecuteOperation(Action<IRemoteControl> operation)
        {
            if (IsConnectionEstablished)
            {
                var client = _channelFactory.CreateChannel(new EndpointAddress(RemoteControlUri));
                operation(client);
            }
            else
            {
                // Queue it to run when the IDE starts
                _operationToRunAfterDevelopmentEnvironmentIsLoaded = operation;
                StartDevelopmentEnvironment();
            }
        }

        private void SetRemoteControlUri(string remoteControlUri)
        {
            RemoteControlUri = remoteControlUri;
        }

        public void Handle(ProjectLoaded message)
        {
            if (IsConnectionEstablished)
            {
                ExecuteOperation(c => c.LoadScriptingProject(message.LoadedProject.ScriptingProjectFilePath));
            }
        }

        public void Handle(ProjectUnloaded message)
        {
            ShutDownDevelopmentEnvironmentSafely();
        }

        public void Handle(DevelopmentEnvironmentConnected message)
        {
            SetRemoteControlUri(message.ListenUri);

            if (_operationToRunAfterDevelopmentEnvironmentIsLoaded != null)
            {
                var operation = _operationToRunAfterDevelopmentEnvironmentIsLoaded;
                _operationToRunAfterDevelopmentEnvironmentIsLoaded = null;
                ExecuteOperation(operation);
            }
        }

        public void Handle(DevelopmentEnvironmentDisconnected message)
        {
            SetRemoteControlUri(null);
        }

        private void SharpDevelopProcessExited(object sender, EventArgs e)
        {
            try
            {
                if (IsConnectionEstablished)
                {
                    _globalEventAggregator.Publish(new DevelopmentEnvironmentDisconnected());
                }
            }
            finally
            {
                _sharpDevelopProcess.Dispose();
                _sharpDevelopProcess = null;
            }
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
                    ExecuteOperation(c => c.ShutDown());
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