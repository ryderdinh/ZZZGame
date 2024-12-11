#if UNITY_EDITOR

using DTT.PublishingTools;
using DTT.Utils.EditorUtilities;
using UnityEditor;
using UnityEngine;

namespace DTT.Networking.ConnectionStatus.Editor
{
    /// <summary>
    /// The styles used for drawing the <see cref="InternetStatusWindow"/>.
    /// </summary>
    internal class InternetStatusWindowStyles : GUIStyleCache
    {
        /// <summary>
        /// The styles used for the address label.
        /// </summary>
        public GUIStyle AddressLabel => base[nameof(AddressLabel)];
        
        /// <summary>
        /// The styles used for the status label.
        /// </summary>
        public GUIStyle StatusLabel => base[nameof(StatusLabel)];
        
        /// <summary>
        /// The text color for a reconnect status.
        /// </summary>
        private readonly Color reconnectColor = new Color32(255, 165, 0, 255);

        /// <summary>
        /// The text color for an offline status.
        /// </summary>
        private readonly Color offlineColor = new Color32(235, 84, 64,255);

        /// <summary>
        /// The text color for an online status.
        /// </summary>
        private readonly Color onlineColor = new Color32(0, 150, 0, 255);

        /// <summary>
        /// Initializes the styles.
        /// </summary>
        internal InternetStatusWindowStyles()
        {
            Add(nameof(AddressLabel), () =>
            {
                GUIStyle style = new GUIStyle(EditorStyles.label);
                style.alignment = TextAnchor.MiddleRight;
                style.padding.right = 10;
                return style;
            });

            Add(nameof(StatusLabel), () =>
            {
                GUIStyle style = new GUIStyle(EditorStyles.label);
                style.fontStyle = FontStyle.Bold;
                return style;
            });
        }

        /// <summary>
        /// Sets the status color based on an internet status.
        /// </summary>
        /// <param name="status">The status to base the color on.</param>
        public void SetStatusColor(InternetStatus status)
        {
            switch (status)
            {
                case InternetStatus.ONLINE:
                    StatusLabel.normal.textColor = onlineColor;
                    break;
                
                case InternetStatus.RECONNECTING:
                    StatusLabel.normal.textColor = reconnectColor;
                    break;
                    
                case InternetStatus.OFFLINE:
                    StatusLabel.normal.textColor = offlineColor;
                    break;
                
                default:
                    StatusLabel.normal.textColor = EditorStyles.label.normal.textColor;
                    break;
            }
        }
    }
}

#endif