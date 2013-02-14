using System;
using System.Reflection;
using System.Threading.Tasks;

namespace MyCoolApp.Domain.Scripting
{
    public interface IScriptingAssemblyLoader
    {
        Task<Assembly> GetAssemblyWithWait(string assemblyName, TimeSpan timeToWaitForAssembly);
        Assembly CurrentScriptingAssembly { get; }
    }
}