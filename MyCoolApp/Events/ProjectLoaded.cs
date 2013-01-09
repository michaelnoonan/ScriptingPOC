namespace MyCoolApp.Events
{
    public class ProjectLoaded
    {
        public string ProjectFileFullPath { get; private set; }

        public ProjectLoaded(string projectFileFullPath)
        {
            ProjectFileFullPath = projectFileFullPath;
        }
    }
}