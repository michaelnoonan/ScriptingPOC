using MyCoolApp.Model;

namespace MyCoolApp.Scripting
{
    /// <summary>
    /// This is the class that will be exposed by default to scripts run in the ScriptingEngine.
    /// </summary>
    public class ScriptingHostModel
    {
        public ScriptingHostModel(Project project)
        {
            Project = project;
        }

        public Project Project { get; private set; }
    }
}