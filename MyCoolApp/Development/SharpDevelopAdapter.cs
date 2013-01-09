using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.ServiceModel;
using Caliburn.Micro;
using MyCoolApp.Events;
using MyCoolApp.Events.DevelopmentEnvironment;
using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.Development
{
    public class SharpDevelopAdapter :
        IDisposable,
        IHandle<ProjectLoaded>,
        IHandle<ProjectClosed>,
        IHandle<RemoteControlStarted>,
        IHandle<RemoteControlShutDown>
    {
        public static readonly SharpDevelopAdapter Instance = new SharpDevelopAdapter();
        private const string SharpDevelopExecutablePath = "SharpDevelop\\bin\\SharpDevelop.exe";
        private readonly ChannelFactory<IRemoteControl> _clientChannelFactory;

        public SharpDevelopAdapter()
        {
            Program.GlobalEventAggregator.Subscribe(this);
            _clientChannelFactory =
                new ChannelFactory<IRemoteControl>(
                    new NetNamedPipeBinding());
        }

        public void StartDevelopmentEnvironment(string projectOrSolutionFilePath = null)
        {
            if (IsConnectionEstablished) return;

            Process.Start(
                BuildSharpDevelopExecutablePath(),
                BuildSharpDevelopArgumentString(projectOrSolutionFilePath));
        }

        private static string BuildSharpDevelopArgumentString(string projectOrSolutionFilePath)
        {
            var args = new List<string>();
            if (string.IsNullOrWhiteSpace(projectOrSolutionFilePath) == false)
            {
                if (File.Exists(projectOrSolutionFilePath) == false)
                    throw new Exception(
                        string.Format("The project or solution file does not exist at '{0}'",
                                      projectOrSolutionFilePath));

                args.Add(projectOrSolutionFilePath);
            }

            args.Add(
                string.Format(
                    Constant.HostApplicationListenUriParameterFormat,
                    EventListenerHost.Instance.ListenUri));

            return string.Join(" ", args);
        }

        private static string BuildSharpDevelopExecutablePath()
        {
            return Path.Combine(Environment.CurrentDirectory, SharpDevelopExecutablePath);
        }

        private void ExecuteOperation(Action<IRemoteControl> operation)
        {
            if (IsConnectionEstablished == false)
                throw new InvalidOperationException("There is no remote development environment connected...");
            var client = _clientChannelFactory.CreateChannel(new EndpointAddress(RemoteControlUri));
            try
            {
                operation(client);
            }
            catch
            {
                // TODO: Log this sucker
                throw;
            }
        }

        public string RemoteControlUri { get; private set; }

        public bool IsConnectionEstablished { get { return RemoteControlUri != null; } }

        private void SetRemoteControlUri(string remoteControlUri)
        {
            RemoteControlUri = remoteControlUri;
            OnConnectionStateChanged();
        }

        public void Handle(RemoteControlStarted message)
        {
            SetRemoteControlUri(message.ListenUri);
        }

        public void Handle(RemoteControlShutDown message)
        {
            SetRemoteControlUri(null);
        }

        public void Handle(ProjectLoaded message)
        {
            if (IsConnectionEstablished)
            {
                ExecuteOperation(c => c.LoadProject(ProjectManager.Instance.ProjectScriptingSolutionFilePath));
            }
        }

        public void Handle(ProjectClosed message)
        {
            ShutDownDevelopmentEnvironment();
        }

        public void LoadScriptingProject(string projectFilePath)
        {
            if (IsConnectionEstablished)
            {
                ExecuteOperation(c => c.LoadProject(projectFilePath));
            }
            else
            {
                StartDevelopmentEnvironment(projectFilePath);
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

            if (_clientChannelFactory != null)
            {
                try
                {
                    _clientChannelFactory.Close();
                }
                catch
                {
                    // Don't throw in dispose
                }
            }
        }

        public event EventHandler<EventArgs> ConnectionStateChanged;

        protected virtual void OnConnectionStateChanged()
        {
            var handler = ConnectionStateChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}