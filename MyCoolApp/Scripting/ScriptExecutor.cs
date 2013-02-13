using System;
using System.IO;
using System.Threading.Tasks;
using MyCoolApp.Events.Scripting;
using MyCoolApp.Model;
using MyCoolApp.Projects;
using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.Scripting
{
    public class ScriptExecutor
    {
        private readonly Logger _logger;
        private DateTime? _startedAt;
        private DateTime? _completedAt;

        public TimeSpan ElapsedTime
        {
            get { return (_startedAt.HasValue && _completedAt.HasValue) ? _completedAt.Value - _startedAt.Value : TimeSpan.Zero; }
        }

        public ScriptExecutor(Logger logger)
        {
            _logger = logger;
        }

        public Task<ScriptExecutionResult> ExecuteScriptAsync(string methodName)
        {
            return (Task<ScriptExecutionResult>) Task.Run(() => { throw new NotImplementedException(); });
        }

        public ScriptExecutionResult ExecuteScript(Project project, string assemblyPath, string scriptMethodPath)
        {
            _logger.Info("Execute Script in {0} at path {1}", assemblyPath, scriptMethodPath);

            _startedAt = DateTime.Now;
            try
            {
                var hostObject = new ScriptingHostModel(project);
                //var resultObject = _session.Execute(methodName);
                _completedAt = DateTime.Now;
                var scriptResult = ScriptExecutionResult.Success(ElapsedTime);
                Program.GlobalEventAggregator.Publish(new ScriptExecutionCompleted(scriptResult));
                return scriptResult;
            }
            catch (Exception e)
            {
                _completedAt = DateTime.Now;
                var scriptResult = ScriptExecutionResult.Failed(e.ToString(), ElapsedTime);
                Program.GlobalEventAggregator.Publish(new ScriptExecutionCompleted(scriptResult));
                return scriptResult;
            }
        }
    }
}