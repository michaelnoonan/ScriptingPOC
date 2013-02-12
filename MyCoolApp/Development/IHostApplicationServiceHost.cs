using System;

namespace MyCoolApp.Development
{
    public interface IHostApplicationServiceHost : IDisposable
    {
        void StartListening();
        string ListenUri { get; }
    }
}