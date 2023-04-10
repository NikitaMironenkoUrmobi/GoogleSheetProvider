using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Redpenguin.GoogleSheets.Editor.Utils
{
    /// <summary>
    /// Adds the given define symbols to PlayerSettings define symbols.
    /// Just add your own define symbols to the Symbols property at the below.
    /// </summary>
    [InitializeOnLoad]
    public class DefineSymbolsSetuper : UnityEditor.Editor
    {
        /// <summary>
        /// Symbols that will be added to the editor
        /// </summary>
        private static readonly Dictionary<string,string> SymbolsDict = new()
        {
            {"Zenject", "ENABLE_ZENJECT"},
            {"NaughtyAttributes", "ENABLE_NAUGHTYATTRIBUTES"}
        };

        /// <summary>
        /// Add define symbols as soon as Unity gets done compiling.
        /// </summary>
        static DefineSymbolsSetuper()
        {
            var definesString =
                PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            var allDefines = definesString.Split(';').ToList();
            foreach (var symbols in SymbolsDict)
            {
                if(allDefines.Contains(symbols.Value)) continue;
                if(NamespaceExist(symbols.Key))
                    allDefines.Add(symbols.Value);
            }
            
            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup,
                string.Join(";", allDefines.ToArray()));
        }

        private static bool NamespaceExist(string nameSpace)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if(type.Namespace == nameSpace)
                        return true;
                }
            }
            return false;
        }
    }
}