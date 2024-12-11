#if UNITY_EDITOR

using DTT.PublishingTools;
using UnityEngine;

namespace DTT.Networking.ConnectionStatus.Editor
{
    /// <summary>
    /// Draws targets in a tree view control inside the tab view.
    /// </summary>
    internal class InternetStatusTargetsTab : IInternetStatusWindowTab
    {
        ///<inheritdoc/>
        public string Title => "Targets";

        /// <summary>
        /// The tree view used for drawing the targets.
        /// </summary>
        private readonly TargetsTreeView _treeView;

        /// <summary>
        /// Initializes the tab with the window state.
        /// </summary>
        /// <param name="windowState">The serialized state of the window.</param>
        public InternetStatusTargetsTab(InternetStatusWindowState windowState)
            =>  _treeView = new TargetsTreeView(windowState); 

        /// <summary>
        /// Draws the tree view with targets in the tab view.
        /// </summary>
        /// <param name="position">The position of the window.</param>
        public void OnTabGUI(Rect position)
        {
            Rect rect = GUILayoutUtility.GetRect(position.width, position.height - DTTHeaderGUI.HEADER_HEIGHT);
            _treeView.OnGUI(rect);
        }

        /// <summary>
        /// Reloads the tree view.
        /// </summary>
        public void Refresh() => _treeView.Reload();
    }
}

#endif