#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DTT.Singletons.Editor
{
    /// <summary>
    /// A collumn dedicated to displaying the boot data for a singleton profile item.
    /// <para>Is currently used for displaying the boot scene only.</para>
    /// </summary>
    internal class BootDataCollumn : ProfileTreeViewCollumn
    {
        #region Constructors
        /// <summary>
        /// Initializes the boot data collumn with the layout.
        /// </summary>
        /// <param name="layout">The tree view layout.</param>
        public BootDataCollumn(ProfileTreeViewLayout layout) : base(layout) { }
        #endregion

        #region Methods
        #region Public
        /// <summary>
        /// Initializes the layout for this collumn.
        /// </summary>
        public override void Initialize()
        {
            headerContent = new GUIContent("Boot scene", "Scene used for bootstrapping the singleton.");
            headerTextAlignment = TextAlignment.Left;
            sortedAscending = true;
            sortingArrowAlignment = TextAlignment.Right;
            width = 100;
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
            => ascending ? items.OrderBy(i => GetOrderForItem(i)) : items.OrderByDescending(i => GetOrderForItem(i));
        #endregion
        #region Protected
        /// <summary>
        /// Draws the GUI for a cell of this collumn.
        /// </summary>
        /// <param name="rect">The cell rectangle.</param>
        /// <param name="item">The item to draw for.</param>
        protected override void OnCellGUI(Rect rect, ProfileTreeViewItem item)
        {
            if (!item.isEditorOnly && !item.isScript)
            {
                SingletonPrefab prefab = p_TreeView.IdToSingletonPrefab(item.id);
                EditorGUI.BeginDisabledGroup(prefab.BootMode != BootModeType.SCENE_BASED);
                {
                    SceneAsset newSceneAsset = (SceneAsset)EditorGUI.ObjectField(rect, prefab.sceneAsset, typeof(SceneAsset), false);

                    if (newSceneAsset != prefab.sceneAsset)
                        SingletonProfileManager.AddSceneToSingletonPrefab(prefab.asset, newSceneAsset);
                }
                EditorGUI.EndDisabledGroup();
            }
        }
        #endregion
        #region Private
        /// <summary>
        /// Returns the order for an item sorting by this collumn.
        /// </summary>
        /// <param name="item">The item to sort.</param>
        /// <returns>The order for the item.</returns>
        private int GetOrderForItem(ProfileTreeViewItem item)
        {
            if (item.isScript || item.isEditorOnly)
            {
                return 0;
            }
            else
            {
                SingletonPrefab prefab = p_TreeView.IdToSingletonPrefab(item.id);
                int score = 1;
                if (prefab.BootMode == BootModeType.SCENE_BASED)
                    score++;
                if (prefab.sceneAsset != null)
                    score++;

                return score;
            }
        }
        #endregion
        #endregion
    }
}

#endif