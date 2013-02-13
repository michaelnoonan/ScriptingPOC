using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Caliburn.Micro;
using MyCoolApp.Events;
using MyCoolApp.Events.Scripting;
using MyCoolApp.Projects;
using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.Scripting
{
    public class ScriptingService :
        IScriptingService,
        IHandle<ProjectLoaded>,
        IHandle<ProjectUnloaded>
    {
        public static ScriptingService Instance =
            new ScriptingService(
                ProjectManager.Instance,
                Program.GlobalEventAggregator,
                ScriptExecutor.Instance,
                new ScriptingAssemblyFileWatcher(Program.GlobalEventAggregator), 
                Logger.Instance);

        private readonly IProjectManager _projectManager;
        private readonly IEventAggregator _globalEventAggregator;
        private readonly ScriptExecutor _scriptExecutor;
        private readonly Logger _logger;
        private readonly ScriptingAssemblyFileWatcher _watcher;
        private Assembly _currentScriptingAssembly;

        public ScriptingService(
            IProjectManager projectManager,
            IEventAggregator globalEventAggregator,
            ScriptExecutor scriptExecutor,
            ScriptingAssemblyFileWatcher watcher,
            Logger logger)
        {
            _projectManager = projectManager;
            _globalEventAggregator = globalEventAggregator;
            _scriptExecutor = scriptExecutor;
            _watcher = watcher;
            _watcher.NewScriptingAssemblyAvailable += NewScriptingAssemblyAvailable;
            _logger = logger;
            _globalEventAggregator.Subscribe(this);
        }

        public Assembly LoadScriptingAssembly()
        {
            if (_projectManager.HasScriptingProject)
            {
                var project = _projectManager.Project;
                if (File.Exists(project.ScriptingAssemblyFilePath))
                {
                    _logger.Info("Loading Scripting assembly at {0}", project.ScriptingAssemblyFilePath);
                    try
                    {
                        var assemblyBytes = File.ReadAllBytes(project.ScriptingAssemblyFilePath);
                        var symbolBytes = File.ReadAllBytes(project.ScriptingSymbolsFilePath);
                        var assembly = Assembly.Load(assemblyBytes, symbolBytes);
                        SetCurrentScriptingAssembly(assembly);
                        _logger.Info("Loaded {0}", assembly.FullName);
                        return assembly;
                    }
                    catch (Exception e)
                    {
                        _logger.Error(e.Message);
                    }
                }
            }

            return null;
        }

        private void SetCurrentScriptingAssembly(Assembly assembly)
        {
            _currentScriptingAssembly = assembly;
            var scriptNames = _currentScriptingAssembly
                .GetTypes()
                .Where(t => t.GetMethods().Any(m => m.Name == "Main" && m.IsStatic && !m.GetParameters().Any()))
                .Select(t => t.FullName)
                .ToArray();
            _globalEventAggregator.Publish(new ScriptingAssemblyLoaded(scriptNames));
        }

        public ScriptExecutionResult ExecuteScript(string className)
        {
            if (_currentScriptingAssembly == null)
                throw new InvalidOperationException("There is no scripting assembly loaded.");

            return _scriptExecutor.ExecuteScript(_currentScriptingAssembly, className, "Main");
        }

        public ScriptExecutionResult ExecuteScriptForDebugging(string assemblyName, string className, string methodName)
        {
            _logger.Info("Execute Script in class {0} method {1} from {2}", className, methodName, assemblyName);

            var assembly = _currentScriptingAssembly;
            if (assembly.FullName != assemblyName)
                throw new Exception(
                    string.Format(
                        "The currently loaded scripting assembly is not the same version as expected to execute this script. Most likely we need to wait for the assembly to load before attempting to execute the script. Expected: '{0}' Loaded: '{1}'",
                        assemblyName, assembly.FullName));

            var declaringClass = assembly.GetType(className);
            if (declaringClass == null)
                throw new Exception(
                    string.Format("The class '{0}' is not available in the currently loaded scripting assembly: {1}",
                                  className, assembly.FullName));

            var method = declaringClass.GetMethod(methodName);
            if (method == null)
                throw new Exception(
                    string.Format("The method '{0}' does not exist on the class '{1}' in '{2}'.",
                                  methodName, className, assembly.FullName));

            if (method.IsStatic == false || method.GetParameters().Any())
                throw new Exception(
                    string.Format("The method '{0}' should be static and have no parameters.", method.Name));

            try
            {
                method.Invoke(null, null);
                return ScriptExecutionResult.Success(TimeSpan.FromSeconds(5));
            }
            catch (Exception e)
            {
                _logger.Error(e, "Failed to execute script");
                return ScriptExecutionResult.Failed(e.ToString(), TimeSpan.FromSeconds(5));
            }
        }

        private void NewScriptingAssemblyAvailable(object sender, NewScriptingAssemblyEventArgs e)
        {
            if (_projectManager.IsProjectLoaded &&
                e.ScriptingAssemblyPath == _projectManager.Project.ScriptingAssemblyFilePath)
            {
                LoadScriptingAssembly();
            }
        }

        public void Handle(ProjectLoaded message)
        {
            LoadScriptingAssembly();
        }

        public void Handle(ProjectUnloaded message)
        {
            _currentScriptingAssembly = null;
        }
    }
}