using MyCoolApp.Domain.Projects;

namespace MyCoolApp.Domain.Events.Projects
{
    public class ProjectUnloaded
    {
        public Project UnloadedProject { get; private set; }

        public ProjectUnloaded(Project unloadedProject)
        {
            UnloadedProject = unloadedProject;
        }
    }
}