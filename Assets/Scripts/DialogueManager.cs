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
        IsInConversation = true; // Báo bận

        for (int i = 0; i < data.lines.Count; i++)
        {
            DialogueLine currentLine = data.lines[i];

            // 1. Chờ thời gian trước khi xuất hiện câu thoại tiếp theo (Trễ nhịp)
            if (i > 0) yield return new WaitForSeconds(timeBeforeNextLine);

            // 2. Sinh phôi bong bóng thoại mới vào Container
            GameObject prefabToSpawn = (currentLine.speaker == Speaker.Player) ? playerBubblePrefab : npcBubblePrefab;
            if (prefabToSpawn != null && dialogueContainer != null)
            {
                GameObject spawnedBubble = Instantiate(prefabToSpawn, dialogueContainer);
                spawnedBubble.transform.SetAsLastSibling();

                TextMeshProUGUI textUI = spawnedBubble.GetComponentInChildren<TextMeshProUGUI>();
                if (textUI != null)
                {
                    // Chạy hiệu ứng máy đánh chữ (Hàm này chạy song song, không chặn vòng lặp cha)
                    StartCoroutine(TypeTextRoutine(currentLine.text, textUI));
                }

                // Cài đặt thời gian tự hủy cho bong bóng thoại này
                Destroy(spawnedBubble, bubbleLifeTime);
            }
        }

        // ĐỢI NỐT CÂU THOẠI CUỐI CÙNG HIỂN THỊ XONG (Khoảng bằng thời gian tồn tại)
        yield return new WaitForSeconds(bubbleLifeTime);
        
        IsInConversation = false; // CHÍNH THỨC GIẢI PHÓNG LUỒNG HỘI THOẠI
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