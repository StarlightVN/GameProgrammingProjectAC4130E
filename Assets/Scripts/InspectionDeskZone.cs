using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class InspectionDeskZone : MonoBehaviour, IDropHandler
{
    [Header("Card Display (Kéo đối tượng cha CardGroup tổng ngoài Hierarchy vào đây)")]
    public CardDisplay largeCardDisplay;

    [Header("Rulebook Display")]
    public RulebookDisplay largeBookDisplay;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject draggedObject = eventData.pointerDrag;
        if (draggedObject == null) return;

        // XỬ LÝ KHI NGƯỜI CHƠI THẢ PHÔI GIẤY TỜ MINI XUỐNG BÀN LỚN
        if (draggedObject.CompareTag("SmallCard"))
        {
            CardDisplay smallCardDisplay = draggedObject.GetComponent<CardDisplay>();
            if (smallCardDisplay != null && largeCardDisplay != null)
            {
                // 1. Chuyển giao tham chiếu hồ sơ PersonProfile từ túi chứa thẻ nhỏ sang xấp lớn
                largeCardDisplay.currentProfile = smallCardDisplay.currentProfile;

                RectTransform smallRect = draggedObject.GetComponent<RectTransform>();
                RectTransform largeRect = largeCardDisplay.GetComponent<RectTransform>();
                
                // Đồng bộ tọa độ xuất hiện bám sát theo vị trí chuột buông tay
                largeRect.position = smallRect.position; 
                largeRect.rotation = Quaternion.identity; 

                // 2. Kích hoạt bật cụm Panel cha CardGroup tổng lên
                largeCardDisplay.gameObject.SetActive(true);
                
                // Bộ lọc an toàn giải phóng tia va chạm chuột để kéo rê lần 2 không bị mờ đơ
                if (largeCardDisplay.TryGetComponent<CanvasGroup>(out CanvasGroup largeCardCG))
                {
                    largeCardCG.alpha = 1f;
                    largeCardCG.blocksRaycasts = true; 
                }

                // 3. Ép xấp lớn đổ toàn bộ dữ liệu chữ vào các linh kiện TMP ẩn bên dưới
                largeCardDisplay.RenderCardData();

                // 4. CƠ CHẾ KIỂM SOÁT LẬT MỞ TỪNG TỜ:
                // Quét thông tin loại phôi kéo lên để bật kích hoạt DUY NHẤT tờ giấy to tương ứng
                switch (smallCardDisplay.smallCardDocType)
                {
                    case DocumentType.MainCard:
                        if (largeCardDisplay.currentProfile.personType == PersonType.Student && largeCardDisplay.studentCardGroup != null)
                            largeCardDisplay.studentCardGroup.SetActive(true);
                        else if (largeCardDisplay.currentProfile.personType == PersonType.Staff && largeCardDisplay.staffCardGroup != null)
                            largeCardDisplay.staffCardGroup.SetActive(true);
                        break;

                    case DocumentType.GatePass:
                        if (largeCardDisplay.gatePassGroup != null) 
                            largeCardDisplay.gatePassGroup.SetActive(true);
                        break;

                    case DocumentType.IntlCertificate:
                        if (largeCardDisplay.intlCertGroup != null) 
                            largeCardDisplay.intlCertGroup.SetActive(true);
                        break;

                    case DocumentType.LabCertificate:
                        if (largeCardDisplay.labCertGroup != null) 
                            largeCardDisplay.labCertGroup.SetActive(true);
                        break;
                }

                // Đẩy layer vật thể vừa sinh lên trên cùng bề mặt bàn làm việc
                largeCardDisplay.transform.SetAsLastSibling(); 

                UIDragDrop largeCardDrag = largeCardDisplay.GetComponent<UIDragDrop>();
                if (largeCardDrag != null)
                {
                    largeCardDrag.SetStablePosition(largeRect.position);
                }

                // 5. Tiêu hủy chính xác mảnh phôi nhỏ vừa kéo lên, giữ lại các phôi khác ở bàn dưới
                Destroy(draggedObject); 
            }
        }
        // XỬ LÝ SỔ TRA CỨU QUY TẮC
        else if (draggedObject.CompareTag("SmallBook"))
        {
            if (largeBookDisplay != null)
            {
                RectTransform smallRect = draggedObject.GetComponent<RectTransform>();
                RectTransform largeRect = largeBookDisplay.GetComponent<RectTransform>();

                largeRect.position = smallRect.position;
                largeRect.rotation = Quaternion.identity; 

                largeBookDisplay.gameObject.SetActive(true);

                if (largeBookDisplay.TryGetComponent<CanvasGroup>(out CanvasGroup largeBookCG))
                {
                    largeBookCG.alpha = 1f;
                    largeBookCG.blocksRaycasts = true; 
                }

                largeBookDisplay.transform.SetAsLastSibling();

                UIDragDrop largeBookDrag = largeBookDisplay.GetComponent<UIDragDrop>();
                if (largeBookDrag != null)
                {
                    largeBookDrag.SetStablePosition(largeRect.position);
                }

                Destroy(draggedObject);
            }
        }
    }
}