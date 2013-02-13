using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.Scripting
{
    public interface IScriptingService
    {
        ScriptExecutionResult ExecuteScript(string assemblyPath, string className, string methodName);
        ScriptLoadResult LoadScript(string assemblyPath, string className, string methodName);
    }
}