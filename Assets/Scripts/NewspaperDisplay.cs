using UnityEngine;

public class NewspaperDisplay : MonoBehaviour
{
    [Header("Spawn New Newspaper Settings")]
    [Tooltip("Kéo file Prefab phôi báo nhỏ ngoài Project vào đây")]
    public GameObject smallNewspaperPrefab; 
    [Tooltip("Kéo đối tượng Slot chứa báo nhỏ ngoài Hierarchy vào đây")]
    public Transform newspaperSlotTarget; 

    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }

        // Đồng bộ vị trí kéo thả ban đầu
        UIDragDrop dragScript = GetComponent<UIDragDrop>();
        if (dragScript != null)
        {
            dragScript.SetStablePosition(transform.position);
        }
    }

    public void CloseNewspaper()
    {
        // Tự sinh phôi báo nhỏ tại vị trí Slot Target giống hệt Rulebook
        if (smallNewspaperPrefab != null && newspaperSlotTarget != null)
        {
            GameObject newSmallPaper = Instantiate(smallNewspaperPrefab, newspaperSlotTarget);
            
            RectTransform newPaperRect = newSmallPaper.GetComponent<RectTransform>();
            newPaperRect.anchoredPosition = Vector2.zero;
            newPaperRect.localPosition = Vector3.zero;
            newPaperRect.rotation = Quaternion.identity; 

            CanvasGroup smallCG = newSmallPaper.GetComponent<CanvasGroup>() ?? newSmallPaper.AddComponent<CanvasGroup>();
            smallCG.alpha = 1f;
            smallCG.blocksRaycasts = true;

            UIDragDrop newDrag = newSmallPaper.GetComponent<UIDragDrop>();
            if (newDrag != null)
            {
                newDrag.SetStablePosition(newPaperRect.position);
            }
        }

        // Reset vị trí báo lớn về tâm mặc định trước khi ẩn
        GetComponent<RectTransform>().localPosition = Vector3.zero;
        gameObject.SetActive(false);
    }
}