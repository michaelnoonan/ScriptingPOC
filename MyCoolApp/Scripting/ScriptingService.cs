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
        IHandle<ProjectLoaded>
    {
        public static ScriptingService Instance =
            new ScriptingService(
                () => new ScriptExecutor(Logger.Instance),
                ProjectManager.Instance,
                Program.GlobalEventAggregator,
                Logger.Instance);

        private readonly Func<ScriptExecutor> _scriptExecutorFactory;
        private readonly IProjectManager _projectManager;
        private readonly IEventAggregator _globalEventAggregator;
        private readonly Logger _logger;
        private Assembly _currentScriptingAssembly;
        private readonly object _scriptingAssemblyLock = new object();

        public ScriptingService(
            Func<ScriptExecutor> scriptExecutorFactory,
            IProjectManager projectManager,
            IEventAggregator globalEventAggregator,
            Logger logger)
        {
            _scriptExecutorFactory = scriptExecutorFactory;
            _projectManager = projectManager;
            _globalEventAggregator = globalEventAggregator;
            _logger = logger;

            _globalEventAggregator.Subscribe(this);
        }

        public Assembly LoadMostRecentScriptingAssembly()
        {
            if (_projectManager.HasScriptingProject)
            {
                var project = _projectManager.Project;
                var matchingFiles = Directory.EnumerateFiles(project.ScriptingFolder, project.ScriptingAssemblyFilename, SearchOption.AllDirectories);
                var mostRecentScriptingAssemblyPath = matchingFiles.OrderByDescending(File.GetCreationTime).FirstOrDefault();
                if (mostRecentScriptingAssemblyPath != null)
                {
                    _logger.Info("Loading Scripting assembly at {0}", mostRecentScriptingAssemblyPath);
                    try
                    {
                        var assembly = Assembly.LoadFrom(mostRecentScriptingAssemblyPath);
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
            lock (_scriptingAssemblyLock)
            {
                _currentScriptingAssembly = assembly;
                var scriptNames = _currentScriptingAssembly
                    .GetTypes()
                    .Where(t => t.GetMethods().Any(m => m.Name == "Main" && m.IsStatic && !m.GetParameters().Any()))
                    .Select(t => t.FullName)
                    .ToArray();
                _globalEventAggregator.Publish(new ScriptingAssemblyLoaded(scriptNames));
            }
        }

        public ScriptExecutionResult ExecuteScript(string className)
        {
            if (_currentScriptingAssembly == null)
                throw new InvalidOperationException("There is no scripting assembly loaded.");

            lock (_scriptingAssemblyLock)
            {
                var method = _currentScriptingAssembly.GetType(className).GetMethod("Main");
                method.Invoke(null, null);
                return ScriptExecutionResult.Success(TimeSpan.FromSeconds(5));
            }
        }

        public ScriptLoadResult LoadScript(string assemblyPath, string className, string methodName)
        {
            _logger.Info("Loading Script from assembly at {0} in class {1} method {2}", assemblyPath, className, methodName);

            Assembly assembly;
            try
            {
                assembly = Assembly.LoadFrom(assemblyPath);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                return ScriptLoadResult.Failed(e.Message);
            }

            _logger.Info("Loaded {0}", assembly.FullName);

            var declaringClass = assembly.GetType(className);
            if (declaringClass == null)
            {
                var result = ScriptLoadResult.Failed("There is no class called '{0}' in the assembly '{1}'", className, assembly.FullName);
                _logger.Error(result.FailureReason);
                return result;
            }

            var method = declaringClass.GetMethod(methodName);
            if (method == null)
            {
                var result =
                    ScriptLoadResult.Failed("There is no method called '{0}' on the class '{1}' in the assembly '{2}'",
                                            methodName, className, assembly.FullName);
                _logger.Error(result.FailureReason);
                return result;
            }

            if (method.IsStatic == false || method.GetParameters().Any())
            {
                var result =
                    ScriptLoadResult.Failed(
                        "The method '{0}' on class '{1}' should be Static/Shared and have no parameters.",
                        methodName, className);
                _logger.Error(result.FailureReason);
                return result;
            }

            return ScriptLoadResult.Success();
        }

        public ScriptExecutionResult ExecuteScript(string assemblyPath, string className, string methodName)
        {
            _logger.Info("Execute Script from assembly at {0} in class {1} method {2}", assemblyPath, className, methodName);

            var assembly = Assembly.LoadFrom(assemblyPath);

            var declaringClass = assembly.GetType(className);
            var method = declaringClass.GetMethod(methodName);

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

        public void Handle(ProjectLoaded message)
        {
            LoadMostRecentScriptingAssembly();
        }
    }
}