using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.Scripting
{
    public interface IScriptingService
    {
        ScriptResult ExecuteScript(string assemblyPath, string className, string methodName);
    }
}