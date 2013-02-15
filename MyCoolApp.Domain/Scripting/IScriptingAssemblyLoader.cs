using System;
using System.Reflection;
using System.Threading.Tasks;

namespace MyCoolApp.Domain.Scripting
{
    public interface IScriptingAssemblyLoader
    {
        Task<Assembly> GetAssemblyAsync(string assemblyName, TimeSpan timeToWaitForAssembly);
        Assembly CurrentScriptingAssembly { get; }
    }
}