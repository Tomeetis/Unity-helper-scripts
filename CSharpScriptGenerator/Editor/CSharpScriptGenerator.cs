using System;
using System.Text;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MyGame.Editor
{
    public sealed class CSharpScriptGenerator : UnityEditor.AssetModificationProcessor
    {
        private const bool ExcludeScriptsFromNamespace = true;
        private const bool MakeClassSealedByDefault = true;
        private const string RootScriptsFolderName = "SCRIPTS";

        private static readonly string[] _DefaultClassUsings = { "UnityEngine" };
        private static readonly string[] _DefaultInterfaceUsings = { };


        /// <summary>
        ///  This gets called for every .meta file created by the Editor.
        /// </summary>
        public static void OnWillCreateAsset(string path)
        {
            path = path.Replace(".meta", string.Empty);

            if (!IsScriptAsset(path))
            {
                return;
            }

            var isInterface = Path.GetFileNameWithoutExtension(path).StartsWith("I", StringComparison.InvariantCultureIgnoreCase) &&
                EditorUtility.DisplayDialog(
                    "Class or interfacer?",
                    "Do you want to create CLASS or INTERFACE?",
                    "Interface",
                    "Class");

            var content = isInterface ? MakeInterfaceContent(path) : MakeClassContent(path);
            var filePath = ResolveSystemPathFromProjectPath(path);

            File.WriteAllText(filePath, content);
        }


        private static bool IsScriptAsset(string path)
        {
            return path.EndsWith(".cs");
        }

        private static string MakeClassContent(string path)
        {
            var fullNamespace = ResolveNamespaceFromPath(path);
            var fileName = Path.GetFileNameWithoutExtension(path);
            var sealedKeyword = MakeClassSealedByDefault ? " sealed " : " ";
            var identation = string.Empty;

            var stringBuilder = new StringBuilder();

            if (_DefaultClassUsings.Length > 0)
            {
                foreach (var u in _DefaultClassUsings)
                {
                    stringBuilder.AppendLine($"using {u};");
                }

                stringBuilder.AppendLine();
            }

            if (!string.IsNullOrWhiteSpace(fullNamespace))
            {
                stringBuilder.AppendLine($"namespace {fullNamespace}");
                stringBuilder.AppendLine("{");
                identation += "    ";
            }
            stringBuilder.AppendLine($"{identation}public{sealedKeyword}class {fileName} : MonoBehaviour");
            stringBuilder.AppendLine($"{identation}{{");
            stringBuilder.AppendLine($"{identation}}}");

            if (!string.IsNullOrWhiteSpace(fullNamespace))
            {
                stringBuilder.AppendLine("}");
                identation = identation.Substring(0, identation.Length - 4);
            }

            return stringBuilder.ToString();
        }

        private static string MakeInterfaceContent(string path)
        {
            var fullNamespace = ResolveNamespaceFromPath(path);
            var fileName = Path.GetFileNameWithoutExtension(path);
            var identation = string.Empty;

            var stringBuilder = new StringBuilder();

            if (_DefaultInterfaceUsings.Length > 0)
            {
                foreach (var u in _DefaultInterfaceUsings)
                {
                    stringBuilder.AppendLine($"using {u};");
                }

                stringBuilder.AppendLine();
            }

            if (!string.IsNullOrWhiteSpace(fullNamespace))
            {
                stringBuilder.AppendLine($"namespace {fullNamespace}");
                stringBuilder.AppendLine("{");
                identation += "    ";
            }

            stringBuilder.AppendLine($"{identation}public interface {fileName}");
            stringBuilder.AppendLine($"{identation}{{");
            stringBuilder.AppendLine($"{identation}}}");

            if (!string.IsNullOrWhiteSpace(fullNamespace))
            {
                stringBuilder.AppendLine("}");
                identation = identation.Substring(0, identation.Length - 4);
            }

            return stringBuilder.ToString();
        }

        private static string ResolveNamespaceFromPath(string pathInProject)
        {
            var rootScriptsFolderName = $"/{RootScriptsFolderName}/";
            pathInProject = pathInProject.Substring(pathInProject.IndexOf(rootScriptsFolderName) + rootScriptsFolderName.Length);

            var lastDirectoryIndex = pathInProject.LastIndexOf("/");
            if (lastDirectoryIndex > 0)
            {
                pathInProject = pathInProject.Substring(0, lastDirectoryIndex);

                if (ExcludeScriptsFromNamespace)
                {
                    pathInProject = pathInProject.Replace("/Scripts/", "/").Replace('/', '.');
                }
            }
            else
            {
                pathInProject = string.Empty;
            }

            var rootNamespace = string.Empty;
            var separator = string.Empty;

            if (!string.IsNullOrWhiteSpace(EditorSettings.projectGenerationRootNamespace))
            {
                rootNamespace = EditorSettings.projectGenerationRootNamespace;

                if (!string.IsNullOrWhiteSpace(pathInProject))
                {
                    separator = ".";
                }
            }

            var fullNamespace = $"{rootNamespace}{separator}{pathInProject}";
            return fullNamespace;
        }

        private static string ResolveSystemPathFromProjectPath(string projectPath)
        {
            var systemPath = projectPath.Insert(0, Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("Assets")));
            return systemPath;
        }
    }
}
