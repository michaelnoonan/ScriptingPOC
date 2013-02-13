using System;
using System.IO;
using Caliburn.Micro;
using MyCoolApp.Events;

namespace MyCoolApp.Scripting
{
    public class ScriptingAssemblyFileWatcher :
        IHandle<ProjectLoaded>,
        IHandle<ProjectUnloaded>
    {
        public event EventHandler<NewScriptingAssemblyEventArgs> NewScriptingAssemblyAvailable;
        private FileSystemWatcher _fileSystemWatcher;

        public ScriptingAssemblyFileWatcher(IEventAggregator globalEventAggregator)
        {
            globalEventAggregator.Subscribe(this);
        }

        public void Handle(ProjectLoaded message)
        {
            StartWatchingScriptingAssembly(message.LoadedProject.ScriptingAssemblyFilePath);
        }

        private void StartWatchingScriptingAssembly(string scriptingAssemblyFilePath)
        {
            var directory = Path.GetDirectoryName(scriptingAssemblyFilePath);
            _fileSystemWatcher = new FileSystemWatcher(directory, "*.dll");
            _fileSystemWatcher.NotifyFilter = NotifyFilters.FileName;
            _fileSystemWatcher.Created += FileCreated;
            _fileSystemWatcher.EnableRaisingEvents = true;
        }

        public void Handle(ProjectUnloaded message)
        {
            StopWatchingScriptingAssembly();
        }

        private void StopWatchingScriptingAssembly()
        {
            if (_fileSystemWatcher != null)
            {
                _fileSystemWatcher.Dispose();
                _fileSystemWatcher = null;
            }
        }

        private void FileCreated(object sender, FileSystemEventArgs e)
        {
            OnNewScriptingAssemblyAvailable(e.FullPath);
        }

        protected virtual void OnNewScriptingAssemblyAvailable(string scriptingAssemblyPath)
        {
            var handler = NewScriptingAssemblyAvailable;
            if (handler != null) handler(this, new NewScriptingAssemblyEventArgs(scriptingAssemblyPath));
        }
    }
}