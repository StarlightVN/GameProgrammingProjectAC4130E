using UnityEngine;
using UnityEngine.EventSystems;

public class SmallDeskZone : MonoBehaviour, IDropHandler
{
    [Header("Kéo đối tượng đầu não _GameManager ngoài Hierarchy vào đây")]
    public SchoolGateManager gateManager;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        // Kiểm tra xem vật thể được thả có mang script UIDragDrop không
        UIDragDrop dragDrop = eventData.pointerDrag.GetComponent<UIDragDrop>();
        if (dragDrop == null) return;

        // Nếu người chơi thả phôi nhỏ bệt sẵn ở bàn dưới thì bỏ qua không xử lý chồng chéo
        if (dragDrop.isSmallCard)
        {
            return;
        }

        // BẮT TRÚNG TÍN HIỆU KHI NGƯỜI CHƠI KÉO XẤP GIẤY TO STANDALONE THẢ VỀ BÀN NHỎ
        CardDisplay largeDisplay = eventData.pointerDrag.GetComponent<CardDisplay>();
        if (largeDisplay != null && gateManager != null)
        {
            // 1. Tắt ẩn ngay chính chiếc thẻ lớn độc lập đang bị kéo thả này
            eventData.pointerDrag.SetActive(false);

            // 2. Dò tìm phân loại thẻ lớn tự trị (thisCardType) để quyết định hồi sinh mảnh phôi nhỏ mini nào
            DocumentType cattedDocType = DocumentType.MainCard;

            switch (largeDisplay.thisCardType)
            {
                case CardDisplayType.StudentCard:     cattedDocType = DocumentType.MainCard; break;
                case CardDisplayType.StaffCard:       cattedDocType = DocumentType.MainCard; break;
                case CardDisplayType.GatePass:         cattedDocType = DocumentType.GatePass; break;
                case CardDisplayType.IntlCertificate: cattedDocType = DocumentType.IntlCertificate; break;
                case CardDisplayType.LabCertificate:  cattedDocType = DocumentType.LabCertificate; break;
            }
            
            // 3. Ra lệnh GameManager tái sinh phôi nhỏ sạch rác nằm ngay ngắn trên mặt bàn gỗ dưới
            gateManager.ReturnToSmallCard(Vector3.zero, cattedDocType);
        }
    }
}