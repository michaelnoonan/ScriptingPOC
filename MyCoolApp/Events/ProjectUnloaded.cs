using MyCoolApp.Projects;

namespace MyCoolApp.Events
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