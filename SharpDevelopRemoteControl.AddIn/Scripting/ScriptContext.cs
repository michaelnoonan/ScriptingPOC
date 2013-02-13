using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace SharpDevelopRemoteControl.AddIn.Scripting
{
    public class ScriptContext
    {
        public ScriptContext(
            IProject currentProject,
            FileName currentFileName,
            Location caretLocation,
            IClass declaringClass,
            IMethod method)
        {
            CurrentProject = currentProject;
            CurrentFileName = currentFileName;
            CaretLocation = caretLocation;
            DeclaringClass = declaringClass;
            Method = method;
        }

        public IProject CurrentProject { get; private set; }
        public FileName CurrentFileName { get; private set; }
        public Location CaretLocation { get; private set; }
        public IClass DeclaringClass { get; private set; }
        public IMethod Method { get; private set; }
    }
}