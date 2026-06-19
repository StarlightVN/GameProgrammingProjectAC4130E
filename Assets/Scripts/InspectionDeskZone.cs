using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class InspectionDeskZone : MonoBehaviour, IDropHandler
{
    [Header("Kéo đối tượng đầu não _GameManager ngoài Hierarchy vào đây")]
    public SchoolGateManager gateManager;

    [Header("Rulebook Display")]
    public RulebookDisplay largeBookDisplay;

    [Header("Newspaper Display (TỰ TRỊ ĐỘC LẬP Y XÌ ĐÚC RULEBOOK)")]
    [Tooltip("Kéo đối tượng NewspaperLarge standalone ngoài Hierarchy vào đây")]
    public NewspaperDisplay largeNewspaperDisplay;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject draggedObject = eventData.pointerDrag;
        if (draggedObject == null) return;

        // =========================================================================
        // NHÁNH 1: XỬ LÝ KHI NGƯỜI CHƠI THẢ PHÔI GIẤY TỜ MINI XUỐNG BÀN LỚN
        // =========================================================================
        if (draggedObject.CompareTag("SmallCard"))
        {
            CardDisplay smallCardDisplay = draggedObject.GetComponent<CardDisplay>();
            if (smallCardDisplay != null && gateManager != null)
            {
                GameObject targetLargeObject = null;

                // Radar quét phân loại tài liệu nhỏ để lấy chính xác thực thể thẻ to standalone tương ứng từ GameManager
                switch (smallCardDisplay.smallCardDocType)
                {
                    case DocumentType.MainCard:
                        if (gateManager.GetCurrentProfile() != null)
                        {
                            if (gateManager.GetCurrentProfile().personType == PersonType.Student)
                                targetLargeObject = gateManager.largeStudentCardObject;
                            else if (gateManager.GetCurrentProfile().personType == PersonType.Staff)
                                targetLargeObject = gateManager.largeStaffCardObject;
                        }
                        break;

                    case DocumentType.GatePass:
                        targetLargeObject = gateManager.largeGatePassObject;
                        break;

                    case DocumentType.IntlCertificate:
                        targetLargeObject = gateManager.largeIntlCertObject;
                        break;

                    case DocumentType.LabCertificate:
                        targetLargeObject = gateManager.largeLabCertObject;
                        break;
                }

                // Kích hoạt bật đối tượng thẻ lớn độc lập tương ứng ngay tại vị trí chuột buông tay
                if (targetLargeObject != null)
                {
                    RectTransform smallRect = draggedObject.GetComponent<RectTransform>();
                    RectTransform largeRect = targetLargeObject.GetComponent<RectTransform>();
                    
                    // Đồng bộ tọa độ xuất hiện bám sát theo vị trí chuột buông tay
                    if (largeRect != null && smallRect != null)
                    {
                        largeRect.position = smallRect.position; 
                        largeRect.rotation = Quaternion.identity; 
                    }

                    // Bật thực thể lớn lên màn hình (Nó sẽ tự kích hoạt OnEnable để nạp chữ)
                    targetLargeObject.SetActive(true);
                    UIDragDrop largeCardDrag = targetLargeObject.GetComponent<UIDragDrop>();
                    if (largeCardDrag != null)
                    {
                        largeCardDrag.ClampToBoundary(); // Khóa cứng thẻ to lọt vào vùng an toàn ngay lập tức!
                    }
                    
                    // Giải phóng tia va chạm chuột để kéo rê mượt mà không bị đơ mờ
                    if (targetLargeObject.TryGetComponent<CanvasGroup>(out CanvasGroup largeCardCG))
                    {
                        largeCardCG.alpha = 1f;
                        largeCardCG.blocksRaycasts = true; 
                    }

                    // Đẩy thứ tự layer hiển thị lên trên cùng bề mặt bàn làm việc
                    targetLargeObject.transform.SetAsLastSibling(); 



                    // Tiêu hủy chính xác mảnh phôi nhỏ vừa kéo lên, giữ sạch rác mặt bàn gỗ
                    Destroy(draggedObject); 
                }
            }
        }
        // =========================================================================
        // NHÁNH 2: XỬ LÝ SỔ TRA CỨU QUY TẮC
        // =========================================================================
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

                // 🔥 ĐÃ SỬA LỖI TYPO: Chuyển đổi chính xác thành Component UIDragDrop hợp lệ
                UIDragDrop largeBookDrag = largeBookDisplay.GetComponent<UIDragDrop>();
                if (largeBookDrag != null && largeRect != null)
                {
                    largeBookDrag.SetStablePosition(largeRect.position);
                }

                Destroy(draggedObject);
            }
        }
        // =========================================================================
        // NHÁNH 3: XỬ LÝ LẬT MỞ TỜ BÁO LỚN
        // =========================================================================
        else if (draggedObject.CompareTag("SmallNewspaper"))
        {
            if (largeNewspaperDisplay != null)
            {
                RectTransform smallRect = draggedObject.GetComponent<RectTransform>();
                RectTransform largeRect = largeNewspaperDisplay.GetComponent<RectTransform>();

                // Đồng bộ hồng tâm xuất hiện của Báo lớn bám khít theo vị trí chuột buông tay
                largeRect.position = smallRect.position;
                largeRect.rotation = Quaternion.identity; 

                // Kích hoạt bật tờ báo lớn lên
                largeNewspaperDisplay.gameObject.SetActive(true);

                // Ép nổi sáng rõ và nhận tia chuột
                if (largeNewspaperDisplay.TryGetComponent<CanvasGroup>(out CanvasGroup largeNewsCG))
                {
                    largeNewsCG.alpha = 1f;
                    largeNewsCG.blocksRaycasts = true; 
                }

                // Đẩy hiển thị đè lên trên đỉnh bàn trực
                largeNewspaperDisplay.transform.SetAsLastSibling();

                // Lưu điểm neo ổn định cho bộ kéo thả của Báo lớn
                UIDragDrop largeNewsDrag = largeNewspaperDisplay.GetComponent<UIDragDrop>();
                if (largeNewsDrag != null && largeRect != null)
                {
                    largeNewsDrag.SetStablePosition(largeRect.position);
                }

                // Tiêu hủy phôi báo nhỏ, dọn sạch rác không gian soi phẳng
                Destroy(draggedObject);
            }
        }
    }
}