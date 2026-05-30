using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;
using TMPro;

public class SchoolGateManager : MonoBehaviour
{
    [Header("Data Pool & Prefabs")]
    public List<StudentProfile> studentPool;     
    public GameObject smallCardPrefab;         
    public Transform spawnPointSmallDesk;      

    [Header("Level Settings")]
    public int totalStudentsToday = 5;          
    private int studentsProcessedCount = 0;     
    public Transform studentReturnZone;        
    public GameObject largeCardDisplay;        

    [Header("HỆ THỐNG VÉ PHẠT UI (TICKET CITATION)")]
    public GameObject citationPanelUI; 
    public TextMeshProUGUI citationReasonText; 

    private GameObject currentSmallCard;
    private bool isProcessingStudent = false;
    private StudentProfile currentStudentProfile; 
    private List<StudentProfile> activeStudentDayPool = new List<StudentProfile>();

    // Biến lưu vết Coroutine đếm ngược 10 giây để kiểm soát việc làm mới thời gian
    private Coroutine citationHideCoroutine;

    void Start()
    {
        if (studentPool != null && studentPool.Count > 0)
        {
            activeStudentDayPool = new List<StudentProfile>(studentPool);
        }
        
        if (citationPanelUI != null) citationPanelUI.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame 
            && !isProcessingStudent && studentsProcessedCount < totalStudentsToday)
        {
            // Nếu người chơi gọi học sinh mới, ta chủ động ẩn vé phạt cũ ngay lập tức
            if (citationPanelUI != null && citationPanelUI.activeSelf) HideCitationTicket();

            SpawnNextStudent();
        }
    }

    public void SpawnNextStudent()
    {
        if (activeStudentDayPool == null || activeStudentDayPool.Count == 0) return;
        isProcessingStudent = true;

        int randomIndex = Random.Range(0, activeStudentDayPool.Count);
        currentStudentProfile = activeStudentDayPool[randomIndex];
        activeStudentDayPool.RemoveAt(randomIndex);

        if (currentStudentProfile.HasStudentCard)
        {
            SpawnSmallCardObject(spawnPointSmallDesk.position, currentStudentProfile, true);
        }
        else
        {
            SpawnSmallCardObject(Vector3.zero, null, false);
        }
    }

    private void SpawnSmallCardObject(Vector3 spawnPos, StudentProfile profile, bool randomRotation)
    {
        if (profile == null) return;

        currentSmallCard = Instantiate(smallCardPrefab, spawnPointSmallDesk);
        RectTransform cardRect = currentSmallCard.GetComponent<RectTransform>();
        cardRect.position = spawnPos;

        if (randomRotation)
        {
            float randomZ = Random.Range(-30f, 30f);
            cardRect.rotation = Quaternion.Euler(0, 0, randomZ);
        }

        StudentCardDisplay display = currentSmallCard.GetComponent<StudentCardDisplay>();
        if (display != null) display.currentProfile = profile; 

        currentSmallCard.tag = "SmallCard";
    }

    public void ReturnToSmallCard(Vector3 dropPosition)
    {
        if (largeCardDisplay != null) largeCardDisplay.SetActive(false);
        SpawnSmallCardObject(dropPosition, currentStudentProfile, true);
    }

    public void OnApproveButtonPressed() { HandleDecision(true); }
    public void OnDenyButtonPressed() { HandleDecision(false); }

    private void HandleDecision(bool playerApproved)
    {
        if (!isProcessingStudent) return;

        // 1. Kiểm tra toàn bộ danh sách các lỗi có thể xảy ra 
        bool missingCard = !currentStudentProfile.HasStudentCard; 
        bool invalidID = !missingCard && (string.IsNullOrEmpty(currentStudentProfile.CardStudentID) || !currentStudentProfile.CardStudentID.StartsWith("2032")); 
        bool photoMismatch = !missingCard && !currentStudentProfile.IsPhotoMatching;   
        bool nameMismatch = !missingCard && !currentStudentProfile.IsNameMatching; 

        // 2. Sử dụng bộ đếm để tính xem tổng cộng có bao nhiêu lỗi vi phạm
        int violationCount = 0;
        if (missingCard) violationCount++;
        if (invalidID) violationCount++;
        if (photoMismatch) violationCount++;
        if (nameMismatch) violationCount++;

        bool studentHasAnyViolation = violationCount > 0;
        string finalErrorString = "";

        // 3. Phân tích phán quyết để tìm ra chuỗi văn bản thông báo phạt cụ thể 
        if (playerApproved && studentHasAnyViolation)
        {
            // YÊU CẦU MỚI: Nếu sinh viên có từ 2 điểm sai sót trở lên, chỉ in thông báo tổng quát
            if (violationCount >= 2)
            {
                finalErrorString = "Sinh viên này mang theo hồ sơ có nhiều lỗi vi phạm quy chế cùng lúc.";
            }
            else
            {
                // Nếu chỉ có duy nhất 1 lỗi, in chỉ đích danh lỗi đó như bình thường 
                if (missingCard) finalErrorString = "Sinh viên không xuất trình được thẻ sinh viên quy định.";
                else if (invalidID) finalErrorString = "Mã số sinh viên không hợp lệ (Yêu cầu đầu số 2032).";
                else if (photoMismatch) finalErrorString = "Ảnh chân dung trên thẻ không trùng khớp với người thực tế.";
                else if (nameMismatch) finalErrorString = "Thông tin họ tên in trên thẻ sinh viên không chính xác.";
            }
        }
        else if (!playerApproved && !studentHasAnyViolation)
        {
            // Bị phạt vì từ chối sai một người hoàn toàn trong sạch
            finalErrorString = "Từ chối sai lệch! Hồ sơ sinh viên hoàn toàn hợp lệ và đúng quy chế trường.";
        }

        // 4. KÍCH HOẠT VÉ PHẠT LÊN UI NẾU MẮC LỖI
        if (!string.IsNullOrEmpty(finalErrorString))
        {
            ShowCitationTicket(finalErrorString);
        }
        else
        {
            Debug.Log("<color=green>Xét duyệt chính xác!</color>");
        }

        // Dọn dẹp bàn làm việc hoán đổi lượt chơi như cũ 
        if (largeCardDisplay != null) largeCardDisplay.SetActive(false);

        if (currentSmallCard != null)
        {
            currentSmallCard.SetActive(true);
            StartCoroutine(ReturnCardToStudentRoutine(currentSmallCard));
        }
        else
        {
            StartCoroutine(ClearStudentWithoutCardRoutine());
        }
    }

    private void ShowCitationTicket(string errorLog)
    {
        if (citationPanelUI != null && citationReasonText != null)
        {
            // Điền văn bản lỗi vi phạm mới nhất 
            citationReasonText.text = $"<color=#FF3B30><b>HÀNH VI PHẠM:</b></color>\n{errorLog}";
            citationPanelUI.SetActive(true);
            citationPanelUI.transform.SetAsLastSibling(); 

            // YÊU CẦU MỚI: SỬA LỖI MẮC LỖI TIẾP TRONG 10 GIÂY
            // Nếu có một bộ đếm thời gian ẩn của vé cũ đang chạy dở, lập tức dập tắt nó đi!
            if (citationHideCoroutine != null)
            {
                StopCoroutine(citationHideCoroutine);
            }

            // Khởi động một vòng đếm trễ 10 giây hoàn toàn mới cho lỗi vừa mắc phải
            citationHideCoroutine = StartCoroutine(AutoHideTicketRoutine(10f));
        }
    }

    // Coroutine đếm ngược thời gian thực để tự động đóng vé phạt
    private IEnumerator AutoHideTicketRoutine(float delaySeconds)
    {
        // Chờ đúng 10 giây theo yêu cầu của bạn
        yield return new WaitForSeconds(delaySeconds);

        // Sau khi hết thời gian chờ, tự động ẩn tấm vé đi mà không bắt người chơi click chuột
        HideCitationTicket();
        citationHideCoroutine = null;
    }

    public void HideCitationTicket()
    {
        if (citationPanelUI != null)
        {
            citationPanelUI.SetActive(false);
        }
    }

    private IEnumerator ClearStudentWithoutCardRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        studentsProcessedCount++;
        isProcessingStudent = false;
        CheckEndOfDay();
    }

    private IEnumerator ReturnCardToStudentRoutine(GameObject cardClosure)
    {
        RectTransform cardRect = cardClosure.GetComponent<RectTransform>();
        Vector3 targetPosition = studentReturnZone.position;
        float speed = 15f; 

        UIDragDrop dragScript = cardClosure.GetComponent<UIDragDrop>();
        if (dragScript != null) dragScript.enabled = false;

        while (Vector3.Distance(cardRect.position, targetPosition) > 10f)
        {
            cardRect.position = Vector3.MoveTowards(cardRect.position, targetPosition, speed * Time.deltaTime * 100f);
            yield return null;
        }

        Destroy(cardClosure);
        currentSmallCard = null;

        studentsProcessedCount++;
        isProcessingStudent = false;
        CheckEndOfDay();
    }

    private void CheckEndOfDay()
    {
        if (studentsProcessedCount >= totalStudentsToday)
        {
            Debug.Log("<color=cyan><b>HẾT NGÀY!</b> Kiểm duyệt xong ngày 1.</color>");
        }
    }
}