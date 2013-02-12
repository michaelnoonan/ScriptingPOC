using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.Events.Scripting
{
    public class ScriptExecutionCompleted
    {
        public ScriptExecutionCompleted(ScriptResult result)
        {
            Result = result;
        }

        public ScriptResult Result { get; private set; } 
    }
}