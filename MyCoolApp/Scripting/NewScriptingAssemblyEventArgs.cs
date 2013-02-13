namespace MyCoolApp.Scripting
{
    public class NewScriptingAssemblyEventArgs
    {
        public NewScriptingAssemblyEventArgs(string scriptingAssemblyPath)
        {
            ScriptingAssemblyPath = scriptingAssemblyPath;
        }

        public string ScriptingAssemblyPath { get; private set; }
    }
}