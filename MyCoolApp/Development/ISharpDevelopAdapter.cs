using MyCoolApp.Projects;

namespace MyCoolApp.Development
{
    public interface ISharpDevelopAdapter
    {
        string RemoteControlUri { get; }
        bool IsConnectionEstablished { get; }
        void StartDevelopmentEnvironment(string projectOrSolutionFilePath = null);
        void LoadScriptingProject(Project project);
        void ShutDownDevelopmentEnvironment();
    }
}