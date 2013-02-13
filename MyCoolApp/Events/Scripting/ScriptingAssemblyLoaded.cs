namespace MyCoolApp.Events.Scripting
{
    public class ScriptingAssemblyLoaded
    {
        public ScriptingAssemblyLoaded(string[] scriptNames)
        {
            ScriptNames = scriptNames;
        }

        public string[] ScriptNames { get; private set; }
    }
}