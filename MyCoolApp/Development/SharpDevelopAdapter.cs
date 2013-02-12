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
    public class SharpDevelopAdapter :
        IDisposable,
        IHandle<ProjectLoaded>,
        IHandle<ProjectClosed>,
        IHandle<DevelopmentEnvironmentConnected>,
        IHandle<DevelopmentEnvironmentDisconnected>, ISharpDevelopAdapter
    {
        public static readonly SharpDevelopAdapter Instance =
            new SharpDevelopAdapter(
                new ChannelFactory<IRemoteControl>(new NetNamedPipeBinding()),
                new HostApplicationServiceHost(),
                new ScriptingProjectBuilder(),
                Program.GlobalEventAggregator);

        private const string SharpDevelopExecutablePath = "SharpDevelop\\bin\\SharpDevelop.exe";
        private readonly IChannelFactory<IRemoteControl> _channelFactory;
        private readonly IHostApplicationServiceHost _hostApplicationServiceHost;
        private readonly IScriptingProjectBuilder _scriptingProjectBuilder;
        private readonly IEventAggregator _globalEventAggregator;
        private Action<IRemoteControl> _actionToRunAfterSharpDevelopIsLoaded;
        private Process _sharpDevelopProcess;

        public string RemoteControlUri { get; private set; }
        public bool IsConnectionEstablished { get { return RemoteControlUri != null; } }

        public SharpDevelopAdapter(
            IChannelFactory<IRemoteControl> channelFactory,
            IHostApplicationServiceHost hostApplicationServiceHost,
            IScriptingProjectBuilder scriptingProjectBuilder,
            IEventAggregator globalEventAggregator)
        {
            _channelFactory = channelFactory;
            _hostApplicationServiceHost = hostApplicationServiceHost;
            _scriptingProjectBuilder = scriptingProjectBuilder;
            _globalEventAggregator = globalEventAggregator;
            _globalEventAggregator.Subscribe(this);
        }

        public void StartDevelopmentEnvironment(string projectOrSolutionFilePath = null)
        {
            if (IsConnectionEstablished) return;

            _sharpDevelopProcess = Process.Start(
                BuildSharpDevelopExecutablePath(),
                BuildSharpDevelopArgumentString(projectOrSolutionFilePath));

            _sharpDevelopProcess.EnableRaisingEvents = true;
            _sharpDevelopProcess.Exited += SharpDevelopProcessExited;
        }

        private string BuildSharpDevelopArgumentString(string projectOrSolutionFilePath)
        {
            var args = new List<string>();
            if (string.IsNullOrWhiteSpace(projectOrSolutionFilePath) == false)
            {
                args.Add(projectOrSolutionFilePath);
            }

            args.Add(
                string.Format(
                    Constant.HostApplicationListenUriParameterFormat,
                    _hostApplicationServiceHost.ListenUri));

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
                _actionToRunAfterSharpDevelopIsLoaded = operation;
                StartDevelopmentEnvironment();
            }
        }

        private void SetRemoteControlUri(string remoteControlUri)
        {
            RemoteControlUri = remoteControlUri;
        }

        public void Handle(DevelopmentEnvironmentConnected message)
        {
            SetRemoteControlUri(message.ListenUri);
            if (_actionToRunAfterSharpDevelopIsLoaded != null)
            {
                var action = _actionToRunAfterSharpDevelopIsLoaded;
                _actionToRunAfterSharpDevelopIsLoaded = null;
                ExecuteOperation(action);
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

        public void Handle(ProjectLoaded message)
        {
            if (IsConnectionEstablished)
            {
                ExecuteOperation(c => c.LoadScriptingProject(message.LoadedProject.ScriptingProjectFilePath));
            }
        }

        public void Handle(ProjectClosed message)
        {
            ShutDownDevelopmentEnvironment();
        }

        public void LoadScriptingProject(Project project)
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
                StartDevelopmentEnvironment(scriptingProjectFilePath);
            }
        }

        public void ShutDownDevelopmentEnvironment()
        {
            if (IsConnectionEstablished)
            {
                ExecuteOperation(c => c.ShutDown());
            }
        }

        public void Dispose()
        {
            ShutDownDevelopmentEnvironment();

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