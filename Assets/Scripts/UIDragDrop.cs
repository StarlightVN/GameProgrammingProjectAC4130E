using UnityEngine;
using UnityEngine.EventSystems;

public class UIDragDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector3 lastStablePosition; 

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
    }

    public void SetStablePosition(Vector3 newPos)
    {
        lastStablePosition = newPos;
    }

    // Click vào vật nào, vật đó lập tức nhảy lên trên cùng lớp hiển thị (Z-Order)
    public void OnPointerDown(PointerEventData eventData)
    {
        transform.SetAsLastSibling(); 
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        lastStablePosition = rectTransform.position; 
        transform.SetAsLastSibling();       

        // GIẢI PHÁP ĐÓNG BĂNG TIA CHUỘT KHI ĐANG KÉO:
        // Cả vật thể LỚN và NHỎ đều cần tắt blocksRaycasts khi đang di chuyển 
        // để con trỏ chuột không bị trượt/văng trúng vào nền bàn lớn gây khóa cứng.
        canvasGroup.blocksRaycasts = false; 

        if (gameObject.CompareTag("SmallCard") || gameObject.CompareTag("SmallBook"))
        {
            canvasGroup.alpha = 0.6f; // Thẻ nhỏ/Sổ nhỏ mờ nhiều tạo hiệu ứng nhấc lên
        }
        else
        {
            canvasGroup.alpha = 0.95f; // Thẻ lớn/Sổ lớn giữ nguyên độ sáng rõ để dễ đọc luật
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        // Giới hạn ranh giới (Bounds Check) chặn cứng rìa viền bàn lớn không cho lòi ra ngoài
        if (gameObject.CompareTag("LargeBook") || gameObject.CompareTag("LargeCard"))
        {
            RectTransform parentRect = transform.parent as RectTransform;
            if (parentRect != null)
            {
                float minX = parentRect.rect.xMin + (rectTransform.rect.width / 2f);
                float maxX = parentRect.rect.xMax - (rectTransform.rect.width / 2f);
                float minY = parentRect.rect.yMin + (rectTransform.rect.height / 2f);
                float maxY = parentRect.rect.yMax - (rectTransform.rect.height / 2f);

                Vector2 clampedPosition = rectTransform.anchoredPosition;
                clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
                clampedPosition.y = Mathf.Clamp(clampedPosition.y, minY, maxY);

                rectTransform.anchoredPosition = clampedPosition;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // QUAN TRỌNG: Buông chuột xong mới bật trả lại tính năng nhận tia chuột
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true; 

        // Kiểm tra xem vị trí buông chuột cuối cùng có nằm đè lên vùng DropZone nào hợp lệ không
        bool droppedOnValidZone = false;

        // SỬA LỖI TRƯỢT CHUỘT: Nếu là Thẻ lớn/Sổ lớn và con trỏ chuột đang nằm mấp mé rìa bàn, 
        // ta ép nhận diện luôn luôn hợp lệ để tránh bị giật ngược về vị trí cũ một cách vô lý.
        if (gameObject.CompareTag("LargeCard") || gameObject.CompareTag("LargeBook"))
        {
            if (eventData.pointerEnter != null)
            {
                // Nếu chuột đang đè lên chính nó, hoặc đè lên nền Bàn lớn, hoặc đè lên Bàn nhỏ/Khay hồng
                if (eventData.pointerEnter == gameObject || 
                    eventData.pointerEnter.GetComponent<IDropHandler>() != null ||
                    eventData.pointerEnter.transform.IsChildOf(transform.parent))
                {
                    droppedOnValidZone = true;
                }
            }
        }
        else
        {
            // Đối với vật thể nhỏ, giữ nguyên logic kiểm tra DropZone gốc
            if (eventData.pointerEnter != null && eventData.pointerEnter.GetComponent<IDropHandler>() != null)
            {
                droppedOnValidZone = true;
            }
        }

        if (!droppedOnValidZone)
        {
            // Thả ra ngoài không trung (vùng xám), giật về vị trí ổn định cũ 
            rectTransform.position = lastStablePosition;
        }
        else
        {
            // Thả vào vùng hợp lệ, ghi nhận điểm neo ổn định mới 
            lastStablePosition = rectTransform.position;
        }
    }
}