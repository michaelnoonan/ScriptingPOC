using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MyCoolApp.Diagnostics;
using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.Scripting
{
    public class ScriptExecutor : IScriptExecutor
    {
        private readonly ILogger _logger;
        private bool _currentlyExecuting;

        public ScriptExecutor(ILogger logger)
        {
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
    }
}