using System;
using System.IO;
using System.Linq;
using System.Reflection;
using MyCoolApp.Projects;
using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.Scripting
{
    public class ScriptingService : IScriptingService
    {
        public static ScriptingService Instance =
            new ScriptingService(
                () => new ScriptExecutor(Logger.Instance),
                ProjectManager.Instance,
                Logger.Instance);

        private readonly Func<ScriptExecutor> _scriptExecutorFactory;
        private readonly IProjectManager _projectManager;
        private readonly Logger _logger;

        public ScriptingService(Func<ScriptExecutor> scriptExecutorFactory, IProjectManager projectManager, Logger logger)
        {
            _scriptExecutorFactory = scriptExecutorFactory;
            _projectManager = projectManager;
            _logger = logger;
        }

        public ScriptLoadResult LoadScript(string assemblyPath, string className, string methodName)
        {
            _logger.Info("Loading Script from assembly at {0} in class {1} method {2}", assemblyPath, className, methodName);

            if (File.Exists(assemblyPath) == false)
            {
                var result = ScriptLoadResult.Failed("Assembly was not found at {0}", assemblyPath);
                _logger.Error(result.FailureReason);
                return result;
            }

            Assembly assembly;

            try
            {
                assembly = Assembly.LoadFrom(assemblyPath);
            }
            catch (Exception e)
            {
                var result = ScriptLoadResult.Failed("Could not load the assembly because {0}", e.Message);
                _logger.Error(e, result.FailureReason);
                return result;
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
                return new ScriptExecutionResult(true, "Great!", TimeSpan.FromSeconds(5));
            }
            catch (Exception e)
            {
                _logger.Error(e, "Failed to execute script");
                return new ScriptExecutionResult(false, "Not great!", TimeSpan.FromSeconds(5));
            }
        }
    }
}