using System.Threading.Tasks;
using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.Development
{
    public interface ISharpDevelopIntegrationService
    {
        Task<LoadScriptingProjectResult> LoadAndBuildScriptingProjectAsync();
        Task StartDebuggingScriptAsync(string className);
    }
}