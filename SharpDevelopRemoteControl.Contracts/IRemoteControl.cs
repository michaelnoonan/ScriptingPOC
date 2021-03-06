﻿using System.ServiceModel;

namespace SharpDevelopRemoteControl.Contracts
{
    [ServiceContract]
    public interface IRemoteControl
    {
        [OperationContract]
        void LogMessage(string message);

        [OperationContract]
        void LoadScriptingProject(string projectFilePath);

        [OperationContract(IsOneWay = true)]
        void ShutDown();

        [OperationContract]
        void StartDebuggingScript(string className);
    }
}