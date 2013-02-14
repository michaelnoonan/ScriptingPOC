using System;
using System.IO;
using System.Reflection;
using ICSharpCode.SharpZipLib.Zip;

namespace MyCoolApp.Domain.Scripting
{
    public class ScriptingProjectBuilder : IScriptingProjectBuilder
    {
        public void BuildScriptingProject(string scriptingProjectName, string scriptingProjectFilePath)
        {
            var projectDirectory = Path.GetDirectoryName(scriptingProjectFilePath);

            var zip = new FastZip();
            using (var s = Assembly.GetExecutingAssembly()
                                .GetManifestResourceStream(GetType(), "VBScriptingProjectTemplate.zip"))
            {
                zip.ExtractZip(s, projectDirectory, FastZip.Overwrite.Always, name => true, null, null, false, true);
            }

            File.Move(Path.Combine(projectDirectory, "{{ProjectName}}.vbproj"), scriptingProjectFilePath);

            ReplaceTokensInFile(scriptingProjectFilePath, scriptingProjectName);
            ReplaceTokensInFile(Path.Combine(projectDirectory, @"My Project\AssemblyInfo.vb"), scriptingProjectName);
        }

        private static void ReplaceTokensInFile(string filePath, string projectName)
        {
            var guid = Guid.NewGuid();
            var projectFileText = File.ReadAllText(filePath);
            projectFileText = projectFileText.Replace("{{ProjectName}}", projectName);
            projectFileText = projectFileText.Replace("{{GUID}}", guid.ToString("B"));
            projectFileText = projectFileText.Replace("{{Year}}", DateTime.Today.Year.ToString());
            File.WriteAllText(filePath, projectFileText);
        }
    }
}