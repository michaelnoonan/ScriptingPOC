using System.ServiceModel;
using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.DevelopmentEnvironment
{
    public class RemoteControlClient
    {
        public static readonly RemoteControlClient Instance = new RemoteControlClient();

        private readonly ChannelFactory<IRemoteControlService> _channelFactory =
            new ChannelFactory<IRemoteControlService>(new NetNamedPipeBinding());
        

    }
}