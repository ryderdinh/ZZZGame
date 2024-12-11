#if UNITY_EDITOR

using DTT.PublishingTools;
using UnityEditor;

namespace DTT.Singletons.Editor
{
    /// <summary>
    /// A helper class that provides the menu item to open the readme for this package.
    /// </summary>
    public class ReadMeOpener
    {
        /// <summary>
        /// Opens the readme for this package.
        /// </summary>
        [MenuItem("Tools/DTT/SingletonCore/ReadMe")]
        private static void OpenReadMe() => DTTReadMeEditorWindow.Open(DTTEditorConfig.GetAssetJson("dtt.singletoncore"));
    }
}

#endif