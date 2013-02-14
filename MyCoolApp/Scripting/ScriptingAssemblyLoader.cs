using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using MyCoolApp.Diagnostics;
using MyCoolApp.Events;
using MyCoolApp.Events.Scripting;
using MyCoolApp.Projects;

namespace MyCoolApp.Scripting
{
    public class ScriptingAssemblyLoader :
        IScriptingAssemblyLoader,
        IHandle<ProjectLoaded>,
        IHandle<ProjectUnloaded>
    {
        private readonly IEventAggregator _globalEventAggregator;
        private readonly ILogger _logger;
        private readonly ScriptingAssemblyFileWatcher _watcher;
        private readonly IProjectManager _projectManager;
        private readonly ConcurrentDictionary<string, Assembly> _loadedScriptingAssemblies = new ConcurrentDictionary<string, Assembly>();
        private Assembly _currentScriptingAssembly;

        public Assembly CurrentScriptingAssembly
        {
            get { return _currentScriptingAssembly; }
        }

        public ScriptingAssemblyLoader(
            IProjectManager projectManager,
            ScriptingAssemblyFileWatcher watcher,
            IEventAggregator globalEventAggregator,
            ILogger logger)
        {
            _globalEventAggregator = globalEventAggregator;
            _logger = logger;
            _watcher = watcher;
            _watcher.NewScriptingAssemblyAvailable += NewScriptingAssemblyAvailable;
            _projectManager = projectManager;

            _globalEventAggregator.Subscribe(this);
        }

        private void LoadScriptingAssemblyIfAvailable()
        {
            if (!_projectManager.HasScriptingProject) return;

            LoadScriptingAssembly(_projectManager.Project.ScriptingAssemblyFilePath, ignoreMissingAssembly: true);
        }

        private void LoadScriptingAssembly(string scriptingAssemblyPath, bool ignoreMissingAssembly = false)
        {
            if (!_projectManager.HasScriptingProject) return;

            if (scriptingAssemblyPath != _projectManager.Project.ScriptingAssemblyFilePath)
                throw new InvalidOperationException(
                    string.Format(
                        "The scripting assembly path did not match what was expected by the project. Expected: '{0}' Actual: '{1}'",
                        _projectManager.Project.ScriptingAssemblyFilePath, scriptingAssemblyPath));

            if (ignoreMissingAssembly == false && File.Exists(scriptingAssemblyPath) == false)
                throw new FileNotFoundException("The scripting assembly was not found.", scriptingAssemblyPath);

            var project = _projectManager.Project;
            var fileToLoad = scriptingAssemblyPath ?? project.ScriptingAssemblyFilePath;

            _logger.Info("Loading Scripting assembly at {0}", project.ScriptingAssemblyFilePath);
            var assemblyBytes = File.ReadAllBytes(project.ScriptingAssemblyFilePath);
            var symbolBytes = File.ReadAllBytes(project.ScriptingSymbolsFilePath);
            var assembly = Assembly.Load(assemblyBytes, symbolBytes);

            _loadedScriptingAssemblies.AddOrUpdate(assembly.FullName, assembly, (key, existingAssembly) => assembly);
            _currentScriptingAssembly = assembly;
            _logger.Info("Loaded {0}", assembly.FullName);
            
            var scriptNames = _currentScriptingAssembly
                .GetTypes()
                .Where(t => t.GetMethods().Any(m => m.Name == "Main" && m.IsStatic && !m.GetParameters().Any()))
                .OrderBy(t => t.FullName)
                .Select(t => t.FullName)
                .ToArray();
            _logger.Info("Available scripts are {0}", string.Join(", ", scriptNames));
            _globalEventAggregator.Publish(new ScriptingAssemblyLoaded(_currentScriptingAssembly.FullName, scriptNames));
        }

        private void NewScriptingAssemblyAvailable(object sender, NewScriptingAssemblyEventArgs e)
        {
            if (_projectManager.IsProjectLoaded &&
                e.ScriptingAssemblyPath == _projectManager.Project.ScriptingAssemblyFilePath)
            {
                LoadScriptingAssembly(e.ScriptingAssemblyPath);
            }
        }

        public async Task<Assembly> GetAssemblyWithWait(string assemblyName, TimeSpan timeToWaitForAssembly)
        {
            var timeoutExpiry = DateTime.Now + timeToWaitForAssembly;
            while (DateTime.Now < timeoutExpiry)
            {
                if (_loadedScriptingAssemblies.ContainsKey(assemblyName))
                {
                    return _loadedScriptingAssemblies[assemblyName];
                }

                await SleepAsync(500);
            }

            return null;
        }

        public Task SleepAsync(int millisecondsTimeout)
        {
            TaskCompletionSource<bool> tcs = null;
            var t = new Timer(unusedState => tcs.TrySetResult(true), null, -1, -1);
            tcs = new TaskCompletionSource<bool>(t);
            t.Change(millisecondsTimeout, -1);
            return tcs.Task;
        }

        public void Handle(ProjectLoaded message)
        {
            _currentScriptingAssembly = null;
            _loadedScriptingAssemblies.Clear();
            LoadScriptingAssemblyIfAvailable();
        }

        public void Handle(ProjectUnloaded message)
        {
            _currentScriptingAssembly = null;
        }
    }
}