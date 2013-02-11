using System;
using System.IO;
using System.Threading.Tasks;
using MyCoolApp.Events.Scripting;
using MyCoolApp.Model;
using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.Scripting
{
    public class ScriptExecutor
    {
        private readonly ScriptingHostModel _hostObject;
        private DateTime? _startedAt;
        private DateTime? _completedAt;

        public TimeSpan ElapsedTime
        {
            get { return (_startedAt.HasValue && _completedAt.HasValue) ? _completedAt.Value - _startedAt.Value : TimeSpan.Zero; }
        }

        public ScriptExecutor(Project project)
        {
            _hostObject = new ScriptingHostModel(project);
        }

        public Task<ScriptResult> ExecuteScriptAsync(string scriptText)
        {
            return Task.Run(() => ExecuteScript(scriptText));
        }

        public ScriptResult ExecuteScript(string scriptText)
        {
            _startedAt = DateTime.Now;
            try
            {
                //var resultObject = _session.Execute(scriptText);
                _completedAt = DateTime.Now;
                var resultString = "Execution completed successfully without a return value.";
                var scriptResult = new ScriptResult(successful: true, result: resultString, elapsedTime: ElapsedTime);
                Program.GlobalEventAggregator.Publish(new ScriptExecutionCompleted(scriptResult));
                return scriptResult;
            }
            catch (Exception e)
            {
                _completedAt = DateTime.Now;
                var scriptResult = new ScriptResult(successful: false, result: e.Message, elapsedTime: ElapsedTime);
                Program.GlobalEventAggregator.Publish(new ScriptExecutionCompleted(scriptResult));
                return scriptResult;
            }
        }

        public ScriptResult ExecuteScriptFile(string scriptFilePath)
        {
            if (string.IsNullOrWhiteSpace(scriptFilePath))
                throw new ArgumentException("The file path was not specified.");

            if (File.Exists(scriptFilePath) == false)
                throw new InvalidOperationException("The file does not exist at that path.");

            return ExecuteScript(File.ReadAllText(scriptFilePath));
        }
    }
}