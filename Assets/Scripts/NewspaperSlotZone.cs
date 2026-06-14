using UnityEngine;
using UnityEngine.EventSystems;

public class NewspaperSlotZone : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject draggedObject = eventData.pointerDrag;
        if (draggedObject == null) return;

        // =========================================================================
        // 🔥 LOGIC TỰ TRỊ TUYỆT ĐỐI: CHỈ ĐÓN NHẬN DUY NHẤT TỜ BÁO LỚN THẢ VỀ
        // Không khai báo Manager, không làm phiền đến CardDisplay hay SchoolGateManager
        // =========================================================================
        if (draggedObject.CompareTag("LargeNewspaper"))
        {
            NewspaperDisplay newspaperDisplay = draggedObject.GetComponent<NewspaperDisplay>();
            if (newspaperDisplay != null)
            {
                // Gọi hàm đóng báo tự sinh phôi nhỏ tích hợp sẵn bên trong chính nó
                newspaperDisplay.CloseNewspaper();
            }
        }
    }
}