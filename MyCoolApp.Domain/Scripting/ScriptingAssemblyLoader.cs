using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using MyCoolApp.Domain.Diagnostics;
using MyCoolApp.Domain.Events;
using MyCoolApp.Domain.Events.Projects;
using MyCoolApp.Domain.Events.Scripting;
using MyCoolApp.Domain.Projects;

namespace MyCoolApp.Domain.Scripting
{
    public class ScriptingAssemblyLoader :
        IScriptingAssemblyLoader,
        IHandle<ApplicationShuttingDown>,
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

            LoadScriptingAssembly(ignoreMissingAssembly: true);
        }

        private void LoadScriptingAssembly(bool ignoreMissingAssembly = false)
        {
            if (!_projectManager.HasScriptingProject) return;

            var scriptingAssemblyPath = _projectManager.Project.ScriptingAssemblyFilePath;
            if (File.Exists(scriptingAssemblyPath) == false)
            {
                if (ignoreMissingAssembly) return;
                throw new FileNotFoundException("The scripting assembly was not found.", scriptingAssemblyPath);
            }

            // Load the assembly with debugging symbols if available
            var symbolsPath = _projectManager.Project.ScriptingSymbolsFilePath;
            Assembly assembly;
            if (File.Exists(symbolsPath))
            {
                _logger.Info("Loading Scripting assembly and symbols at {0}", scriptingAssemblyPath);
                var assemblyBytes = File.ReadAllBytes(scriptingAssemblyPath);
                var symbolBytes = File.ReadAllBytes(symbolsPath);
                assembly = Assembly.Load(assemblyBytes, symbolBytes);
            }
            else
            {
                _logger.Info("Loading Scripting assembly without symbols {0}", scriptingAssemblyPath);
                var assemblyBytes = File.ReadAllBytes(scriptingAssemblyPath);
                assembly = Assembly.Load(assemblyBytes);
            }

            // Keep a reference so we can late bind to this assembly by assembly name as required
            _loadedScriptingAssemblies.AddOrUpdate(assembly.FullName, assembly, (key, existingAssembly) => assembly);
            _currentScriptingAssembly = assembly;
            _logger.Info("Loaded {0}", assembly.FullName);
            
            // Let the world know we have a new scripting assembly with new scripts available
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
                LoadScriptingAssembly();
            }
        }

        public async Task<Assembly> GetAssemblyAsync(string assemblyName, TimeSpan timeToWaitForAssembly)
        {
            var timeoutExpiry = DateTime.Now + timeToWaitForAssembly;
            while (DateTime.Now < timeoutExpiry)
            {
                if (_loadedScriptingAssemblies.ContainsKey(assemblyName))
                {
                    return _loadedScriptingAssemblies[assemblyName];
                }

                await Sleeper.SleepAsync(500);
            }

            return null;
        }

        private void Clear()
        {
            _currentScriptingAssembly = null;
            _loadedScriptingAssemblies.Clear();
        }

        public void Handle(ProjectLoaded message)
        {
            Clear();
            LoadScriptingAssemblyIfAvailable();
        }

        public void Handle(ProjectUnloaded message)
        {
            Clear();
        }

        public void Handle(ApplicationShuttingDown message)
        {
            _globalEventAggregator.Unsubscribe(this);
            Clear();
        }
    }
}