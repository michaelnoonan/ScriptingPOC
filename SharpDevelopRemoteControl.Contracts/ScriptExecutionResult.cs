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

        protected ScriptExecutionResult(bool isSuccessful, bool isCancelled, string failureReason, TimeSpan elapsedTime)
        {
            IsSuccessful = isSuccessful;
            IsCancelled = isCancelled;
            FailureReason = failureReason;
            ElapsedTime = elapsedTime;
        }

        public static ScriptExecutionResult Success(TimeSpan elapsedTime)
        {
            return new ScriptExecutionResult(true, false, null, elapsedTime);
        }

        public static ScriptExecutionResult Cancelled(TimeSpan elapsedTime)
        {
            return new ScriptExecutionResult(false, true, null, elapsedTime);
        }

        public static ScriptExecutionResult Failed(string failureReason, TimeSpan elapsedTime)
        {
            return new ScriptExecutionResult(false, false, failureReason, elapsedTime);
        }

        [DataMember]
        public bool IsSuccessful { get; private set; }
        [DataMember]
        public bool IsCancelled { get; private set; }
        [DataMember]
        public string FailureReason { get; private set; }
        [DataMember]
        public TimeSpan ElapsedTime { get; private set; }
    }
}