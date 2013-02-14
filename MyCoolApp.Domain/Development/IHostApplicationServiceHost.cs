using System;

namespace MyCoolApp.Domain.Development
{
    public interface IHostApplicationServiceHost : IDisposable
    {
        void StartListening();
        string ListenUri { get; }
    }
}