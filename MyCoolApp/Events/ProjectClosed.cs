using MyCoolApp.Projects;

namespace MyCoolApp.Events
{
    public class ProjectClosed
    {
        public Project ClosedProject { get; private set; }

        public ProjectClosed(Project closedProject)
        {
            ClosedProject = closedProject;
        }
    }
}