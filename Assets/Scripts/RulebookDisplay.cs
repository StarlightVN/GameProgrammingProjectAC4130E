using UnityEngine;
using UnityEngine.UI; // BẮT BUỘC: Để điều khiển linh kiện UI Image
using System.Collections.Generic;

public class RulebookDisplay : MonoBehaviour
{
    [Header("--- NỘI DUNG SỔ THEO TỪNG TRANG ĐÔI (ẢNH SPREAD) ---")]
    [Tooltip("Kéo danh sách các tấm ảnh trang sổ (đã vẽ cả trang trái + phải trên 1 ảnh) vào đây")]
    public List<Sprite> bookPageSprites = new List<Sprite>();

    [Header("--- UI COMPONENTS HIỂN THỊ ---")]
    [Tooltip("Kéo linh kiện UI Image đóng vai trò làm mặt giấy lật mở bên trong Sổ luật vào đây")]
    public Image bookPageImage; 

    [Header("--- SPAWN NEW BOOK SETTINGS ---")]
    public GameObject smallBookPrefab; 
    public Transform rulebookSlotTarget; 
    
    [Header("--- SFX LẬT TRANG SỔ ---")]
    public AudioClip pageFlipSFX;

    private int currentPageIndex = 0;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        currentPageIndex = 0; // Luôn mở ra ở trang đầu tiên
        RenderPage();

        // Ép sổ lớn luôn sáng rõ và nhận tia chuột khi mở ra
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }

        UIDragDrop dragScript = GetComponent<UIDragDrop>();
        if (dragScript != null)
        {
            dragScript.SetStablePosition(transform.position);
        }
    }

    public void RenderPage()
    {
        if (bookPageSprites == null || bookPageSprites.Count == 0)
        {
            if (bookPageImage != null) bookPageImage.sprite = null;
            return;
        }
        
        // Nẹp cứng chỉ số trang trong ranh giới số lượng ảnh có sẵn
        currentPageIndex = Mathf.Clamp(currentPageIndex, 0, bookPageSprites.Count - 1);
        
        // Thay đổi Sprite ảnh hiển thị
        if (bookPageImage != null) 
        {
            bookPageImage.sprite = bookPageSprites[currentPageIndex];
        }
    }

    // Hàm lật trang tịnh tiến lên trước / về sau
    public void NextPage() { if (currentPageIndex < bookPageSprites.Count - 1) { currentPageIndex++; RenderPage(); PlayFlipSound(); } }
    public void PreviousPage() { if (currentPageIndex > 0) { currentPageIndex--; RenderPage(); PlayFlipSound(); } }

    // =========================================================================
    // 🔥 CHỨC NĂNG MỚI: TÍNH NĂNG BOOKMARK ĐIỀU HƯỚNG NHẢY TRANG CẤP TỐC
    // =========================================================================
    public void GoToPage(int pageIndex)
    {
        if (bookPageSprites == null || bookPageSprites.Count == 0) return;

        // Gán cứng số trang và ép vẽ lại giao diện tức thì
        currentPageIndex = Mathf.Clamp(pageIndex, 0, bookPageSprites.Count - 1);
        RenderPage();
        
        Debug.Log($"HỆ THỐNG SỔ: Đã nhảy nhanh đến trang số {currentPageIndex} thông qua Bookmark.");
    }

    public void CloseBook()
    {
        if (smallBookPrefab != null && rulebookSlotTarget != null)
        {
            GameObject newSmallBook = Instantiate(smallBookPrefab, rulebookSlotTarget);
            
            RectTransform newBookRect = newSmallBook.GetComponent<RectTransform>();
            newBookRect.anchoredPosition = Vector2.zero;
            newBookRect.localPosition = Vector3.zero;
            newBookRect.rotation = Quaternion.identity; 

            CanvasGroup smallCG = newSmallBook.GetComponent<CanvasGroup>() ?? newSmallBook.AddComponent<CanvasGroup>();
            smallCG.alpha = 1f;
            smallCG.blocksRaycasts = true;

            UIDragDrop newDrag = newSmallBook.GetComponent<UIDragDrop>();
            if (newDrag != null)
            {
                newDrag.SetStablePosition(newBookRect.position);
            }
        }

        // Đưa RectTransform về vị trí tâm bàn lớn trước khi tắt ẩn
        RectTransform myRect = GetComponent<RectTransform>();
        if (myRect != null)
        {
            myRect.anchoredPosition = Vector2.zero; 
        }
        
        gameObject.SetActive(false);
    }
    private void PlayFlipSound()
    {
        // Thử tìm GameManager trên Hierarchy để mượn đầu phát SFX chung phát ra tiếng
        SchoolGateManager gateManager = Object.FindFirstObjectByType<SchoolGateManager>();
        if (gateManager != null && gateManager.sfxSource != null && pageFlipSFX != null)
        {
            gateManager.sfxSource.PlayOneShot(pageFlipSFX);
        }
    }
}