using UnityEngine;
using UnityEngine.EventSystems;

public class RulebookSlotZone : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject draggedObject = eventData.pointerDrag;
        if (draggedObject == null) return;

        UIDragDrop dragScript = draggedObject.GetComponent<UIDragDrop>();
        if (dragScript == null) return;

        // KHI KÉO SỔ LỚN THẢ THẲNG VỀ KHAY HỒNG HOẶC BÀN NHỎ CHUYỂN TIẾP
        if (draggedObject.CompareTag("LargeBook"))
        {
            RulebookDisplay largeBookDisplay = draggedObject.GetComponent<RulebookDisplay>();
            if (largeBookDisplay != null)
            {
                // Kích hoạt thẳng hàm CloseBook() để nó tự Destroy sổ lớn và Instantiate sổ nhỏ mới tinh vào khay hồng
                largeBookDisplay.CloseBook();
            }
        }
    }
}