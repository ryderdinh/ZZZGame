using UnityEngine;

#if UNITY_EDITOR

namespace DTT.Networking.ConnectionStatus.Editor
{
    /// <summary>
    /// An interface for an internet status window tab to implement.
    /// </summary>
    internal interface IInternetStatusWindowTab
    {
        /// <summary>
        /// The tool bar title of the tab.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Draws the tab GUI.
        /// </summary>
        void OnTabGUI(Rect position);

        /// <summary>
        /// Called to refresh states at important time frames.
        /// </summary>
        void Refresh();
    }
}

#endif