using UnityEngine;
using UnityEngine.EventSystems;

public class SmallDeckZone : MonoBehaviour, IDropHandler
{
    [Header("Chuyển tiếp cho Sổ Quy Tắc")]
    public RulebookSlotZone rulebookSlotZone; 

    [Header("School Gate Manager Reference")]
    public SchoolGateManager gateManager; 

    public void OnDrop(PointerEventData eventData)
    {
        GameObject draggedObject = eventData.pointerDrag;
        if (draggedObject == null) return;

        UIDragDrop dragScript = draggedObject.GetComponent<UIDragDrop>();
        if (dragScript == null) return;

        if (draggedObject.CompareTag("LargeCard"))
        {
            StudentCardDisplay largeCardDisplay = draggedObject.GetComponent<StudentCardDisplay>();
            if (largeCardDisplay != null && gateManager != null)
            {
                // Gọi đầu não xử lý và chuyển giao tọa độ chuột buông
                gateManager.ReturnToSmallCard(draggedObject.GetComponent<RectTransform>().position);
            }
        }
        else if (draggedObject.CompareTag("LargeBook"))
        {
            if (rulebookSlotZone != null)
            {
                rulebookSlotZone.OnDrop(eventData);
            }
        }
    }
}