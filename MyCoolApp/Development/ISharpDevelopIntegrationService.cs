using System;
using MyCoolApp.Projects;
using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.Development
{
    public interface ISharpDevelopIntegrationService
    {
        void LoadScriptingProject(Project project, Action<IRemoteControl> whenProjectHasLoaded = null);
    }
}