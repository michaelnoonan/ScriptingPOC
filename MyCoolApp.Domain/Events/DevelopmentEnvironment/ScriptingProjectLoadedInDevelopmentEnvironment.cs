using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.Domain.Events.DevelopmentEnvironment
{
    public class ScriptingProjectLoadedInDevelopmentEnvironment
    {
        public ScriptingProjectLoadedInDevelopmentEnvironment(LoadScriptingProjectResult result)
        {
            Result = result;
        }

        public LoadScriptingProjectResult Result { get; private set; }
    }
}