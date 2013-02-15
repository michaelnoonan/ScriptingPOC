using System.Threading.Tasks;
using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.Domain.Scripting
{
    public interface IScriptingService
    {
        Task<ScriptExecutionResult> ExecuteScriptForDebuggingAsync(string assemblyName, string className, string methodName);
        Task<ScriptExecutionResult> ExecuteScriptAsync(string className);
        Task DebugScriptAsync(string className);
        Task LoadScriptingProjectAsync();
    }
}