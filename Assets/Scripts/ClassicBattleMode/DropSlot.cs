using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClassicBattleMode
{
    public class DropSlot : MonoBehaviour, IDropHandler
    {
        public RectTransform gridParent; // Parent chứa tất cả các DropSlot (ví dụ như Grid Layout)

        public void OnDrop(PointerEventData eventData)
        {
            var droppedItem = eventData.pointerDrag; // Lấy item đang kéo
            if (droppedItem != null)
            {
                var droppedRectTransform = droppedItem.GetComponent<RectTransform>();

                // Tìm tất cả các DropSlot (cells) trong grid
                var allSlots = gridParent.GetComponentsInChildren<DropSlot>();

                // Biến để lưu DropSlot gần nhất
                DropSlot nearestSlot = null;
                var minDistance = float.MaxValue;

                // Tìm DropSlot gần vị trí item được thả nhất
                foreach (var slot in allSlots)
                {
                    var slotposition = slot.GetComponent<RectTransform>();
                    // Tính khoảng cách giữa item và từng DropSlot
                    var distance = Vector2.Distance(droppedRectTransform.anchoredPosition, slotposition.anchoredPosition);

                    // Nếu khoảng cách nhỏ hơn khoảng cách nhỏ nhất đã lưu, cập nhật lại DropSlot gần nhất
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestSlot = slot;
                    }
                }
                var ass = nearestSlot.GetComponent<RectTransform>();
                // Nếu khoảng cách đủ nhỏ, item sẽ được đặt vào DropSlot gần nhất
                if (nearestSlot != null && minDistance < 50f) // 50f là ngưỡng khoảng cách, bạn có thể điều chỉnh
                    droppedRectTransform.anchoredPosition = ass.anchoredPosition; // Đặt item vào vị trí của DropSlot
            }
        }

       
    }
}