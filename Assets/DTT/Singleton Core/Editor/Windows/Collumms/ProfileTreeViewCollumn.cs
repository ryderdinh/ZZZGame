#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace DTT.Singletons.Editor
{
    /// <summary>
    /// An abstract representatin of a collumn that is part of the singleton profile tree view.
    /// </summary>
    internal abstract class ProfileTreeViewCollumn : MultiColumnHeaderState.Column
    {
        #region Variables
        #region Protected
        /// <summary>
        /// The tree view reference.
        /// </summary>
        protected ProfileTreeView p_TreeView => _layout.GetTreeViewLazy();
        #endregion
        #region Private
        /// <summary>
        /// Whether or not the collumn is initialized or not. 
        /// <para>This needs to be done lazily.</para>
        /// </summary>
        private bool _initialized;

        /// <summary>
        /// The layout used to retrieve the tree view reference.
        /// </summary>
        private ProfileTreeViewLayout _layout;
        #endregion
        #endregion

        #region Constructors
        public ProfileTreeViewCollumn(ProfileTreeViewLayout layout) => _layout = layout;
        #endregion

        #region Variables
        #region Methods
        /// <summary>
        /// Draws the GUI for this collumn.
        /// </summary>
        /// <param name="rect">The cell rectangle.</param>
        /// <param name="item">The item to draw for.</param>
        public void OnGUI(Rect cellRect, TreeViewItem item)
        {
            if (!_initialized)
            {
                Initialize();
                _initialized = true;
            }

            // Use 'as' cast to avoid cast exception when tree view item is null.
            ProfileTreeViewItem profileTreeViewItem = item as ProfileTreeViewItem;
            if (profileTreeViewItem != null)
                OnCellGUI(cellRect, profileTreeViewItem);
        }

        /// <summary>
        /// Initializes this collumn.
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Returns the ordered list of profile items when sorting by this collumn.
        /// </summary>
        /// <param name="items">The items to sort.</param>
        /// <param name="ascending">Whether or not the sorting is ascending.</param>
        /// <returns>The new ordered list of profile items.</returns>
        public abstract IOrderedEnumerable<ProfileTreeViewItem> Order(IEnumerable<ProfileTreeViewItem> items, bool ascending);
        #endregion
        #region Protected
        /// <summary>
        /// Draws the GUI for a cell of this collumn.
        /// </summary>
        /// <param name="rect">The cell rectangle.</param>
        /// <param name="item">The item to draw for.</param>
        protected abstract void OnCellGUI(Rect rec, ProfileTreeViewItem item);
        #endregion
        #endregion
    }
}

#endif