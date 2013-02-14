using MyCoolApp.Domain.Projects;

namespace MyCoolApp.Domain.Events.Projects
{
    public class ScriptingProjectCreated
    {
        public ScriptingProjectCreated(Project project)
        {
            Project = project;
        }

        public Project Project { get; private set; }
    }
}