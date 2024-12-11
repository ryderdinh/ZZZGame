#if UNITY_EDITOR

using DTT.PublishingTools;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DTT.Singletons.Editor
{
    /// <summary>
    /// A collumn dedicated to displaying the boot mode for a singleton profile item.
    /// </summary>
    internal class NameCollumn : ProfileTreeViewCollumn
    {
        #region Constructors
        /// <summary>
        /// Initializes the name collumn with the layout.
        /// </summary>
        /// <param name="layout">The tree view layout.</param>
        public NameCollumn(ProfileTreeViewLayout layout) : base(layout) { }
        #endregion

        #region Methods
        #region Public
        /// <summary>
        /// Initializes the layout for this collumn.
        /// </summary>
        public override void Initialize()
        {
            headerContent = new GUIContent("Name");
            headerTextAlignment = TextAlignment.Left;
            sortedAscending = true;
            sortingArrowAlignment = TextAlignment.Right;
            width = 150;
            minWidth = 60;
            autoResize = false;
            allowToggleVisibility = false;
        }

        /// <summary>
        /// Returns the ordered list of profile items when sorting by this collumn.
        /// </summary>
        /// <param name="items">The items to sort.</param>
        /// <param name="ascending">Whether or not the sorting is ascending.</param>
        /// <returns>The new ordered list of profile items.</returns>
        public override IOrderedEnumerable<ProfileTreeViewItem> Order(IEnumerable<ProfileTreeViewItem> items, bool ascending)
         => ascending ? items.OrderBy(i => i.displayName) : items.OrderByDescending(i => i.displayName);
        #endregion
        #region Protected
        /// <summary>
        /// Draws the GUI for a collumn of this cell.
        /// </summary>
        /// <param name="rect">The cell rectangle.</param>
        /// <param name="item">The item to draw for.</param>
        protected override void OnCellGUI(Rect rect, ProfileTreeViewItem item)
        {
            if (!item.isEditorOnly && !item.isScript)
            {
                SingletonPrefab prefab = p_TreeView.IdToSingletonPrefab(item.id);
                EditorGUI.BeginDisabledGroup(prefab.BootMode == BootModeType.DISABLED);
                GUI.Label(rect, item.displayName, DTTGUI.styles.Label);
                EditorGUI.EndDisabledGroup();

            }
            else
            {
                GUI.Label(rect, item.displayName, DTTGUI.styles.Label);
            }
        }
        #endregion
        #endregion
    }
}

#endif
