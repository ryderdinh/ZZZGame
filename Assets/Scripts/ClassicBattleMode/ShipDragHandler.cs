using UnityEngine;
using UnityEngine.EventSystems;

public class ShipDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGroup;

    private Vector2 originalPosition; // Vị trí gốc của tàu
    private RectTransform rectTransform;
    
    private Collider2D[] hitSlots;
    public LayerMask slotLayer; // Các ô inventory đang chạm vào item

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition; // Lưu lại vị trí gốc
        canvasGroup.alpha = 0.6f; // Làm mờ tàu khi đang kéo
        canvasGroup.blocksRaycasts = false; // Cho phép tàu đi qua các UI khác khi kéo
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Di chuyển tàu theo con trỏ chuột (theo vị trí trên Canvas)
        transform.position = Input.mousePosition;
    }
    
    public void OnCollisionEnter2D(Collision other)
    {
        if (other.)
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Bạn có thể bổ sung thêm điều kiện nếu muốn đặt lại tàu khi thả không hợp lệ
        // Ví dụ, nếu không đặt đúng lưới thì quay lại vị trí ban đầu
    }
}