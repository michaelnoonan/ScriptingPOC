using System.Reflection;
using System.Threading.Tasks;
using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.Scripting
{
    public interface IScriptExecutor
    {
        Task<ScriptExecutionResult> ExecuteScriptAsync(Assembly assembly, string className, string methodName);
        ScriptExecutionResult ExecuteScript(Assembly assembly, string className, string methodName);
    }
}