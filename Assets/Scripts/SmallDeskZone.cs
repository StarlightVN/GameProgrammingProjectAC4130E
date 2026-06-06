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

        if (draggedObject.CompareTag("LargeCard"))
        {
            CardDisplay largeCardDisplay = draggedObject.GetComponent<CardDisplay>();
            if (largeCardDisplay != null && gateManager != null)
            {
                // 1. Tắt ẩn xấp giấy to
                largeCardDisplay.gameObject.SetActive(false);

                // 2. Dò tìm loại giấy tờ đang hiển thị để cất
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
                
                // 3. KHẮC PHỤC LỖI: Lấy chính xác vị trí chuột của sự kiện UI thả
                // và bắn sang cho GameManager điều phối
                gateManager.ReturnToSmallCard(eventData.position, cattedDocType);
            }
        }
    }
}