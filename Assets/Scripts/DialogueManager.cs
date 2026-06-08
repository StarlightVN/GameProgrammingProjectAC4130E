using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("--- MẪU PREFAB BONG BÓNG THOẠI ---")]
    [Tooltip("Prefab của bong bóng Player (Có chứa Image nền và TMP Text con)")]
    public GameObject playerBubblePrefab;
    [Tooltip("Prefab của bong bóng NPC (Có chứa Image nền và TMP Text con)")]
    public GameObject npcBubblePrefab;
    
    [Header("--- KHUNG CHỨA TỔNG (CONTAINER) ---")]
    [Tooltip("Kéo Panel 'Dialogue_UI_Container' (Có gắn Vertical Layout Group) vào đây")]
    public Transform dialogueContainer;

    [Header("--- THÔNG SỐ THỜI GIAN (TIMING) ---")]
    public float typingSpeed = 0.02f;
    [Tooltip("Thời gian bong bóng thoại tồn tại trên màn hình trước khi tự hủy (giây)")]
    public float bubbleLifeTime = 4.0f;
    [Tooltip("Thời gian tạm nghỉ (trễ) trước khi xuất hiện câu thoại tiếp theo (giây)")]
    public float timeBeforeNextLine = 0.5f;

    private Coroutine activeDialogueRoutine;
    
    // Cờ đánh dấu hệ thống đang bận chạy chuỗi hội thoại
    public bool IsInConversation { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void PlayDialogue(DialogueData dialogueData)
    {
        if (dialogueData == null || dialogueData.lines.Count == 0) return;

        if (activeDialogueRoutine != null) StopCoroutine(activeDialogueRoutine);
        activeDialogueRoutine = StartCoroutine(ExecuteDialogueRoutine(dialogueData));
    }

    private IEnumerator ExecuteDialogueRoutine(DialogueData data)
    {
        IsInConversation = true;

        for (int i = 0; i < data.lines.Count; i++)
        {
            DialogueLine currentLine = data.lines[i];

            // BƯỚC 1: CHỜ THỜI GIAN TRƯỚC KHI XUẤT HIỆN BUBBLE TIẾP THEO
            if (i > 0) // Từ câu thứ 2 trở đi mới áp dụng thời gian trễ nghỉ nhịp
            {
                yield return new WaitForSeconds(timeBeforeNextLine);
            }

            // BƯỚC 2: SINH PHÔI BONG BÓNG THOẠI MỚI (Tự động đẩy câu cũ lên trên nhờ Vertical Layout)
            GameObject prefabToSpawn = (currentLine.speaker == Speaker.Player) ? playerBubblePrefab : npcBubblePrefab;
            if (prefabToSpawn != null && dialogueContainer != null)
            {
                GameObject spawnedBubble = Instantiate(prefabToSpawn, dialogueContainer);
                spawnedBubble.transform.SetAsLastSibling(); // Ép luôn nằm dưới cùng để đẩy câu cũ lên

                // Tìm linh kiện TextMeshPro con bên trong phôi vừa sinh
                TextMeshProUGUI textUI = spawnedBubble.GetComponentInChildren<TextMeshProUGUI>();
                if (textUI != null)
                {
                    // Chạy hiệu ứng máy đánh chữ cho bong bóng thoại mới sinh này
                    StartCoroutine(TypeTextRoutine(currentLine.text, textUI));
                }

                // BƯỚC 3: CÀI ĐẶT THÔNG SỐ TỰ HỦY CHO BONG BÓNG THOẠI NÀY SAU VÀI GIÂY
                Destroy(spawnedBubble, bubbleLifeTime);
            }
        }

        // Đợi nốt bong bóng thoại cuối cùng biến mất thì mới mở khóa hệ thống
        yield return new WaitForSeconds(bubbleLifeTime);
        IsInConversation = false;
    }

    private IEnumerator TypeTextRoutine(string fullText, TextMeshProUGUI textUI)
    {
        textUI.text = "";
        foreach (char c in fullText.ToCharArray())
        {
            textUI.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void ForceStopDialogue()
    {
        if (activeDialogueRoutine != null) StopCoroutine(activeDialogueRoutine);
        
        // Xóa sạch các bong bóng thoại đang trôi trên màn hình
        if (dialogueContainer != null)
        {
            foreach (Transform child in dialogueContainer)
            {
                Destroy(child.gameObject);
            }
        }
        IsInConversation = false;
    }
}