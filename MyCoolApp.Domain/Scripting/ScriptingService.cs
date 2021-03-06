﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using MyCoolApp.Domain.Development;
using MyCoolApp.Domain.Diagnostics;
using MyCoolApp.Domain.Projects;
using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.Domain.Scripting
{
    public class ScriptingService : IScriptingService
    {
        public static ScriptingService Instance =
            new ScriptingService(
                SharpDevelopIntegrationService.Instance,
                new ScriptingAssemblyLoader(ProjectManager.Instance, new ScriptingAssemblyFileWatcher(GlobalEventAggregator.Instance), GlobalEventAggregator.Instance, Logger.Instance),
                new ScriptExecutor(ProjectManager.Instance, Logger.Instance), 
                GlobalEventAggregator.Instance,
                Logger.Instance);

        private readonly ISharpDevelopIntegrationService _sharpDevelopIntegrationService;
        private readonly IScriptingAssemblyLoader _scriptingAssemblyLoader;
        private readonly IEventAggregator _globalEventAggregator;
        private readonly IScriptExecutor _scriptExecutor;
        private readonly ILogger _logger;

        public ScriptingService(
            ISharpDevelopIntegrationService sharpDevelopIntegrationService,
            IScriptingAssemblyLoader scriptingAssemblyLoader,
            IScriptExecutor scriptExecutor,
            IEventAggregator globalEventAggregator,
            ILogger logger)
        {
            _sharpDevelopIntegrationService = sharpDevelopIntegrationService;
            _scriptingAssemblyLoader = scriptingAssemblyLoader;
            _globalEventAggregator = globalEventAggregator;
            _scriptExecutor = scriptExecutor;
            _logger = logger;
            _globalEventAggregator.Subscribe(this);
        }

        public async Task LoadScriptingProjectAsync()
        {
            var result = await _sharpDevelopIntegrationService.LoadAndBuildScriptingProjectAsync();
        }

        public async Task<ScriptExecutionResult> ExecuteScriptAsync(string className, CancellationToken cancellation)
        {
            if (_scriptingAssemblyLoader.CurrentScriptingAssembly == null)
                throw new InvalidOperationException("There is no scripting assembly loaded.");
            var result = await _scriptExecutor.ExecuteScriptAsync(_scriptingAssemblyLoader.CurrentScriptingAssembly, className, "Main", cancellation);
            return result;
        }

        public async Task DebugScriptAsync(string className)
        {
            await _sharpDevelopIntegrationService.StartDebuggingScriptAsync(className);
        }

        public async Task<ScriptExecutionResult> ExecuteScriptForDebuggingAsync(string assemblyName, string className, string methodName)
        {
            _logger.Info("Execute Script in class {0} method {1} from {2}", className, methodName, assemblyName);
            
            // Wait for the assembly to be loaded completely before continuing with executing the script
            var assembly = await _scriptingAssemblyLoader.GetAssemblyAsync(assemblyName, TimeSpan.FromSeconds(10));
            return await _scriptExecutor.ExecuteScriptAsync(assembly, className, methodName);
        }
    }
}