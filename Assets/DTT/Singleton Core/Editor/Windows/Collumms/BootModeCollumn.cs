#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DTT.Singletons.Editor
{
    /// <summary>
    /// A collumn dedicated to displaying the boot mode for a singleton profile item.
    /// </summary>
    internal class BootModeCollumn : ProfileTreeViewCollumn
    {
        #region Constructors
        /// <summary>
        /// Initializes the boot mode collumn with the layout.
        /// </summary>
        /// <param name="layout">The tree view layout.</param>
        public BootModeCollumn(ProfileTreeViewLayout layout) : base(layout) { }
        #endregion

        #region Methods
        #region Public
        /// <summary>
        /// Initializes the layout for this collumn.
        /// </summary>
        public override void Initialize()
        {
            headerContent = new GUIContent("Boot mode");
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
        /// Draws the GUI for a collumn of this cell.
        /// </summary>
        /// <param name="rect">The cell rectangle.</param>
        /// <param name="item">The item to draw for.</param>
        protected override void OnCellGUI(Rect rect, ProfileTreeViewItem item)
        {
            if (!item.isScript && !item.isEditorOnly)
            {
                SingletonPrefab prefab = p_TreeView.IdToSingletonPrefab(item.id);
                if (prefab.usesAttribute)
                {
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUI.EnumPopup(rect, prefab.BootMode);
                    EditorGUI.EndDisabledGroup();
                }
                else
                {
                    BootModeType mode = (BootModeType)EditorGUI.EnumPopup(rect, prefab.BootMode);
                    if (mode != prefab.BootMode)
                    {
                        SingletonProfileManager.UpdateBootMode(prefab.asset, mode);

                        List<int> selectedRows = new List<int>(p_TreeView.GetSelection());
                        bool isOnlySelected = selectedRows.Count == 0 || (selectedRows.Count == 1 && selectedRows[0] == item.id);
                        if (!isOnlySelected)
                        {
                            // If this is not the only selected row, ensure this row is not part 
                            // of the selection and update the other rows as wel.
                            selectedRows.Remove(item.id);

                            TryUpdateOtherSelectedRowsWithBootMode(selectedRows, mode);
                        }

                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                }
            }
        }
        #endregion
        #region Private
        /// <summary>
        /// Tries updating selected rows other than this one with a boot mode type.
        /// <para>Will filter out non-runtime-prefabs.</para>
        /// </summary>
        /// <param name="otherSelectedRows">The other selected rows.</param>
        /// <param name="mode">The new boot mode type.</param>
        private void TryUpdateOtherSelectedRowsWithBootMode(List<int> otherSelectedRows, BootModeType mode)
        {
            IEnumerable<SingletonPrefab> prefabs = p_TreeView.IdsToRuntimePrefabs(otherSelectedRows);
            foreach (SingletonPrefab prefab in prefabs)
                if (!prefab.usesAttribute)
                    SingletonProfileManager.UpdateBootMode(prefab.asset, mode);
        }

        /// <summary>
        /// Returns the order for an item sorting by this collumn.
        /// </summary>
        /// <param name="item">The item to sort.</param>
        /// <returns>The order for the item.</returns>
        private int GetOrderForItem(ProfileTreeViewItem item)
        {
            bool unEditable = item.isScript || item.isEditorOnly;
            int initial = unEditable ? 0 : 1;
            int additional = unEditable ? 0 : (int)p_TreeView.IdToSingletonPrefab(item.id).BootMode;
            return initial + additional;
        }
        #endregion
        #endregion
    }
}

#endif
