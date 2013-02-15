using System;
using System.IO;
using System.Threading;
using Caliburn.Micro;
using MyCoolApp.Domain.Events;
using MyCoolApp.Domain.Events.Projects;

namespace MyCoolApp.Domain.Scripting
{
    public class ScriptingAssemblyFileWatcher :
        IHandle<ApplicationShuttingDown>,
        IHandle<ProjectLoaded>,
        IHandle<ProjectUnloaded>,
        IHandle<ScriptingProjectCreated>
    {
        private readonly IEventAggregator _globalEventAggregator;
        public event EventHandler<NewScriptingAssemblyEventArgs> NewScriptingAssemblyAvailable;
        private FileSystemWatcher _fileSystemWatcher;
        private Timer _fileLockTimer;
        private const int DefaultInterval = 500;

        public ScriptingAssemblyFileWatcher(IEventAggregator globalEventAggregator)
        {
            _globalEventAggregator = globalEventAggregator;
            globalEventAggregator.Subscribe(this);
        }

        public void Handle(ProjectLoaded message)
        {
            if (message.LoadedProject.HasScriptingProject)
            {
                StartWatchingScriptingAssembly(message.LoadedProject.ScriptingAssemblyFilePath);
            }
        }

        public void Handle(ScriptingProjectCreated message)
        {
            StartWatchingScriptingAssembly(message.Project.ScriptingAssemblyFilePath);
        }

        private void StartWatchingScriptingAssembly(string scriptingAssemblyFilePath)
        {
            var directory = Path.GetDirectoryName(scriptingAssemblyFilePath);
            Directory.CreateDirectory(directory);
            _fileSystemWatcher = new FileSystemWatcher(directory, "*.dll");
            _fileSystemWatcher.NotifyFilter = NotifyFilters.FileName;
            _fileSystemWatcher.Created += NewScriptingAssemblyFileCreated;
            _fileSystemWatcher.EnableRaisingEvents = true;
        }

        public void Handle(ProjectUnloaded message)
        {
            StopWatchingScriptingAssembly();
        }

        public void Handle(ApplicationShuttingDown message)
        {
            _globalEventAggregator.Unsubscribe(this);
            StopWatchingScriptingAssembly();
        }

        private void StopWatchingScriptingAssembly()
        {
            if (_fileLockTimer != null)
            {
                _fileLockTimer.Dispose();
                _fileLockTimer = null;
            }

            if (_fileSystemWatcher != null)
            {
                _fileSystemWatcher.Dispose();
                _fileSystemWatcher = null;
            }
        }

        private void NewScriptingAssemblyFileCreated(object sender, FileSystemEventArgs e)
        {
            // Stop any existing timer since we've got new files
            if (_fileLockTimer != null)
            {
                _fileLockTimer.Dispose();
                _fileLockTimer = null;
            }

            var assemblyPath = e.FullPath;
            var symbolsFilename = Path.GetFileNameWithoutExtension(assemblyPath) + ".pdb";
            var symbolsPath = Path.Combine(Path.GetDirectoryName(assemblyPath), symbolsFilename);

            // Enqueue a single tick
            _fileLockTimer = new Timer(
                CheckFilesAreUnlocked,
                new {AssemblyPath = assemblyPath, SymbolsPath = symbolsPath},
                DefaultInterval, -1);
        }

        private void CheckFilesAreUnlocked(object state)
        {
            dynamic filePaths = state;

            try
            {
                File.OpenRead(filePaths.AssemblyPath).Dispose();
                File.OpenRead(filePaths.SymbolsPath).Dispose();
                OnNewScriptingAssemblyAvailable(filePaths.AssemblyPath);
            }
            catch (IOException)
            {
                // Enqueue anoher tick
                _fileLockTimer.Change(DefaultInterval, -1);
            }
        }

        protected virtual void OnNewScriptingAssemblyAvailable(string scriptingAssemblyPath)
        {
            var handler = NewScriptingAssemblyAvailable;
            if (handler != null) handler(this, new NewScriptingAssemblyEventArgs(scriptingAssemblyPath));
        }
    }
}