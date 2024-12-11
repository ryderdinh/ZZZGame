#if UNITY_EDITOR

using DTT.PublishingTools;
using DTT.Utils.EditorUtilities;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace  DTT.Networking.ConnectionStatus.Editor
{
    /// <summary>
    /// Draws target tree view items on the screen in a tree view format.
    /// </summary>
    internal class TargetsTreeView : TreeView
    {
        /// <summary>
        /// The window state.
        /// </summary>
        private InternetStatusWindowState _windowState;

        /// <summary>
        /// The default target name.
        /// </summary>
        private const string DEFAULT_TARGET_NAME = "target";

        /// <summary>
        /// The default target address.
        /// </summary>
        private const string DEFAULT_TARGET_ADDRESS = "0.0.0.0";
        
        /// <summary>
        /// Initializes the tree view.
        /// </summary>
        /// <param name="windowState">The window state.</param>
        public TargetsTreeView(InternetStatusWindowState windowState) : base(windowState.TreeState)
        {
            showBorder = true;
            showAlternatingRowBackgrounds = true;

            _windowState = windowState;
            
            Reload();
        }

        /// <summary>
        /// Initializes the tree view with stored targets.
        /// </summary>
        /// <returns>The tree view root item.</returns>
        protected override TreeViewItem BuildRoot()
        {
            TreeViewItem root = new TreeViewItem(-1, -1, "ROOT");

            if (_windowState.PrefabWorkerProperties.HasTargets)
            {
                int defaultTargetIndex = _windowState.PrefabWorkerProperties.defaultTargetIndex.intValue;
                SerializedProperty targetsList = _windowState.PrefabWorkerProperties.targets;
                for (int i = 0; i < targetsList.arraySize; i++)
                {
                    bool isDefault = i == defaultTargetIndex;
                    SerializedProperty element = targetsList.GetArrayElementAtIndex(i);
                    root.AddChild(new TargetTreeViewItem(i, element, isDefault));
                }
            }
            else
            {
                root.AddChild(new TreeViewItem(0,0, "EMPTY"));
            }
            
            return root;
        }

        /// <summary>
        /// Draws the row gui for a tree view item.
        /// </summary>
        /// <param name="args">The arguments for drawing the row.</param>
        protected override void RowGUI(RowGUIArgs args)
        {
            if (args.selected)
                EditorGUI.DrawRect(args.rowRect, DTTColors.light.line);
            
            TargetTreeViewItem item = (TargetTreeViewItem) args.item;
            Rect rect = args.rowRect;

            string displayName = item.displayName;
            if (item.isDefault)
                displayName = displayName.Insert(0, "(default) ");
            
            float halfWidth = rect.width * 0.5f;
            Rect nameRect = new Rect(rect.x, rect.y, halfWidth, rect.height);
            GUI.Label(nameRect, displayName);

            Rect addressRect = new Rect(rect.x + halfWidth, rect.y, halfWidth, rect.height);
            GUI.Label(addressRect, item.properties.address.stringValue, _windowState.Styles.AddressLabel);
        }

        /// <summary>
        /// Shows a context menu for a target tree view item.
        /// </summary>
        /// <param name="id">The id of the clicked item.</param>
        protected override void ContextClickedItem(int id)
        {
            // We can't modify the handler using the context menu during playmode.
            if (EditorApplication.isPlaying)
                return;
            
            Event.current.Use();
            
            TargetTreeViewItem item = (TargetTreeViewItem)FindItem(id, rootItem);
            ContextDropdownBuilder builder = new ContextDropdownBuilder(null);

            if (!item.isDefault)
            {
                builder.AddItem("Set Default", () => SetDefaultTarget(id));
                builder.AddSeparator();
            }

            // The default target can't be removed.
            if (item.properties.name.stringValue != InternetStatusTarget.DEFAULT_NAME)
                builder.AddItem("Remove", () => RemoveTarget(id));
            
            builder.GetResult().Show();
        }

        /// <summary>
        /// Called when the user right-clicks the tree view. It shows a context menu to add a target.
        /// </summary>
        protected override void ContextClicked() 
        {
            // We can't modify the handler using the context menu during playmode.
            if (EditorApplication.isPlaying)
                return;
            
            new ContextDropdownBuilder(null)
                .AddItem("Add Target", AddTarget)
                .GetResult()
                .Show();
        }

        /// <summary>
        /// Called when an item is double clicked to set the selected target index.
        /// </summary>
        /// <param name="id">The item id.</param>
        protected override void DoubleClickedItem(int id) => _windowState.SelectedTargetIndex = id;

        /// <summary>
        /// Sets the default target of the internet status handler.
        /// </summary>
        /// <param name="newDefaultTargetIndex">The new default target index.</param>
        private void SetDefaultTarget(int newDefaultTargetIndex)
        {
            _windowState.PrefabWorkerProperties.defaultTargetIndex.intValue = newDefaultTargetIndex;
            _windowState.PrefabWorkerProperties.ApplyChanges();
            
            Reload();
        }

        /// <summary>
        /// Removes a target from the list of targets.
        /// </summary>
        /// <param name="targetId">The id of the target to remove.</param>
        private void RemoveTarget(int targetId)
        {
            SerializedProperty targetsList = _windowState.PrefabWorkerProperties.targets;
            targetsList.DeleteArrayElementAtIndex(targetId);
            
            _windowState.EnsureCorrectDefaultTargetBounds();
            _windowState.SelectedTargetIndex = targetsList.arraySize - 1;
            _windowState.PrefabWorkerProperties.ApplyChanges();

            Reload();
        }

        /// <summary>
        /// Adds a new internet status target to the tree view and handler.
        /// </summary>
        private void AddTarget()
        {
            SerializedProperty targetsList = _windowState.PrefabWorkerProperties.targets;
            targetsList.arraySize++;

            SerializedProperty newProperty = targetsList.GetArrayElementAtIndex(targetsList.arraySize - 1);
            InternetStatusTargetProperties properties = new InternetStatusTargetProperties(newProperty);
            properties.name.stringValue = DEFAULT_TARGET_NAME;
            properties.address.stringValue = DEFAULT_TARGET_ADDRESS;
            properties.pingInterval.floatValue = InternetStatusTarget.DEFAULT_PING_INTERVAL;
            properties.maxReconnectDuration.floatValue = InternetStatusTarget.DEFAULT_MAX_RECONNECT_DURATION;
            
            _windowState.SelectedTargetIndex = targetsList.arraySize - 1;
            _windowState.PrefabWorkerProperties.ApplyChanges();

            Reload();
        }
    }
}

#endif
