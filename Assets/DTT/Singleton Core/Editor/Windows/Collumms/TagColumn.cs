#if UNITY_EDITOR

using DTT.PublishingTools;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DTT.Singletons.Editor
{
    /// <summary>
    /// A collumn dedicated to displaying the tag for a singleton profile item (e.g. prefab or script).
    /// </summary>
    internal class TagColumn : ProfileTreeViewCollumn
    {
        #region Constructors
        /// <summary>
        /// Initializes the tag collumn with the layout.
        /// </summary>
        /// <param name="layout">The tree view layout.</param>
        public TagColumn(ProfileTreeViewLayout layout) : base(layout) { }
        #endregion

        #region Methods
        #region Public
        /// <summary>
        /// Initializes the layout for this collumn.
        /// </summary>
        public override void Initialize()
        {
            headerContent = new GUIContent(EditorTextures.Load<Texture>(
                SingletonEditorConfig.FULL_PACKAGE_NAME,
                EditorGUIUtility.isProSkin ? "model_light.png" : "model_dark.png"
            ));
            headerContent.tooltip = "Filter by asset type.";
            contextMenuText = "Asset";
            headerTextAlignment = TextAlignment.Center;
            sortedAscending = true;
            sortingArrowAlignment = TextAlignment.Right;
            width = 30;
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
         => ascending ? items.OrderBy(i => !i.isScript) : items.OrderByDescending(i => !i.isScript);
        #endregion
        #region Protected
        /// <summary>
        /// Returns the order for an item sorting by this collumn.
        /// </summary>
        /// <param name="item">The item to sort.</param>
        /// <returns>The order for the item.</returns>
        protected override void OnCellGUI(Rect rect, ProfileTreeViewItem item)
        {
            if (item.isScript)
                GUI.Label(rect, ProfileWindow.Content.ScriptTag, ProfileWindow.Styles.EmptyCentered);
            else
                GUI.Label(rect, ProfileWindow.Content.PrefabTag, ProfileWindow.Styles.EmptyCentered);
        }
        #endregion
        #endregion
    }
}

#endif