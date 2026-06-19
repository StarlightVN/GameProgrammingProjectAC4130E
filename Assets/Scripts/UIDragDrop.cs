using UnityEngine;
using UnityEngine.EventSystems;

public class UIDragDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector3 lastStablePosition; 

    [Header("--- PHÂN LOẠI ĐỐI TƯỢNG KÉO THẢ ---")]
    [Tooltip("Tích chọn nếu đây là phôi nhỏ ở bàn dưới. Bỏ tích nếu đây là vật thể lớn.")]
    public bool isSmallCard = false;

    [Header("--- RANH GIỚI DI CHUYỂN PHẲNG (CHỐNG VĂNG GIẤY) ---")]
    [Tooltip("Kéo đối tượng GameplayCanvas ngoài Hierarchy vào đây để khóa rìa viền màn hình bốt trực")]
    public RectTransform deskBoundaryRect;

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

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.SetAsLastSibling(); 
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        lastStablePosition = rectTransform.position; 
        transform.SetAsLastSibling();       
        canvasGroup.blocksRaycasts = false; // Xuyên thấu tia chuột để vùng thả nhận diện được

        // 🔥 GIẢI PHÁP 2: Ép nổi phôi nhỏ lên trên cùng bốt trực khi đang cầm kéo rê
        if (isSmallCard || gameObject.CompareTag("SmallCard") || gameObject.CompareTag("SmallBook") || gameObject.CompareTag("SmallNewspaper"))
        {
            canvasGroup.alpha = 0.6f; 

            // Rút linh kiện Canvas gán cứng trên Prefab nhỏ để ép nổi tầng hiển thị
            Canvas myCanvas = GetComponent<Canvas>();
            if (myCanvas != null)
            {
                myCanvas.overrideSorting = true;
                myCanvas.sortingOrder = 999; // Đẩy lên tầng mây cao nhất, đè lên cả bàn lớn và sổ luật
            }
        }
        else
        {
            canvasGroup.alpha = 0.95f; 
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (rectTransform == null || canvas == null) return;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        // Vòng lặp nẹp viền liên tục khi đang rê chuột
        if (!isSmallCard && deskBoundaryRect != null)
        {
            ClampToBoundary();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true; 

        if (isSmallCard || gameObject.CompareTag("SmallCard") || gameObject.CompareTag("SmallBook") || gameObject.CompareTag("SmallNewspaper"))
        {
            // 🔥 Tắt chế độ ép nổi khi buông tay để phôi nhỏ chìm lại về khay gỗ bàn dưới đúng quy chế
            Canvas myCanvas = GetComponent<Canvas>();
            if (myCanvas != null)
            {
                myCanvas.overrideSorting = false;
            }

            bool droppedOnValidZone = (eventData.pointerEnter != null && eventData.pointerEnter.GetComponent<IDropHandler>() != null);
            if (!droppedOnValidZone) rectTransform.position = lastStablePosition; 
            else lastStablePosition = rectTransform.position;
        }
    }

    // =========================================================================
    // 🔥 GIẢI PHÁP 1: HÀM BO VIỀN CƯỠNG CHẾ (Giải quyết dứt điểm lỗi chờm biên lần đầu)
    // =========================================================================
    public void ClampToBoundary()
    {
        if (isSmallCard || deskBoundaryRect == null || rectTransform == null) return;

        Vector2 localPos = deskBoundaryRect.InverseTransformPoint(rectTransform.position);
        float halfWidth = rectTransform.rect.width / 2f;
        float halfHeight = rectTransform.rect.height / 2f;
        
        float minX = deskBoundaryRect.rect.xMin + halfWidth;
        float maxX = deskBoundaryRect.rect.xMax - halfWidth;
        float minY = deskBoundaryRect.rect.yMin + halfHeight;
        float maxY = deskBoundaryRect.rect.yMax - halfHeight;
        
        localPos.x = Mathf.Clamp(localPos.x, minX, maxX);
        localPos.y = Mathf.Clamp(localPos.y, minY, maxY);
        
        rectTransform.position = deskBoundaryRect.TransformPoint(localPos);
        lastStablePosition = rectTransform.position; // Cập nhật luôn điểm tọa độ an toàn
    }
}