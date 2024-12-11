#if UNITY_EDITOR

using DTT.PublishingTools;
using UnityEditor;

/// <summary>
/// Class that handles opening the editor window for the connection status package
/// </summary>
internal class OpenConnectionStatusReadMe
{
    /// <summary>
    /// Opens the readme for this package
    /// </summary>
    [MenuItem("Tools/DTT/ConnectionStatus/ReadMe")]
    private static void OpenReadMe() => DTTEditorConfig.OpenReadMe("dtt.connectionstatus");
}

#endif