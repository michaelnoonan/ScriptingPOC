using System;
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

        public ScriptResult ExecuteScript(string assemblyPath, string className, string methodName)
        {
            _logger.Info("Execute Script from assembly at {0} in class {1} method {2}", assemblyPath, className, methodName);

            var assembly = Assembly.LoadFrom(assemblyPath);

            var declaringClass = assembly.GetType(className);
            var method = declaringClass.GetMethod(methodName);
            if (method.IsStatic == false)
            {
                var instance = Activator.CreateInstance(declaringClass);
                method.Invoke(instance, null);
            }
            else
            {
                method.Invoke(null, null);
            }

            return new ScriptResult(true, "Great!", TimeSpan.FromSeconds(5));
        }
    }
}