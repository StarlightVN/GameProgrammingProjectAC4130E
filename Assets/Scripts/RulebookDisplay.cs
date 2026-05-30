using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class RulebookDisplay : MonoBehaviour
{
    [Header("Nội dung Sổ theo từng trang")]
    [TextArea(5, 10)]
    public List<string> bookPages = new List<string>();

    [Header("UI Components")]
    public TextMeshProUGUI pageContentText; 

    [Header("Spawn New Book Settings")]
    public GameObject smallBookPrefab; 
    public Transform rulebookSlotTarget; 

    private int currentPageIndex = 0;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        currentPageIndex = 0;
        RenderPage();

        // KHẮC PHỤC LỖI CỐ ĐỊNH/MỜ: Ép sổ lớn luôn sáng rõ và nhận tia chuột khi mở ra
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
        if (bookPages == null || bookPages.Count == 0)
        {
            if (pageContentText != null) pageContentText.text = "Sổ trống...";
            return;
        }
        currentPageIndex = Mathf.Clamp(currentPageIndex, 0, bookPages.Count - 1);
        if (pageContentText != null) pageContentText.text = bookPages[currentPageIndex];
    }

    public void NextPage() { if (currentPageIndex < bookPages.Count - 1) { currentPageIndex++; RenderPage(); } }
    public void PreviousPage() { if (currentPageIndex > 0) { currentPageIndex--; RenderPage(); } }

    public void CloseBook()
    {
        if (smallBookPrefab != null && rulebookSlotTarget != null)
        {
            GameObject newSmallBook = Instantiate(smallBookPrefab, rulebookSlotTarget);
            
            RectTransform newBookRect = newSmallBook.GetComponent<RectTransform>();
            newBookRect.anchoredPosition = Vector2.zero;
            newBookRect.localPosition = Vector3.zero;
            newBookRect.rotation = Quaternion.identity; 

            // KHẮC PHỤC LỖI MỜ LẦN 2: Ép sổ nhỏ mới sinh luôn ở trạng thái sạch, không bị mờ
            CanvasGroup smallCG = newSmallBook.GetComponent<CanvasGroup>() ?? newSmallBook.AddComponent<CanvasGroup>();
            smallCG.alpha = 1f;
            smallCG.blocksRaycasts = true;

            UIDragDrop newDrag = newSmallBook.GetComponent<UIDragDrop>();
            if (newDrag != null)
            {
                newDrag.SetStablePosition(newBookRect.position);
            }
        }

        GetComponent<RectTransform>().localPosition = Vector3.zero;
        gameObject.SetActive(false);
    }
}