using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.Domain.Scripting
{
    public interface IScriptExecutor
    {
        Task<ScriptExecutionResult> ExecuteScriptAsync(Assembly assembly, string className, string methodName, CancellationToken cancellation = new CancellationToken());
    }
}