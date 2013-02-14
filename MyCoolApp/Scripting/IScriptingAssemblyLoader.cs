﻿using System;
using System.Reflection;
using System.Threading.Tasks;

namespace MyCoolApp.Scripting
{
    public interface IScriptingAssemblyLoader
    {
        Task<Assembly> GetAssemblyWithWait(string assemblyName, TimeSpan timeToWaitForAssembly);
        Assembly CurrentScriptingAssembly { get; }
    }
}