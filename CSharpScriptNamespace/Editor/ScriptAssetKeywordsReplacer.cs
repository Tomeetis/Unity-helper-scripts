using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MyGame.Editor.Assets
{
    public sealed class ScriptAssetKeywordsReplacer : UnityEditor.AssetModificationProcessor
    {
        /// <summary>
        /// Build project path nodes tree for ignoring path in namespace.
        /// Leave 'Children' unset (or empty) to indicate end (leaf) node.
        /// </summary>
        private static readonly PathNode IgnoredPathTree = new PathNode("Assets")
        {
            Children = new PathNode[]
            {
                new PathNode("Scripts")
                {
                    Children = new PathNode[]
                    {
                        new PathNode("Domain"),
                        new PathNode("Application"),
                        new PathNode("Infrastructure")
                    }
                }
            }
        };

        
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


        private static string AddPathSign(string pathString)
        {
            return pathString.EndsWith("/") ? pathString : pathString + "/";
        }

        private static string BuildNamespace(string systemPath)
        {
            var parts = new List<string>();

            if (!string.IsNullOrWhiteSpace(EditorSettings.projectGenerationRootNamespace))
            {
                parts.Add(EditorSettings.projectGenerationRootNamespace);
            }

            if (!string.IsNullOrWhiteSpace(systemPath))
            {
                parts.Add(systemPath);
            }

            var fullNamespace = string.Join(".", parts);

            return string.IsNullOrWhiteSpace(fullNamespace)
                ? "RootNamespaceNotSet"
                : fullNamespace;
        }

        private static void ReplaceScriptKeywords(string systemPath, string projectPath)
        {
            projectPath = projectPath.Substring(0, projectPath.LastIndexOf('/'));
            projectPath = TrimPathMembers(projectPath, IgnoredPathTree).Replace('/', '.').Trim('.');

            var fullNamespace = BuildNamespace(projectPath);

            var fileData = File.ReadAllText(systemPath);
            fileData = fileData.Replace("#NAMESPACE#", fullNamespace);
            File.WriteAllText(systemPath, fileData);
        }

        private static string TrimPathMembers(string projectPath, PathNode pathNode)
        {
            if (pathNode == null || !pathNode.Matches(projectPath))
            {
                return projectPath;
            }

            var trimString = projectPath.Length > pathNode.Name.Length
                ? AddPathSign(pathNode.Name)
                : pathNode.Name;

            projectPath = projectPath.Remove(0, trimString.Length);

            if (!pathNode.IsLeaf)
            {
                foreach(var childNode in pathNode.Children)
                {
                    projectPath = TrimPathMembers(projectPath, childNode);
                }
            }

            return projectPath;
        }

        
        private sealed class PathNode
        {
            public string Name { get; } = string.Empty;
            public PathNode[] Children { get; set; } = new PathNode[0];

            public bool IsLeaf => Children == null || Children.Length == 0;


            public PathNode(string name)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new ArgumentNullException(nameof(name));
                }

                // Forcing name to NOT end with '/'
                this.Name = name.EndsWith("/")
                    ? name.Substring(name.Length - 2, 1)
                    : name;
            }


            public bool Matches(string path)
            {
                return !string.IsNullOrWhiteSpace(Name) &&
                    path.StartsWith(Name, StringComparison.InvariantCultureIgnoreCase);
            }
        }
    }
}
