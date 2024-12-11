#if UNITY_EDITOR

using DTT.PublishingTools;
using System.Collections.Generic;
using System.Linq;
using DTT.Utils.EditorUtilities;
using UnityEngine;

namespace DTT.Singletons.Editor
{
    /// <summary>
    /// A collumn dedicated to displaying the boot mode for a singleton profile item.
    /// </summary>
    internal class EditCollumn : ProfileTreeViewCollumn
    {
        #region Variables
        #region Private
        /// <summary>
        /// The horizontal margin used for drawing the button.
        /// </summary>
        private const float HORIZONTAL_MARGIN = 5f;
        #endregion
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes the edit collumn with the layout.
        /// </summary>
        /// <param name="layout">The tree view layout.</param>
        public EditCollumn(ProfileTreeViewLayout layout) : base(layout) { }
        #endregion

        #region Methods
        #region Public
        /// <summary>
        /// Initializes the layout for this collumn.
        /// </summary>
        public override void Initialize()
        {
            headerContent = new GUIContent("Edit");
            headerTextAlignment = TextAlignment.Left;
            sortedAscending = true;
            sortingArrowAlignment = TextAlignment.Right;
            width = 120;
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
         => ascending ? items.OrderBy(i => !i.isScript) : items.OrderByDescending(i => !i.isScript);
        #endregion
        #region Protected
        /// <summary>
        /// Draws the GUI for a collumn of this cell.
        /// </summary>
        /// <param name="rect">The cell rectangle.</param>
        /// <param name="item">The item to draw for.</param>
        protected override void OnCellGUI(Rect rect, ProfileTreeViewItem item)
        {
            rect.x += HORIZONTAL_MARGIN;
            rect.width -= HORIZONTAL_MARGIN * 2f;

            if (item.isScript)
            {
                if (GUI.Button(rect, "Script", DTTGUI.styles.Button))
                {
                    TextAsset asset = p_TreeView.IdToScriptAsset(item.id);
                    AssetDatabaseUtility.OpenScript(asset);
                }
            }
            else
            {
                if (GUI.Button(rect, "Prefab", DTTGUI.styles.Button))
                {
                    GameObject asset = p_TreeView.IdToPrefabAsset(item.id);
                    AssetDatabaseUtility.OpenPrefab(asset);
                }
            }
        }
        #endregion
        #endregion
    }
}

#endif