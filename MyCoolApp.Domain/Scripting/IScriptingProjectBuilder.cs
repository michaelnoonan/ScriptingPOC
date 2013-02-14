namespace MyCoolApp.Domain.Scripting
{
    public interface IScriptingProjectBuilder
    {
        void BuildScriptingProject(string scriptingProjectName, string scriptingProjectFilePath);
    }
}