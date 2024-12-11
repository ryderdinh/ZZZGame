#if UNITY_EDITOR

using DTT.Utils.EditorUtilities;
using UnityEditor;
using UnityEngine;

namespace DTT.Singletons.Editor
{
    /// <summary>
    /// The styles used for drawing the singleton profile.
    /// </summary>
    public class ProfileStyles : GUIStyleCache
    {
        #region Variables
        #region Public
        /// <summary>
        /// The style for a refresh button.
        /// </summary>
        public GUIStyle ToolbarButton => base[nameof(ToolbarButton)];

        /// <summary>
        /// The style for an empty centered control.
        /// </summary>
        public GUIStyle EmptyCentered => base[nameof(EmptyCentered)];
        #endregion
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes the styles.
        /// </summary>
        public ProfileStyles()
        {
            Add(nameof(ToolbarButton), () =>
            {
                GUIStyle style = new GUIStyle(EditorStyles.toolbarButton);
                style.imagePosition = ImagePosition.ImageOnly;
                style.padding = new RectOffset(0, 0, 0, 1);
                return style;
            });

            Add(nameof(EmptyCentered), () =>
            {
                GUIStyle style = new GUIStyle(GUIStyle.none);
                style.alignment = TextAnchor.MiddleCenter;
                return style;
            });
        }
        #endregion
    }
}

#endif
