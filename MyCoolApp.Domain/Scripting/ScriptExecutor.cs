using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MyCoolApp.Domain.Diagnostics;
using MyCoolApp.Domain.Projects;
using MyCoolApp.Domain.Scripting.Adapters;
using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.Domain.Scripting
{
    public class ScriptExecutor : IScriptExecutor
    {
        private readonly IProjectManager _projectManager;
        private readonly ILogger _logger;
        private bool _currentlyExecuting;

        public ScriptExecutor(IProjectManager projectManager, ILogger logger)
        {
            _projectManager = projectManager;
            _logger = logger;
        }

        private void AssertSingleScript()
        {
            if (_currentlyExecuting)
                throw new InvalidOperationException("A script is currently executing - only one script can execute at a time.");
        }

        public Task<ScriptExecutionResult> ExecuteScriptAsync(Assembly assembly, string className, string methodName)
        {
            return Task.Run(() => ExecuteScript(assembly, className, methodName));
        }

        public ScriptExecutionResult ExecuteScript(Assembly assembly, string className, string methodName)
        {
            AssertSingleScript();
            _currentlyExecuting = true;

            var declaringClass = assembly.GetType(className);
            if (declaringClass == null)
                throw new Exception(
                    string.Format("The class '{0}' is not available in the scripting assembly: {1}",
                                  className, assembly.FullName));

            var method = declaringClass.GetMethod(methodName);
            if (method == null)
                throw new Exception(
                    string.Format("The method '{0}' does not exist on the class '{1}' in '{2}'.",
                                  methodName, className, assembly.FullName));

            if (method.IsStatic == false || method.GetParameters().Any())
                throw new Exception(
                    string.Format("The method '{0}' should be static and have no parameters.", method.Name));

            InjectProperties(declaringClass);

            var startedAt = DateTime.MinValue;
            var completedAt = DateTime.MinValue;
            try
            {
                startedAt = DateTime.Now;
                method.Invoke(null, null);
                completedAt = DateTime.Now;
                _currentlyExecuting = false;
                return ScriptExecutionResult.Success(completedAt - startedAt);
            }
            catch (Exception e)
            {
                completedAt = DateTime.Now;
                _currentlyExecuting = false;
                _logger.Error(e, "The script failed with an exception.");
                return ScriptExecutionResult.Failed(e.Message, completedAt - startedAt);
            }
        }

        private void InjectProperties(Type declaringClass)
        {
            Inject<ILogger>(declaringClass, _logger);
            Inject<ISchedule>(declaringClass, new ScheduleAdapter(_projectManager.Project.Schedule));
        }

        private void Inject<T>(Type declaringClass, object instance)
        {
            var properties = declaringClass.GetProperties();

            foreach (PropertyInfo p in properties)
            {
                if (p.PropertyType != typeof (T))
                {
                    continue;
                }

                // If not writable then cannot null it; if not readable then cannot check it's value
                if (!p.CanWrite || !p.CanRead)
                {
                    continue;
                }

                // Check the accessibility of the Set method
                var mset = p.GetSetMethod(nonPublic: false);
                if (mset == null)
                {
                    continue;
                }
                p.SetValue(null, instance);
            }
        }
    }
}