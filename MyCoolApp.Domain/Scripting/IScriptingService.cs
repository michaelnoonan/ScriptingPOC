using System.Threading.Tasks;
using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.Domain.Scripting
{
    public interface IScriptingService
    {
        ScriptExecutionResult ExecuteScriptForDebugging(string assemblyName, string className, string methodName);
        Task<ScriptExecutionResult> ExecuteScriptAsync(string className);
        Task DebugScriptAsync(string className);
        Task LoadScriptingProjectAsync();
    }
}