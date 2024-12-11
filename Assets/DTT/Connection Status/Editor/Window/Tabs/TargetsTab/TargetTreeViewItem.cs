#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace DTT.Networking.ConnectionStatus.Editor
{
    /// <summary>
    /// Represents an internet status target drawn in the targets tree view.
    /// </summary>
    internal class TargetTreeViewItem : TreeViewItem
    {
        /// <summary>
        /// Whether this is the default target.
        /// </summary>
        public readonly bool isDefault;
        
        /// <summary>
        /// The serialized properties of the internet status target.
        /// </summary>
        public readonly InternetStatusTargetProperties properties;

        /// <summary>
        /// Initializes the item with an id and the target's serialized property.
        /// </summary>
        /// <param name="id">The id of the item.</param>
        /// <param name="property">The serialized property.</param>
        /// <param name="isDefault">Whether this is the default target.</param>
        public TargetTreeViewItem(int id, SerializedProperty property, bool isDefault): base(id, 0)
        {
            properties = new InternetStatusTargetProperties(property);

            this.isDefault = isDefault;
            
            displayName = properties.name.stringValue;
        }
    }
}

#endif