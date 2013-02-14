namespace MyCoolApp.Events.Scripting
{
    public class ScriptingAssemblyLoaded
    {
        public ScriptingAssemblyLoaded(string assemblyName, string[] scriptNames)
        {
            AssemblyName = assemblyName;
            ScriptNames = scriptNames;
        }

        public string AssemblyName { get; private set; }
        public string[] ScriptNames { get; private set; }
    }
}