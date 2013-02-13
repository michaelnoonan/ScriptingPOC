using System;
using System.Runtime.Serialization;

namespace SharpDevelopRemoteControl.Contracts
{
    [DataContract]
    public class ScriptExecutionResult
    {
        protected ScriptExecutionResult()
        {
            
        }

        protected ScriptExecutionResult(bool successful, string failureReason, TimeSpan elapsedTime)
        {
            Successful = successful;
            FailureReason = failureReason;
            ElapsedTime = elapsedTime;
        }

        public static ScriptExecutionResult Success(TimeSpan elapsedTime)
        {
            return new ScriptExecutionResult(true, null, elapsedTime);
        }

        public static ScriptExecutionResult Failed(string failureReason, TimeSpan elapsedTime)
        {
            return new ScriptExecutionResult(false, failureReason, elapsedTime);
        }

        [DataMember]
        public bool Successful { get; private set; }
        [DataMember]
        public string FailureReason { get; private set; }
        [DataMember]
        public TimeSpan ElapsedTime { get; private set; }
    }
}