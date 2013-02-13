namespace MyCoolApp.Projects
{
    public interface IProjectManager
    {
        Project Project { get; }
        bool IsProjectLoaded { get; }
        bool HasScriptingProject { get; }
        void CreateNewProject(string projectPath);
        void LoadProject(string projectFilePath);
        void SaveProject();
        void UnloadProject();
    }
}