using UnityEngine;
using UnityEngine.UIElements;

public class AdjustUIToParent : MonoBehaviour
{
    public UIDocument uiDocument;
    private VisualElement rootVisualElement;

    private void Start()
    {
        // Lấy root của UIDocument
        rootVisualElement = uiDocument.rootVisualElement;

        // Gọi hàm cập nhật kích thước lần đầu
        UpdateSize();

        // Đăng ký sự kiện thay đổi kích thước
        rootVisualElement.RegisterCallback<GeometryChangedEvent>(evt => { UpdateSize(); });
    }

    private void UpdateSize()
    {
        // Lấy RectTransform của GameObject cha
        var parentRectTransform = GetComponentInParent<RectTransform>();

        if (parentRectTransform != null)
        {
            // Điều chỉnh kích thước VisualElement theo kích thước của RectTransform cha
            rootVisualElement.style.width = new Length(parentRectTransform.rect.width, LengthUnit.Pixel);
            rootVisualElement.style.height = new Length(parentRectTransform.rect.height, LengthUnit.Pixel);
        }
        else
        {
            Debug.LogWarning("Không tìm thấy RectTransform của phần tử cha.");
        }
    }
}