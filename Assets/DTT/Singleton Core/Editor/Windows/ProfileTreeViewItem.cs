#if UNITY_EDITOR

using UnityEditor.IMGUI.Controls;

namespace DTT.Singletons.Editor
{
    /// <summary>
    /// A tree view item used in the singleton profile tree view.
    /// </summary>
    public class ProfileTreeViewItem : TreeViewItem
    {
        #region Variables
        #region Public
        /// <summary>
        /// Whether the item is related to an editor only singleton.
        /// </summary>
        public readonly bool isEditorOnly;

        /// <summary>
        /// Whether the item is related to an script only singleton.
        /// </summary>
        public readonly bool isScript;
        #endregion
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes the item.
        /// </summary>
        /// <param name="isPartOfRuntime">Whether the item is related to an editor only singleton.</param>
        /// <param name="isScript">Whether the item is related to an script only singleton.</param>
        public ProfileTreeViewItem(bool isPartOfRuntime, bool isScript)
        {
            this.isEditorOnly = isPartOfRuntime;
            this.isScript = isScript;
        }
        #endregion
    }
}

#endif