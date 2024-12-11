#if UNITY_EDITOR

using DTT.PublishingTools;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DTT.Singletons.Editor
{
    /// <summary>
    /// A collumn dedicated to displaying the type of the singleton profile item (e.g. editor or runtime).
    /// </summary>
    internal class TypeCollumn : ProfileTreeViewCollumn
    {
        #region Constructors
        /// <summary>
        /// Initializes the type collumn with the layout.
        /// </summary>
        /// <param name="layout">The tree view layout.</param>
        public TypeCollumn(ProfileTreeViewLayout layout) : base(layout) { }
        #endregion

        #region Methods
        #region Public
        /// <summary>
        /// Initializes the layout for this collumn.
        /// </summary>
        public override void Initialize()
        {
            headerContent = new GUIContent("Type", "Filter singletons by type");
            contextMenuText = "Type";
            headerTextAlignment = TextAlignment.Left;
            sortedAscending = true;
            sortingArrowAlignment = TextAlignment.Right;
            width = 60;
            minWidth = 30;
            maxWidth = 60;
            autoResize = false;
            allowToggleVisibility = true;
        }

        /// <summary>
        /// Returns the ordered list of profile items when sorting by this collumn.
        /// </summary>
        /// <param name="items">The items to sort.</param>
        /// <param name="ascending">Whether or not the sorting is ascending.</param>
        /// <returns>The new ordered list of profile items.</returns>
        public override IOrderedEnumerable<ProfileTreeViewItem> Order(IEnumerable<ProfileTreeViewItem> items, bool ascending)
            => ascending ? items.OrderBy(i => !i.isEditorOnly) : items.OrderByDescending(i => !i.isEditorOnly);
        #endregion
        #region Protected
        /// <summary>
        /// Returns the order for an item sorting by this collumn.
        /// </summary>
        /// <param name="item">The item to sort.</param>
        /// <returns>The order for the item.</returns>
        protected override void OnCellGUI(Rect rect, ProfileTreeViewItem item)
        {
            if (item.isEditorOnly)
                GUI.Label(rect, ProfileWindow.Content.EditorTag, DTTGUI.styles.Label);
            else
                GUI.Label(rect, ProfileWindow.Content.RuntimeTag, DTTGUI.styles.Label);
        }
        #endregion
        #endregion
    }
}

#endif
