using System.Reflection;
using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.Scripting
{
    public interface IScriptingService
    {
        ScriptExecutionResult ExecuteScript(string assemblyPath, string className, string methodName);
        ScriptExecutionResult ExecuteScript(string className);
        ScriptLoadResult LoadScript(string assemblyPath, string className, string methodName);
        Assembly LoadMostRecentScriptingAssembly();
    }
}