using System.Threading.Tasks;
using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.Domain.Development
{
    public interface ISharpDevelopIntegrationService
    {
        Task<LoadScriptingProjectResult> LoadAndBuildScriptingProjectAsync();
        Task StartDebuggingScriptAsync(string className);
        string RemoteControlUri { get; }
        bool IsConnectionEstablished { get; }
        bool WaitingForSharpDevelopToStart { get; }
        bool IsSharpDevelopRunning { get; }
    }
}