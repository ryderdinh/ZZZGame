#if UNITY_EDITOR

using DTT.Utils.EditorUtilities;
using UnityEditor;

namespace DTT.Networking.ConnectionStatus.Editor
{
    /// <summary>
    /// Represents the serialized properties of the <see cref="InternetStatusTarget"/> class.
    /// </summary>
    internal class InternetStatusTargetProperties : RelativePropertyCache
    {
        /// <summary>
        /// The name property.
        /// </summary>
        public SerializedProperty name => base[nameof(name)];

        /// <summary>
        /// The current status property.
        /// </summary>
        public SerializedProperty currentStatus => base[nameof(currentStatus)];
        
        /// <summary>
        /// The use status override property.
        /// </summary>
        public SerializedProperty usesStatusOverride => base[nameof(usesStatusOverride)];

        /// <summary>
        /// The status override property.
        /// </summary>
        public SerializedProperty statusOverride => base[nameof(statusOverride)];

        /// <summary>
        /// The adress property.
        /// </summary>
        public SerializedProperty address => base[nameof(address)];

        /// <summary>
        /// The ping interval property.
        /// </summary>
        public SerializedProperty pingInterval => base[nameof(pingInterval)];

        /// <summary>
        /// The max reconnect duration property.
        /// </summary>
        public SerializedProperty maxReconnectDuration => base[nameof(maxReconnectDuration)];
        
        /// <summary>
        /// Initializes the properties.
        /// </summary>
        /// <param name="serializedProperty">The serialized internet status target property.</param>
        public InternetStatusTargetProperties(SerializedProperty serializedProperty) : base(serializedProperty) { }

        /// <summary>
        /// Copies values from these serialized properties to another one and applies the changes.
        /// </summary>
        /// <param name="properties">The properties to copy values to.</param>
        public void CopyTo(InternetStatusTargetProperties properties)
        {
            properties.UpdateObjectRepresentation();
            
            properties.name.stringValue = name.stringValue;
            properties.address.stringValue = address.stringValue;
            properties.currentStatus.enumValueIndex = currentStatus.enumValueIndex;
            properties.usesStatusOverride.boolValue = usesStatusOverride.boolValue;
            properties.statusOverride.enumValueIndex = statusOverride.enumValueIndex;
            properties.pingInterval.floatValue = pingInterval.floatValue;
            properties.maxReconnectDuration.floatValue = maxReconnectDuration.floatValue;
            
            properties.ApplyChangesToObject();
        }
    }
}

#endif