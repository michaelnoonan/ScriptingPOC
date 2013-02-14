using System.Runtime.Serialization;

namespace SharpDevelopRemoteControl.Contracts
{
    [DataContract]
    public class LoadScriptingProjectResult
    {
        protected LoadScriptingProjectResult()
        {
            
        }

        public LoadScriptingProjectResult(string scriptingProjectFilePath, bool isProjectBuildingSuccessfully)
        {
            ScriptingProjectFilePath = scriptingProjectFilePath;
            IsProjectBuildingSuccessfully = isProjectBuildingSuccessfully;
        }

        [DataMember]
        public string ScriptingProjectFilePath { get; protected set; }

        [DataMember]
        public bool IsProjectBuildingSuccessfully { get; protected set; }
    }
}