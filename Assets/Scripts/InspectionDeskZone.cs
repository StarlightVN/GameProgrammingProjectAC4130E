using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class InspectionDeskZone : MonoBehaviour, IDropHandler
{
    [Header("Student Card Display (Kéo đối tượng StudentCard_Large vào đây)")]
    public StudentCardDisplay largeCardDisplay;

    [Header("Rulebook Display")]
    public RulebookDisplay largeBookDisplay;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject draggedObject = eventData.pointerDrag;
        if (draggedObject == null) return;

        // ĐÃ XÓA DÒNG LỆNH GÂY LỖI NGẮT LUỒNG: dragScript.enabled = false;

        // XỬ LÝ KHI THẢ THẺ SINH VIÊN MINI LÊN BÀN LỚN
        if (draggedObject.CompareTag("SmallCard"))
        {
            StudentCardDisplay smallCardDisplay = draggedObject.GetComponent<StudentCardDisplay>();
            if (smallCardDisplay != null && largeCardDisplay != null)
            {
                // 1. LẤY PROFILE TỪ CHIẾC TÚI CHỨA CỦA THẺ NHỎ TRUYỀN SANG THẺ TO
                largeCardDisplay.currentProfile = smallCardDisplay.currentProfile;

                RectTransform smallRect = draggedObject.GetComponent<RectTransform>();
                RectTransform largeRect = largeCardDisplay.GetComponent<RectTransform>();
                
                // 2. Đồng bộ vị trí xuất hiện trùng khớp tọa độ chuột thả
                largeRect.position = smallRect.position; 
                largeRect.rotation = Quaternion.identity; 

                // 3. Kích hoạt thẻ lớn
                largeCardDisplay.gameObject.SetActive(true);
                
                // KHẮC PHỤC LỖI KHÓA CỨNG: Giải phóng hoàn toàn tia chuột cho Thẻ lớn ngay khi vừa xuất hiện
                CanvasGroup largeCardCG = largeCardDisplay.GetComponent<CanvasGroup>() ?? largeCardDisplay.gameObject.AddComponent<CanvasGroup>();
                largeCardCG.alpha = 1f;
                largeCardCG.blocksRaycasts = true; // Ép bật sẵn va chạm để nhận diện click lần 2 mượt mà

                // 4. ÉP THẺ LỚN ĐỌC DATA VÀ VẼ CHỮ LÊN TEXTMESHPRO
                largeCardDisplay.RenderCardData();
                largeCardDisplay.transform.SetAsLastSibling(); // Đè lớp hiển thị lên trên cùng

                UIDragDrop largeCardDrag = largeCardDisplay.GetComponent<UIDragDrop>();
                if (largeCardDrag != null)
                {
                    largeCardDrag.SetStablePosition(largeRect.position);
                }

                // 5. Tiêu hủy chiếc thẻ nhỏ trơ trọi đi
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

                // KHẮC PHỤC LỖI KHÓA CỨNG: Giải phóng hoàn toàn tia chuột cho Sổ lớn ngay khi vừa xuất hiện
                CanvasGroup largeBookCG = largeBookDisplay.GetComponent<CanvasGroup>() ?? largeBookDisplay.gameObject.AddComponent<CanvasGroup>();
                largeBookCG.alpha = 1f;
                largeBookCG.blocksRaycasts = true; // Ép bật sẵn va chạm chuột cho sổ lớn

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