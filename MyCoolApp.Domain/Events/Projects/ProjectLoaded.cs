using MyCoolApp.Domain.Projects;

namespace MyCoolApp.Domain.Events.Projects
{
    public class ProjectLoaded
    {
        public Project LoadedProject { get; private set; }

        public ProjectLoaded(Project loadedProject)
        {
            LoadedProject = loadedProject;
        }
    }
}