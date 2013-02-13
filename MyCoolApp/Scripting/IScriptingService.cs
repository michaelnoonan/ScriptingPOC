using System.Reflection;
using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.Scripting
{
    public interface IScriptingService
    {
        ScriptExecutionResult ExecuteScriptForDebugging(string assemblyName, string className, string methodName);
        ScriptExecutionResult ExecuteScript(string className);
        Assembly LoadScriptingAssembly();
    }
}