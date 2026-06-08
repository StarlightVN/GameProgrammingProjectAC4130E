using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDragDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector3 lastStablePosition; 

    [Header("--- CẤU HÌNH THU HỒI (RULEBOOK STYLE) ---")]
    [Tooltip("Kéo đối tượng phân vùng gỗ Bàn Nhỏ (Desk_Small) ngoài Hierarchy vào đây")]
    public RectTransform smallDeskZoneRect;

    [Tooltip("TÍCH CHỌN nếu đây là phôi giấy nhỏ bàn dưới. BỎ TÍCH nếu đây là giấy lớn bàn soi phẳng.")]
    public bool isSmallCard = false;

    [Tooltip("CHỈ DÀNH CHO GIẤY TỜ LỚN: Xác định loại giấy tờ này để cất về đúng phôi nhỏ")]
    public DocumentType largeDocType = DocumentType.MainCard;

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
        // Click vào tờ nào, tờ đó lập tức nổi lên trên cùng của lớp vẽ hiện tại
        transform.SetAsLastSibling(); 
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        lastStablePosition = rectTransform.position; 
        transform.SetAsLastSibling();       

        // Xuyên thấu hoàn toàn tia chuột trong lúc di chuyển để không tự chặn chính mình
        canvasGroup.blocksRaycasts = false; 

        if (isSmallCard || gameObject.CompareTag("SmallCard") || gameObject.CompareTag("SmallBook"))
        {
            canvasGroup.alpha = 0.6f; 

            // ÉP NỔI TRÊN CÙNG: Giúp phôi nhỏ đè lên trên tấm nền bàn lớn khi đang di chuyển
            Canvas dynamicCanvas = gameObject.GetComponent<Canvas>();
            if (dynamicCanvas == null) dynamicCanvas = gameObject.AddComponent<Canvas>();
            dynamicCanvas.overrideSorting = true;
            dynamicCanvas.sortingOrder = 999;

            if (gameObject.GetComponent<GraphicRaycaster>() == null) gameObject.AddComponent<GraphicRaycaster>();
        }
        else
        {
            canvasGroup.alpha = 0.95f; 
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (rectTransform != null && canvas != null)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;

        // =========================================================================
        // NHÁNH 1: QUY TRÌNH THẢ DÀNH CHO VẬT THỂ NHỎ (GIỮ NGUYÊN LOGIC GỐC)
        // =========================================================================
        if (isSmallCard || gameObject.CompareTag("SmallCard") || gameObject.CompareTag("SmallBook"))
        {
            canvasGroup.blocksRaycasts = true; 

            if (gameObject.GetComponent<GraphicRaycaster>() != null) Destroy(gameObject.GetComponent<GraphicRaycaster>());
            if (gameObject.GetComponent<Canvas>() != null) Destroy(gameObject.GetComponent<Canvas>());

            bool droppedOnValidZone = (eventData.pointerEnter != null && eventData.pointerEnter.GetComponent<IDropHandler>() != null);
            
            if (!droppedOnValidZone) rectTransform.position = lastStablePosition; 
            else lastStablePosition = rectTransform.position; 
            
            return; 
        }

        // =========================================================================
        // NHÁNH 2: XỬ LÝ CẤT GIẤY THEO PHONG CÁCH RULEBOOK (CHỈ ẢNH HƯỞNG CHÍNH NÓ)
        // =========================================================================
        if (smallDeskZoneRect != null && RectTransformUtility.RectangleContainsScreenPoint(smallDeskZoneRect, eventData.position, eventData.pressEventCamera))
        {
            // Trường hợp A: Nếu là SỔ LUẬT LỚN
            RulebookDisplay rulebookDisplay = GetComponent<RulebookDisplay>();
            if (rulebookDisplay != null || gameObject.CompareTag("LargeBook"))
            {
                canvasGroup.blocksRaycasts = true; 
                if (rulebookDisplay != null) rulebookDisplay.CloseBook(); 
                return;
            }

            // Trường hợp B: Nếu là CÁC TỜ GIẤY TỜ LỚN (studentCardGroup, gatePassGroup...)
            SchoolGateManager gateManager = Object.FindFirstObjectByType<SchoolGateManager>();
            if (gateManager != null)
            {
                // CHỈ TẮT ẨN DUY NHẤT CHÍNH NÓ (Tờ giấy đang bị kéo thả)
                gameObject.SetActive(false);
                
                // Trả lại vị trí tâm cục bộ để lần sau bấm mở ra không bị lệch vị trí ban đầu
                rectTransform.anchoredPosition = Vector2.zero;
                canvasGroup.blocksRaycasts = true;

                // Gọi lệnh GameManager hồi sinh phôi nhỏ tương ứng dưới bàn gỗ
                gateManager.ReturnToSmallCard(Vector3.zero, largeDocType);
                return;
            }
        }

        // Nếu buông chuột ngoài không trung bàn lớn -> Nằm im tại vị trí đó (Cơ chế Rulebook)
        canvasGroup.blocksRaycasts = true; 
        lastStablePosition = rectTransform.position;
    }
}