using System.IO;
using MyCoolApp.DevelopmentEnvironment;

namespace MyCoolApp
{
    public class ProjectManager
    {
        public static readonly ProjectManager Instance = new ProjectManager();

        public string ProjectFileFullPath { get; private set; }
        public string ProjectScriptingSolutionFilePath { get; private set; }

        public void LoadProject(string projectFilePath)
        {
            ProjectFileFullPath = projectFilePath;
            ProjectScriptingSolutionFilePath = Path.Combine(Path.GetDirectoryName(projectFilePath), "Scripting\\MyProject1.sln");
        }

        public void LoadScriptingProject()
        {
            DevelopmentEnvironmentHub.Instance.LoadProject(ProjectScriptingSolutionFilePath);
            
        }
    }
}