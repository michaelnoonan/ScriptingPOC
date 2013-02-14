using System.Threading.Tasks;
using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.Scripting
{
    public interface IScriptingService
    {
        ScriptExecutionResult ExecuteScriptForDebugging(string assemblyName, string className, string methodName);
        Task<ScriptExecutionResult> ExecuteScriptAsync(string className);
    }
}