namespace MyCoolApp.Events.Scripting
{
    public class ScriptExecutionStarted
    {
        public ScriptExecutionStarted(string scriptAssemblyPath, string methodPath)
        {
            ScriptAssemblyPath = scriptAssemblyPath;
            ScriptMethodPath = methodPath;
        }

        public string ScriptAssemblyPath { get; private set; }
        public string ScriptMethodPath { get; private set; }
    }
}