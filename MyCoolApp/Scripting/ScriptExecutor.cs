using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MyCoolApp.Events.Scripting;
using MyCoolApp.Model;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;
using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.Scripting
{
    public class ScriptExecutor
    {
        private readonly ScriptingHostModel _hostObject;
        private readonly ScriptEngine _engine;
        private readonly Session _session;
        private DateTime? _startedAt;
        private DateTime? _completedAt;

        public TimeSpan ElapsedTime
        {
            get { return (_startedAt.HasValue && _completedAt.HasValue) ? _completedAt.Value - _startedAt.Value : TimeSpan.Zero; }
        }

        public ScriptExecutor(Project project)
        {
            _hostObject = new ScriptingHostModel(project);
            _engine = new ScriptEngine();
            _session = _engine.CreateSession(_hostObject);
            AddReferencesAndNamespaces(_session,
                                       new[]
                                           {
                                               typeof (ScriptingHostModel),
                                               typeof (IEnumerable<>),
                                               typeof (IQueryable),
                                               typeof (File)
                                           });
        }

        private void AddReferencesAndNamespaces(Session session, IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                session.AddReference(type.Assembly);
                session.ImportNamespace(type.Namespace);
            }
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
                var resultObject = _session.Execute(scriptText);
                _completedAt = DateTime.Now;
                var resultString = (resultObject ?? "Execution completed successfully without a return value.").ToString();
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