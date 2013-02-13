using System.Runtime.Serialization;

namespace SharpDevelopRemoteControl.Contracts
{
    [DataContract]
    public class ScriptLoadResult
    {
        internal ScriptLoadResult()
        {
        }

        protected ScriptLoadResult(bool successful, string failureReason = null)
        {
            Successful = successful;
            FailureReason = failureReason;
        }

        public static ScriptLoadResult Success()
        {
            return new ScriptLoadResult(true);
        }

        public static ScriptLoadResult Failed(string reasonFormat, params object[] args)
        {
            return new ScriptLoadResult(false, string.Format(reasonFormat, args));
        }

        [DataMember]
        public bool Successful { get; private set; }
        [DataMember]
        public string FailureReason { get; set; }
    }
}