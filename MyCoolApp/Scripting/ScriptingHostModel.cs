using System.Collections.Generic;
using MyCoolApp.Model;

namespace MyCoolApp.Scripting
{
    /// <summary>
    /// This is the class that will be exposed by default to scripts run in the ScriptingEngine.
    /// </summary>
    public class ScriptingHostModel
    {
        private readonly Project _project;

        public ScriptingHostModel(Project project)
        {
            _project = project;
        }

        public IEnumerable<RecordedAction> RecordedActions { get { return _project.RecordedActions; } }

        public void AddRecordedAction(string description)
        {
            _project.RecordedActions.Add(new RecordedAction(description));
        }
    }
}