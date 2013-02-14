using System.Threading.Tasks;
using MyCoolApp.Projects;
using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.Development
{
    public interface ISharpDevelopIntegrationService
    {
        Task<LoadScriptingProjectResult> LoadScriptingProjectAsync(Project project);
    }
}