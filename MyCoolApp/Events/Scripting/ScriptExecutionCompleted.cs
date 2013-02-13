using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.Events.Scripting
{
    public class ScriptExecutionCompleted
    {
        public ScriptExecutionCompleted(ScriptExecutionResult result)
        {
            Result = result;
        }

        public ScriptExecutionResult Result { get; private set; } 
    }
}