using System.IO;
using UnityEditor;
using UnityEngine;

namespace SlingshotBob.Editor.Assets
{
    public sealed class ScriptAssetKeywordsReplacer : UnityEditor.AssetModificationProcessor
    {
        /// <summary>
        ///  This gets called for every .meta file created by the Editor.
        /// </summary>
        public static void OnWillCreateAsset(string path)
        {
            path = path.Replace(".meta", string.Empty);

            if (!path.EndsWith(".cs"))
            {
                return;
            }

            var systemPath = path.Insert(0, Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("Assets")));

            ReplaceScriptKeywords(systemPath, path);

            AssetDatabase.Refresh();
        }


        private static void ReplaceScriptKeywords(string systemPath, string projectPath)
        {
            projectPath = projectPath.Substring(projectPath.IndexOf("/SCRIPTS/") + "/SCRIPTS/".Length);
            projectPath = projectPath.Substring(0, projectPath.LastIndexOf("/"));
            projectPath = projectPath.Replace("/Scripts/", "/").Replace('/', '.');

            var rootNamespace = string.IsNullOrWhiteSpace(EditorSettings.projectGenerationRootNamespace) ?
                string.Empty :
                $"{EditorSettings.projectGenerationRootNamespace}.";

            var fullNamespace = $"{rootNamespace}{projectPath}";

            var fileData = File.ReadAllText(systemPath);

            fileData = fileData.Replace("#NAMESPACE#", fullNamespace);

            File.WriteAllText(systemPath, fileData);
        }
    }
}
