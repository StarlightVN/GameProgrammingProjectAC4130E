using UnityEngine;
using UnityEngine.EventSystems;

public class SmallDeskZone : MonoBehaviour, IDropHandler
{
    [Header("Kéo đối tượng đầu não _GameManager ngoài Hierarchy vào đây")]
    public SchoolGateManager gateManager;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject draggedObject = eventData.pointerDrag;
        if (draggedObject == null) return;

        // BẮT TRÚNG TÍN HIỆU KHI NGƯỜI CHƠI KÉO XẤP GIẤY TO THẢ VỀ BÀN NHỎ
        if (draggedObject.CompareTag("LargeCard"))
        {
            CardDisplay largeCardDisplay = draggedObject.GetComponent<CardDisplay>();
            if (largeCardDisplay != null && gateManager != null)
            {
                // 1. Tắt ẩn ngay xấp giấy to
                largeCardDisplay.gameObject.SetActive(false);

                // 2. Tự động dò tìm xem tờ giấy to nào đang hiển thị để cất đúng loại phôi mini
                DocumentType cattedDocType = DocumentType.MainCard;

                if (largeCardDisplay.gatePassGroup != null && largeCardDisplay.gatePassGroup.activeSelf)
                {
                    cattedDocType = DocumentType.GatePass;
                    largeCardDisplay.gatePassGroup.SetActive(false);
                }
                else if (largeCardDisplay.intlCertGroup != null && largeCardDisplay.intlCertGroup.activeSelf)
                {
                    cattedDocType = DocumentType.IntlCertificate;
                    largeCardDisplay.intlCertGroup.SetActive(false);
                }
                else if (largeCardDisplay.labCertGroup != null && largeCardDisplay.labCertGroup.activeSelf)
                {
                    cattedDocType = DocumentType.LabCertificate;
                    largeCardDisplay.labCertGroup.SetActive(false);
                }
                else
                {
                    if (largeCardDisplay.studentCardGroup != null) largeCardDisplay.studentCardGroup.SetActive(false);
                    if (largeCardDisplay.staffCardGroup != null) largeCardDisplay.staffCardGroup.SetActive(false);
                    cattedDocType = DocumentType.MainCard;
                }
                
                // 3. Ra lệnh hồi vị (Toạ độ rơi đã được xử lý tự động trong GameManager)
                gateManager.ReturnToSmallCard(Vector3.zero, cattedDocType);
            }
        }
    }
}