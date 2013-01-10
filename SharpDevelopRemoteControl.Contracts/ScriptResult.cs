using System;
using System.Runtime.Serialization;

namespace SharpDevelopRemoteControl.Contracts
{
    [DataContract]
    public class ScriptResult
    {
        public ScriptResult(bool successful, string result, TimeSpan elapsedTime)
        {
            Successful = successful;
            ResultDescription = result;
            ElapsedTime = elapsedTime;
        }

        [DataMember]
        public bool Successful { get; private set; }
        [DataMember]
        public string ResultDescription { get; private set; }
        [DataMember]
        public TimeSpan ElapsedTime { get; private set; }
    }
}