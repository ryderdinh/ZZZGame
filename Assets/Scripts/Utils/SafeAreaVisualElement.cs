using UnityEngine;
using UnityEngine.UIElements;

namespace Managers
{
    public class SafeAreaVisualElement : MonoBehaviour
    {
        [SerializeField] private bool ConformX = true; // Conform to screen safe area on X-axis (default true)
        [SerializeField] private bool ConformY = true; // Conform to screen safe area on Y-axis (default true)
        [SerializeField] private bool Logging; // Log safe area application

        private ScreenOrientation LastOrientation = ScreenOrientation.AutoRotation;
        private Rect LastSafeArea = new(0, 0, 0, 0);
        private Vector2Int LastScreenSize = new(0, 0);

        private VisualElement rootVisualElement; // Root visual element for UIBuilder

        private void Update()
        {
            Refresh();
        }

        private void OnEnable()
        {
            // Get the root visual element (from UI Document or manually)
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument != null) rootVisualElement = uiDocument.rootVisualElement;

            if (rootVisualElement == null)
            {
                Debug.LogError("No VisualElement found to apply safe area.");
                return;
            }

            Refresh();
        }

        private void Refresh()
        {
            var safeArea = GetSafeArea();

            if (safeArea != LastSafeArea
                || Screen.width != LastScreenSize.x
                || Screen.height != LastScreenSize.y
                || Screen.orientation != LastOrientation)
            {
                LastScreenSize.x = Screen.width;
                LastScreenSize.y = Screen.height;
                LastOrientation = Screen.orientation;

                ApplySafeArea(safeArea);
            }
        }

        private Rect GetSafeArea()
        {
            var safeArea = Screen.safeArea;
            return safeArea;
        }

        private void ApplySafeArea(Rect r)
        {
            LastSafeArea = r;

            // Ignore x-axis?
            if (!ConformX)
            {
                r.x = 0;
                r.width = Screen.width;
            }

            // Ignore y-axis?
            if (!ConformY)
            {
                r.y = 0;
                r.height = Screen.height;
            }

            // Calculate margins from the safe area
            var leftMargin = r.x / Screen.width;
            var rightMargin = (Screen.width - r.xMax) / Screen.width;
            var topMargin = (Screen.height - r.yMax) / Screen.height;
            var bottomMargin = r.y / Screen.height;

            // Apply margins to the root visual element
            rootVisualElement.style.marginLeft = new Length(leftMargin * 100, LengthUnit.Percent);
            rootVisualElement.style.marginRight = new Length(rightMargin * 100, LengthUnit.Percent);
            rootVisualElement.style.marginTop = new Length(topMargin * 100, LengthUnit.Percent);
            rootVisualElement.style.marginBottom = new Length(bottomMargin * 100, LengthUnit.Percent);

            if (Logging)
                Debug.LogFormat("New safe area margins applied: left={0}, right={1}, top={2}, bottom={3}",
                    leftMargin, rightMargin, topMargin, bottomMargin);
        }
    }
}