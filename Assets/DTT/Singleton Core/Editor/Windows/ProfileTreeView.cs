#if UNITY_EDITOR

using DTT.PublishingTools;
using System.Collections.Generic;
using System.Linq;
using DTT.Utils.EditorUtilities;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace DTT.Singletons.Editor
{
    /// <summary>
    /// Displays the singletons in the profile in a multicollumn tree view format.
    /// </summary>
    public class ProfileTreeView : TreeView
    {
        #region Variables
        #region Public
        /// <summary>
        /// Whether assets could be found when building the tree view.
        /// </summary>
        public bool CouldInitialize { get; private set; }
        #endregion
        #region Private
        /// <summary>
        /// An empty tree view item used when the tree view couldn't initialize.
        /// </summary>
        private TreeViewItem _Empty => new TreeViewItem(-1, 0, "Empty");

        /// <summary>
        /// The collumns used for this tree view.
        /// </summary>
        private readonly Dictionary<int, ProfileTreeViewCollumn> _collumns;

        /// <summary>
        /// The height used for a row.
        /// </summary>
        private const float ROW_HEIGHT = 20f;
        #endregion
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes the tree view.
        /// </summary>
        /// <param name="state">The serialized tree view state.</param>
        /// <param name="header">The header used for the tree view.</param>
        public ProfileTreeView(TreeViewState state, MultiColumnHeader header) : base(state, header)
        {
            _collumns = new Dictionary<int, ProfileTreeViewCollumn>();

            int count = 0;
            foreach (ProfileTreeViewCollumn collumn in header.state.columns.Cast<ProfileTreeViewCollumn>())
                _collumns.Add(count++, collumn);

            rowHeight = ROW_HEIGHT;
            showAlternatingRowBackgrounds = true;
            multiColumnHeader.sortingChanged += OnSortingChanged;
            showBorder = true;

            Reload();
        }
        #endregion

        #region Methods
        #region Public
        /// <summary>
        /// Returns the prefab asset based on the given tree view id.
        /// </summary>
        /// <param name="id">The tree view id.</param>
        /// <returns>The prefab asset.</returns>
        public GameObject IdToPrefabAsset(int id) => EditorUtility.InstanceIDToObject(id) as GameObject;

        /// <summary>
        /// Returns the script asset based on the given tree view item id.
        /// </summary>
        /// <param name="id">The tree view item id.</param>
        /// <returns>The script asset.</returns>
        public TextAsset IdToScriptAsset(int id) => EditorUtility.InstanceIDToObject(id) as TextAsset;

        /// <summary>
        /// Returns a singleton prefab based on the given tree view id.
        /// </summary>
        /// <param name="id">The tree view id.</param>
        /// <returns>The singleton prefab.</returns>
        public SingletonPrefab IdToSingletonPrefab(int id) => SingletonProfileManager.GetSingletonPrefab(IdToPrefabAsset(id));

        /// <summary>
        /// Returns a singleton script based on the given tree view item id.
        /// </summary>
        /// <param name="id">The tree view item id.</param>
        /// <returns>The singleton script.</returns>
        public SingletonScript IdToSingletonScript(int id) => SingletonProfileManager.GetSingletonScript(IdToScriptAsset(id));

        /// <summary>
        /// Validates the content the tree view should display.
        /// Triggers a reload if the content is invalid.
        /// </summary>
        public void ValidateContent()
        {
            IList<TreeViewItem> items = GetRows();
            for (int i = 0; i < items.Count; i++)
            {
                if (EditorUtility.InstanceIDToObject(items[i].id) == null)
                {
                    Reload();
                    break;
                }
            }
        }

        /// <summary>
        /// Returns an enumerable of runtime prefabs for given tree view ids.
        /// </summary>
        /// <param name="ids">The tree view ids.</param>
        /// <returns>The enumerable of runtime prefabs.</returns>
        public IEnumerable<SingletonPrefab> IdsToRuntimePrefabs(IList<int> ids)
        {
            return ids.Where(id =>
            {
                ProfileTreeViewItem item = (ProfileTreeViewItem)FindItem(id, rootItem);
                return !item.isScript && !item.isEditorOnly;
            })
            .Select(id => IdToSingletonPrefab(id))
            .AsEnumerable();
        }
        #endregion
        #region Protected
        /// <summary>
        /// Draws the GUI for a row.
        /// </summary>
        /// <param name="args">The arguments needed for drawing the row.</param>
        protected override void RowGUI(RowGUIArgs args)
        {
            DrawRowBackground(args);
            DrawVisibleCollumns(args);
        }

        /// <summary>
        /// Initializes the tree view with tree view items for singleton prefabs and singleton scripts.
        /// </summary>
        /// <returns>The root of the tree.</returns>
        protected override TreeViewItem BuildRoot()
        {
            SingletonProfileManager.Refresh();

            TreeViewItem root = TreeViewUtility.DefaultRoot;

            CouldInitialize = AddPrefabsToRoot(root) | AddScriptsToRoot(root);

            if (!CouldInitialize)
                root.AddChild(_Empty);

            return root;
        }

        /// <summary>
        /// Initializes the rows and sorts them before doing so.
        /// </summary>
        /// <param name="root">The root of the tree.</param>
        /// <returns>The sorted rows.</returns>
        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            IList<TreeViewItem> rows = base.BuildRows(root);
            TrySortRows(root, rows);
            return rows;
        }
        #endregion
        #region Private
        /// <summary>
        /// Draws the background for the row. 
        /// </summary>
        /// <param name="args">The arguments for drawing the row gui.</param>
        private void DrawRowBackground(RowGUIArgs args)
        {
            if (args.selected)
                EditorGUI.DrawRect(args.rowRect, DTTColors.light.line);
        }

        /// <summary>
        /// Tries sorting a list of given rows and updates the roots children with the new order.
        /// </summary>
        /// <param name="root">The root tree view item.</param>
        /// <param name="rows">The rows to sort.</param>
        private void TrySortRows(TreeViewItem root, IList<TreeViewItem> rows)
        {
            // If there is one or less row, or there is no collumn to sort,
            // do nothing.
            if (rows.Count <= 1 && multiColumnHeader.sortedColumnIndex == -1)
                return;

            // Get the indices of collumns that are sorted.
            int[] sortedCollumns = multiColumnHeader.state.sortedColumns;
            if (sortedCollumns.Length != 0)
            {
                int current = sortedCollumns[0];
                IOrderedEnumerable<ProfileTreeViewItem> sorted = _collumns[current].Order(
                    root.children.Cast<ProfileTreeViewItem>(),
                    multiColumnHeader.IsSortedAscending(current));

                root.children = sorted.Cast<TreeViewItem>().ToList();
            }

            TreeToList(rows);
            Repaint();
        }

        /// <summary>
        /// Updates a list with the a tree structure, flattening it.
        /// </summary>
        /// <param name="result">The resulting list of tree view items.</param>
        private void TreeToList(IList<TreeViewItem> result)
        {
            result.Clear();

            // Create a stack of tree view items and add top level items to it.
            Stack<TreeViewItem> stack = new Stack<TreeViewItem>();
            for (int i = rootItem.children.Count - 1; i >= 0; i--)
                stack.Push(rootItem.children[i]);

            // While the stack is not empty, pop items and add them to the result.
            // If an item has children push them onto the stack.
            while (stack.Count > 0)
            {
                TreeViewItem current = stack.Pop();
                result.Add(current);

                if (current.hasChildren && current.children[0] != null)
                {
                    for (int i = current.children.Count - 1; i >= 0; i--)
                    {
                        stack.Push(current.children[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Called when the sorting has changed in the multicollumn header. It
        /// tries sorting the rows based on the new sorted collumn.
        /// </summary>
        /// <param name="header"></param>
        private void OnSortingChanged(MultiColumnHeader header) => TrySortRows(rootItem, GetRows());

        /// <summary>
        /// Draws the visible collumn using row GUI arguments.
        /// </summary>
        /// <param name="args">The arguments used to draw the row GUI.</param>
        private void DrawVisibleCollumns(RowGUIArgs args)
        {
            int visibleRows = args.GetNumVisibleColumns();

            for (int i = 0; i < visibleRows; i++)
            {
                Rect rect = args.GetCellRect(i);

                CenterRectUsingSingleLineHeight(ref rect);

                _collumns[args.GetColumn(i)].OnGUI(rect, args.item);
            }
        }

        /// <summary>
        /// Adds singleton prefabs in the form of tree view items to the root tree view item.
        /// </summary>
        /// <param name="root">The root tree view item.</param>
        /// <returns>Whether it was succesfull in adding them.</returns>
        private bool AddPrefabsToRoot(TreeViewItem root)
        {
            SingletonPrefab[] prefabs = SingletonProfileManager.Prefabs;

            if (prefabs.Length != 0)
            {
                for (int i = 0; i < prefabs.Length; i++)
                {
                    SingletonPrefab prefab = prefabs[i];
                    bool isEditorOnly = EditorSingletonValidator.IsSubClassOfEditorSingleton(prefab.singletonType);
                    ProfileTreeViewItem item = new ProfileTreeViewItem(isEditorOnly, false)
                    {
                        id = prefab.asset.GetInstanceID(),
                        depth = 0,
                        displayName = prefab.asset.name,
                    };

                    root.AddChild(item);
                }

                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Adds singleton scripts in the form of tree view items to the root tree view item.
        /// </summary>
        /// <param name="root">The root tree view item.</param>
        /// <returns>Whether it was succesfull in adding them.</returns>
        private bool AddScriptsToRoot(TreeViewItem root)
        {
            SingletonScript[] scripts = SingletonProfileManager.Scripts;

            if (scripts.Length != 0)
            {
                for (int i = 0; i < scripts.Length; i++)
                {
                    SingletonScript script = scripts[i];
                    bool isEditorOnly = EditorSingletonValidator.IsSubClassOfEditorSingleton(script.singletonType);
                    ProfileTreeViewItem item = new ProfileTreeViewItem(isEditorOnly, true)
                    {
                        id = script.asset.GetInstanceID(),
                        depth = 0,
                        displayName = script.singletonType.Name
                    };

                    root.AddChild(item);
                }

                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
        #endregion
    }
}

#endif