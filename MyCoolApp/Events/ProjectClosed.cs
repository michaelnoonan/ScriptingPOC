namespace MyCoolApp.Events
{
    public class ProjectClosed
    {
        public string ProjectFilePath { get; private set; }

        public ProjectClosed(string projectFilePath)
        {
            ProjectFilePath = projectFilePath;
        }
    }
}