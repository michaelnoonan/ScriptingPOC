using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MyCoolApp.Model;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;

namespace MyCoolApp.Scripting
{
    public class ScriptExecutor
    {
        private readonly ScriptingHostModel _hostObject;
        private readonly ScriptEngine _engine;
        private readonly Session _session;

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

        public Task<string> ExecuteScriptAsync(string scriptText)
        {
            return Task.Run(() => ExecuteScript(scriptText));
        }

        public string ExecuteScript(string scriptText)
        {
            var result = _session.Execute(scriptText);
            return (result ?? "Execution completed successfully without a return value.").ToString();
        }
    }
}