using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using MyCoolApp.Development;
using MyCoolApp.Diagnostics;
using MyCoolApp.Projects;
using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.Scripting
{
    public class ScriptingService : IScriptingService
    {
        public static ScriptingService Instance =
            new ScriptingService(
                ProjectManager.Instance,
                SharpDevelopIntegrationService.Instance,
                new ScriptingAssemblyLoader(ProjectManager.Instance, new ScriptingAssemblyFileWatcher(Program.GlobalEventAggregator), Program.GlobalEventAggregator, Logger.Instance),
                new ScriptExecutor(Logger.Instance), 
                Program.GlobalEventAggregator,
                Logger.Instance);

        private readonly IProjectManager _projectManager;
        private readonly ISharpDevelopIntegrationService _sharpDevelopIntegrationService;
        private readonly IScriptingAssemblyLoader _scriptingAssemblyLoader;
        private readonly IEventAggregator _globalEventAggregator;
        private readonly IScriptExecutor _scriptExecutor;
        private readonly ILogger _logger;

        public ScriptingService(
            IProjectManager projectManager,
            ISharpDevelopIntegrationService sharpDevelopIntegrationService,
            IScriptingAssemblyLoader scriptingAssemblyLoader,
            IScriptExecutor scriptExecutor,
            IEventAggregator globalEventAggregator,
            ILogger logger)
        {
            _projectManager = projectManager;
            _sharpDevelopIntegrationService = sharpDevelopIntegrationService;
            _scriptingAssemblyLoader = scriptingAssemblyLoader;
            _globalEventAggregator = globalEventAggregator;
            _scriptExecutor = scriptExecutor;
            _logger = logger;
            _globalEventAggregator.Subscribe(this);
        }

        public void LoadScriptingProject()
        {
            _sharpDevelopIntegrationService.LoadScriptingProject(_projectManager.Project);
        }

        public async Task<ScriptExecutionResult> ExecuteScriptAsync(string className)
        {
            if (_scriptingAssemblyLoader.CurrentScriptingAssembly == null)
                throw new InvalidOperationException("There is no scripting assembly loaded.");

            var result = await _scriptExecutor.ExecuteScriptAsync(_scriptingAssemblyLoader.CurrentScriptingAssembly, className, "Main");
            return result;
        }

        public ScriptExecutionResult ExecuteScriptForDebugging(string assemblyName, string className, string methodName)
        {
            _logger.Info("Execute Script in class {0} method {1} from {2}", className, methodName, assemblyName);
            
            // Wait for the assembly to be loaded completely before continuing with executing the script
            var assemblyTask = _scriptingAssemblyLoader.GetAssemblyWithWait(assemblyName, TimeSpan.FromSeconds(10));
            assemblyTask.Wait(TimeSpan.FromSeconds(15));
            if (assemblyTask.Status == TaskStatus.RanToCompletion)
            {
                return _scriptExecutor.ExecuteScript(assemblyTask.Result, className, methodName);
            }
            throw new Exception("Failed to execute the script. Inner exceptions may provide more information.", assemblyTask.Exception);
        }
    }
}