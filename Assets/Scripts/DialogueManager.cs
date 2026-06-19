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

    // 🔥 ĐÃ THÊM: Biến bắc cầu kết nối trực tiếp với SchoolGateManager để khóa/mở nút bấm tự động
    public bool isDialoguePlaying => IsInConversation;

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
        IsInConversation = true; // Báo bận luồng thoại

        for (int i = 0; i < data.lines.Count; i++)
        {
            DialogueLine currentLine = data.lines[i];

            // Chờ thời gian trước khi xuất hiện câu thoại tiếp theo (Trễ nhịp đọc)
            if (i > 0) yield return new WaitForSeconds(timeBeforeNextLine);

            // Sinh phôi bong bóng thoại mới vào Container bố cục phẳng
            GameObject prefabToSpawn = (currentLine.speaker == Speaker.Player) ? playerBubblePrefab : npcBubblePrefab;
            if (prefabToSpawn != null && dialogueContainer != null)
            {
                GameObject spawnedBubble = Instantiate(prefabToSpawn, dialogueContainer);
                spawnedBubble.transform.SetAsLastSibling();

                TextMeshProUGUI textUI = spawnedBubble.GetComponentInChildren<TextMeshProUGUI>();
                if (textUI != null)
                {
                    // 🔥 ĐÃ SỬA: Thay thế lệnh StartCoroutine thường bằng "yield return StartCoroutine"
                    // Ép vòng lặp phải đợi câu thoại này gõ xong hoàn toàn rồi mới được phép sinh câu tiếp theo
                    yield return StartCoroutine(TypeTextRoutine(currentLine.text, textUI));
                }

                // Cài đặt thời gian tự hủy cho bong bóng thoại này sau khi gõ xong
                Destroy(spawnedBubble, bubbleLifeTime);
            }
        }

        // ĐỢI NỐT CÂU THOẠI CUỐI CÙNG HIỂN THỊ XONG TRÊN MÀN HÌNH (Để người chơi đọc nốt chữ)
        yield return new WaitForSeconds(bubbleLifeTime * 0.75f);
        
        IsInConversation = false; // CHÍNH THỨC GIẢI PHÓNG LUỒNG HỘI THOẠI $\rightarrow$ Gọi người tiếp theo hoặc cho mở nút
    }

    // =========================================================================
    // 🔥 GIẢI PHÁP DIỆT LỖI \n: NẠP TOÀN BỘ CHỮ TRƯỚC - HIỆN TỪNG CHỮ SAU
    // =========================================================================
    private IEnumerator TypeTextRoutine(string fullText, TextMeshProUGUI textUI)
    {
        textUI.text = fullText; // 1. Nạp trọn vẹn lời thoại vào TextMeshPro để bẻ hàng \n ngay từ đầu
        textUI.maxVisibleCharacters = 0; // 2. Tạm thời giấu toàn bộ chữ đi

        // Ép TextMeshPro cập nhật lưới dựng hình ngay lập tức để đếm chính xác số ký tự hiển thị thực tế
        textUI.ForceMeshUpdate(); 

        // Sử dụng dữ liệu textInfo để bỏ qua các ký tự ẩn, mã màu (Rich Text) nếu có sau này
        int totalCharacters = textUI.textInfo.characterCount;
        
        // 3. Vòng lặp tăng dần số lượng ký tự được phép hiển thị ra màn hình
        for (int counter = 0; counter <= totalCharacters; counter++)
        {
            textUI.maxVisibleCharacters = counter;
            yield return new WaitForSeconds(typingSpeed);
        }

        // Bảo hiểm mở khóa toàn bộ chữ khi kết thúc hiệu ứng gõ
        textUI.maxVisibleCharacters = totalCharacters;
    }

    public void ForceStopDialogue()
    {
        if (activeDialogueRoutine != null) StopCoroutine(activeDialogueRoutine);
        
        // Xóa sạch các bong bóng thoại đang trôi trên màn hình bốt trực
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