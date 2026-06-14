using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

// 🔥 ĐÃ THÊM LẠI: Struct đóng gói cấu hình nhân vật cốt truyện (Sửa lỗi dòng 28)
[System.Serializable]
public struct FixedCharacterConfig
{
    [Tooltip("Hồ sơ PersonProfile của nhân vật cốt truyện cố định")]
    public PersonProfile personProfile;

    [Tooltip("Lượt xuất hiện cụ thể (Bắt đầu từ lượt số 0 là người đầu tiên trong ngày)")]
    public int appearanceTurn;
}

public class SchoolGateManager : MonoBehaviour
{
    [Header("--- CẤU HÌNH NGÀY CHƠI (SCENE SETTINGS) ---")]
    [Range(1, 5)] public int currentDay = 1;

    [Header("--- DATA POOL NGẪU NHIÊN ---")]
    public List<PersonProfile> studentPool;     
    public Transform spawnPointSmallDesk;      

    [Header("--- DANH SÁCH PREFAB PHÔI NHỎ RIÊNG BIỆT ---")]
    public GameObject mainCardSmallPrefab;         
    public GameObject gatePassSmallPrefab;        
    public GameObject intlCertSmallPrefab;        
    public GameObject labCertSmallPrefab; 
    public GameObject newspaperSmallPrefab;        

    [Header("--- KHUNG ẢNH BỐT TRỰC (BOOTH CAMERA UI) ---")]
    [Tooltip("Kéo cấu phần Image con nằm trong ô màu nâu phẳng bốt trực vào đây")]
    public Image boothAvatarDisplayUI;

    [Header("--- CƠ CHẾ NHÂN VẬT CỐ ĐỊNH (STORY LINE) ---")]
    public List<FixedCharacterConfig> fixedCharactersToday; // Lỗi dòng 28 đã được xóa sạch!

    [Header("--- LEVEL SETTINGS ---")]
    [Tooltip("Tổng số lượng người cần xét duyệt trong ngày hôm nay")]
    public int totalStudentsToday = 5;          
    private int studentsProcessedCount = 0;     
    public Transform studentReturnZone;        

    [Header("--- DANH SÁCH 5 THẺ LỚN STANDALONE NGOÀI HIERARCHY ---")]
    public GameObject largeStudentCardObject;
    public GameObject largeStaffCardObject;
    public GameObject largeGatePassObject;
    public GameObject largeIntlCertObject;
    public GameObject largeLabCertObject;
    public GameObject largeNewspaperObject;



    [Header("--- HỆ THỐNG VÉ PHẠT UI AUTOHIDE ---")]
    public GameObject citationPanelUI; 
    public TextMeshProUGUI citationReasonText; 

    [Header("--- THỐNG KÊ KẾT QUẢ CUỐI NGÀY ---")]
    public int correctDecisionsCount = 0;     
    public int incorrectDecisionsCount = 0;   
    public GameObject summaryCanvas;
    public GameObject gameplayCanvas;

    private bool isProcessingStudent = false;
    private PersonProfile currentPersonProfile; 
    private List<PersonProfile> activeStudentDayPool = new List<PersonProfile>();
    private Coroutine citationHideCoroutine;

    // Hàm Getter tối cao giúp các thẻ lớn tự trị rút dữ liệu hồ sơ hành chính
    public PersonProfile GetCurrentProfile()
    {
        return currentPersonProfile;
    }

    void Start()
    {
        if (studentPool != null && studentPool.Count > 0)
        {
            activeStudentDayPool = new List<PersonProfile>(studentPool);
        }
        if (citationPanelUI != null) citationPanelUI.SetActive(false);
        if (boothAvatarDisplayUI != null) boothAvatarDisplayUI.gameObject.SetActive(false); 
        if (summaryCanvas != null) summaryCanvas.SetActive(false); 
        if (newspaperSmallPrefab != null)
        {
            ReturnToSmallCard(Vector3.zero, DocumentType.Newspaper);
        }

        // Ẩn tất cả các thẻ lớn standalone lúc bắt đầu vào game
        HideAllLargeCards();
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame 
            && !isProcessingStudent && studentsProcessedCount < totalStudentsToday)
        {
            if (citationPanelUI != null && citationPanelUI.activeSelf) HideCitationTicket();
            SpawnNextStudent();
        }
    }

    public void SpawnNextStudent()
    {
        if (DialogueManager.Instance != null) DialogueManager.Instance.ForceStopDialogue(); 

        isProcessingStudent = true; 
        currentPersonProfile = null;

        foreach (FixedCharacterConfig fixedChar in fixedCharactersToday)
        {
            if (fixedChar.appearanceTurn == studentsProcessedCount && fixedChar.personProfile != null)
            {
                currentPersonProfile = fixedChar.personProfile;
                break;
            }
        }

        if (currentPersonProfile == null)
        {
            if (activeStudentDayPool != null && activeStudentDayPool.Count > 0)
            {
                int randomIndex = Random.Range(0, activeStudentDayPool.Count);
                currentPersonProfile = activeStudentDayPool[randomIndex];
                activeStudentDayPool.RemoveAt(randomIndex); 
            }
            else
            {
                Debug.LogWarning("HỆ THỐNG: Bể nhân vật ngẫu nhiên đã cạn kiệt!");
                isProcessingStudent = false;
                return;
            }
        }

        currentPersonProfile.cardStudentDoB = FormatToDateString(currentPersonProfile.cardStudentDoB);
        currentPersonProfile.cardStudentExpirationDate = FormatToDateString(currentPersonProfile.cardStudentExpirationDate);
        currentPersonProfile.cardStaffDoB = FormatToDateString(currentPersonProfile.cardStaffDoB);
        currentPersonProfile.passDurationDate = FormatToDateString(currentPersonProfile.passDurationDate);

        if (boothAvatarDisplayUI != null && currentPersonProfile.boothAvatarImage != null)
        {
            boothAvatarDisplayUI.sprite = currentPersonProfile.boothAvatarImage;
            boothAvatarDisplayUI.gameObject.SetActive(true); 
        }

        Vector3 basePos = spawnPointSmallDesk.position;

        if (currentPersonProfile.hasMainCard)
            SpawnSmallCardObject(basePos, currentPersonProfile, mainCardSmallPrefab, DocumentType.MainCard, true);

        if (currentPersonProfile.hasGatePass)
            SpawnSmallCardObject(basePos + new Vector3(1f, -0.3f, 0f), currentPersonProfile, gatePassSmallPrefab, DocumentType.GatePass, true);

        if (currentPersonProfile.hasIntlCertificate)
            SpawnSmallCardObject(basePos + new Vector3(-0.5f, 0.3f, 0f), currentPersonProfile, intlCertSmallPrefab, DocumentType.IntlCertificate, true);

        if (currentPersonProfile.hasLabCertificate)
            SpawnSmallCardObject(basePos + new Vector3(0f, -0.1f, 0f), currentPersonProfile, labCertSmallPrefab, DocumentType.LabCertificate, true);

        if (DialogueManager.Instance != null && currentPersonProfile.greetingDialogue != null)
        {
            DialogueManager.Instance.PlayDialogue(currentPersonProfile.greetingDialogue);
        }
    }

    private void SpawnSmallCardObject(Vector3 spawnPos, PersonProfile profile, GameObject prefabToSpawn, DocumentType docType, bool randomRotation)
    {
        if (profile == null || prefabToSpawn == null) return;

        GameObject newSmallCard = Instantiate(prefabToSpawn, spawnPointSmallDesk);
        RectTransform cardRect = newSmallCard.GetComponent<RectTransform>();
        cardRect.position = spawnPos;

        if (randomRotation)
        {
            float randomZ = Random.Range(-25f, 25f);
            cardRect.rotation = Quaternion.Euler(0, 0, randomZ);
        }

        CardDisplay display = newSmallCard.GetComponent<CardDisplay>();
        if (display != null)
        {
            display.currentProfile = profile; 
            display.smallCardDocType = docType; 
        }

        CanvasGroup cg = newSmallCard.GetComponent<CanvasGroup>() ?? newSmallCard.AddComponent<CanvasGroup>();
        cg.alpha = 1f;
        cg.blocksRaycasts = true;

        UIDragDrop drag = newSmallCard.GetComponent<UIDragDrop>();
        if (drag != null) drag.SetStablePosition(cardRect.position);

        newSmallCard.tag = "SmallCard";
    }

    public void ReturnToSmallCard(Vector3 dropPosition, DocumentType docType)
    {
        GameObject targetPrefab = mainCardSmallPrefab; 
        switch (docType)
        {
            case DocumentType.MainCard: targetPrefab = mainCardSmallPrefab; break;
            case DocumentType.GatePass: targetPrefab = gatePassSmallPrefab; break;
            case DocumentType.IntlCertificate: targetPrefab = intlCertSmallPrefab; break;
            case DocumentType.LabCertificate: targetPrefab = labCertSmallPrefab; break;
            case DocumentType.Newspaper: targetPrefab = newspaperSmallPrefab; break;
        }

        GameObject newSmallCard = Instantiate(targetPrefab, spawnPointSmallDesk);
        RectTransform cardRect = newSmallCard.GetComponent<RectTransform>();
        
        float randomX = Random.Range(-30f, 30f);
        float randomY = Random.Range(-20f, 20f);
        cardRect.anchoredPosition3D = new Vector3(randomX, randomY, 0f);

        float randomZ = Random.Range(-25f, 25f);
        cardRect.rotation = Quaternion.Euler(0, 0, randomZ);

        CardDisplay smallDisplay = newSmallCard.GetComponent<CardDisplay>();
        if (smallDisplay != null)
        {
            smallDisplay.currentProfile = currentPersonProfile; 
            smallDisplay.smallCardDocType = docType; 
        }
    }

    public void OnApproveButtonPressed() { HandleDecision(true); }
    public void OnDenyButtonPressed() { HandleDecision(false); }

    private void HandleDecision(bool playerApproved)
    {
        if (!isProcessingStudent) return;

        int violationCount = 0;
        string specificErrorString = "";

        bool missingMainCard = !currentPersonProfile.hasMainCard;
        bool photoMismatch = !missingMainCard && !currentPersonProfile.isPhotoMatching;
        bool nameMismatch = !missingMainCard && !currentPersonProfile.isNameMatching;
        bool dataMismatch = !missingMainCard && !currentPersonProfile.isDataMatching;
        bool majorMismatch = !missingMainCard && currentPersonProfile.personType == PersonType.Student && !currentPersonProfile.isMajorMatching;
        bool avatarMismatch = !missingMainCard && !currentPersonProfile.isAvatarMatching;
        bool paperExpired = !missingMainCard && !currentPersonProfile.IsPaperValid;

        if (missingMainCard) { violationCount++; specificErrorString = currentPersonProfile.personType == PersonType.Student ? "Sinh viên không xuất trình được thẻ sinh viên quy định." : "Cán bộ không xuất trình được thẻ công chức/giảng viên."; }
        if (photoMismatch) { violationCount++; specificErrorString = "Ảnh chân dung giữa các loại giấy tờ đối chiếu không trùng khớp với nhau."; }
        if (nameMismatch) { violationCount++; specificErrorString = "Thông tin họ và tên giữa các loại giấy tờ bị sai lệch, không đồng nhất."; }
        if (dataMismatch) { violationCount++; specificErrorString = "Dữ liệu hành chính giữa các tài liệu đối chiếu bị đá nhau."; }
        if (majorMismatch) { violationCount++; specificErrorString = "Sai quy chế hành chính! Ngành học ghi trên thẻ không thuộc thẩm quyền khoa."; }
        if (avatarMismatch) { violationCount++; specificErrorString = "Vi phạm nghiêm trọng! Ảnh chân dung chụp tại bốt trực không trùng khớp với diện mạo người trên thẻ."; }
        if (paperExpired) { violationCount++; specificErrorString = "Giấy tờ không hợp lệ! Tài liệu xuất trình đã quá thời hạn sử dụng quy định."; }

        if (!missingMainCard && currentPersonProfile.personType == PersonType.Student && currentPersonProfile.hasGatePass)
        {
            if (currentPersonProfile.passIDOrStaffTag != currentPersonProfile.cardStudentID)
            {
                violationCount++;
                specificErrorString = "Mã số sinh viên in trên Giấy ra vào tòa nhà không trùng khớp với Thẻ sinh viên.";
            }
        }

        if (!missingMainCard)
        {
            switch (currentDay)
            {
                case 1:
                    if (currentPersonProfile.personType == PersonType.Student && currentPersonProfile.cardStudentFaculty != "SEEE")
                    {
                        violationCount++; 
                        specificErrorString = "Sai quy định Ngày 1! Tòa nhà hôm nay chỉ tiếp nhận sinh viên thuộc khoa SEEE.";
                    }
                    break;

                case 2:
                    if (!currentPersonProfile.hasGatePass) { violationCount++; specificErrorString = "Thiếu tài liệu! Ngày hôm nay bắt buộc phải xuất trình thêm Giấy ra vào tòa nhà."; }
                    else
                    {
                        if (currentPersonProfile.personType == PersonType.Staff && currentPersonProfile.passIDOrStaffTag != "Cán bộ") { violationCount++; specificErrorString = "Sai quy chế tài liệu! Mục MSSV/Cán bộ trên Giấy ra vào của cán bộ phải ghi chữ 'Cán bộ'."; }
                        if (currentPersonProfile.personType == PersonType.Student && currentPersonProfile.passPurpose == GatePurpose.NghienCuu) { violationCount++; specificErrorString = "Vi phạm lệnh cấm Ngày 2! Sinh viên tuyệt đối không được vào với mục đích Nghiên cứu."; }
                    }
                    break;

                case 3:
                    if (currentPersonProfile.hasGatePass)
                    {
                        if (currentPersonProfile.personType == PersonType.Staff && currentPersonProfile.passIDOrStaffTag != "Cán bộ") { violationCount++; specificErrorString = "Sai quy chế tài liệu! Mục MSSV/Cán bộ trên Giấy ra vào của cán bộ phải ghi chữ 'Cán bộ'."; }
                        if (currentPersonProfile.passPurpose == GatePurpose.HocTap_GiangDay) { violationCount++; specificErrorString = "Sai mục đích Ngày 3! Hoạt động Học tập/Giảng dạy không được phép diễn ra."; }
                        if (currentPersonProfile.personType == PersonType.Student && currentPersonProfile.passPurpose == GatePurpose.NghienCuu && !currentPersonProfile.hasLabCertificate)
                        {
                            violationCount++; specificErrorString = "Thiếu tài liệu Ngày 3! Sinh viên vào nghiên cứu phải có Giấy chứng nhận phòng Lab.";
                        }
                    }
                    break;

                case 4:
                    if (currentPersonProfile.personType == PersonType.Student)
                    {
                        if (currentPersonProfile.hasIntlCertificate)
                        {
                            if (currentPersonProfile.intlStudentID != currentPersonProfile.cardStudentID) { violationCount++; specificErrorString = "Thông tin đá nhau! Mã số sinh viên trên Giấy chứng nhận quốc tế không khớp."; }
                        }
                        else { violationCount++; specificErrorString = "Thiếu giấy tờ Ngày 4! Sinh viên quốc tế phải xuất trình thêm Giấy chứng nhận quốc tế."; }
                    }
                    break;
            }
        }

        HideAllLargeCards();

        if (boothAvatarDisplayUI != null) boothAvatarDisplayUI.gameObject.SetActive(false); 

        bool studentHasAnyViolation = violationCount > 0;
        string finalTicketText = "";

        if (playerApproved && studentHasAnyViolation)
        {
            incorrectDecisionsCount++; 
            finalTicketText = violationCount >= 2 ? "Người này mang theo hồ sơ có nhiều lỗi vi phạm quy chế cùng lúc." : specificErrorString;
        }
        else if (!playerApproved && !studentHasAnyViolation)
        {
            incorrectDecisionsCount++; 
            finalTicketText = "Từ chối sai lệch! Hồ sơ người này hoàn toàn hợp lệ và đúng quy chế trường.";
        }
        else
        {
            correctDecisionsCount++; 
        }

        if (!string.IsNullOrEmpty(finalTicketText)) ShowCitationTicket(finalTicketText);

        GameObject[] remainingSmallCards = GameObject.FindGameObjectsWithTag("SmallCard");
        foreach (GameObject card in remainingSmallCards) Destroy(card);

        if (DialogueManager.Instance != null && currentPersonProfile != null)
        {
            DialogueData targetDialogue = playerApproved ? currentPersonProfile.approveDialogue : currentPersonProfile.denyDialogue;
            if (targetDialogue != null)
            {
                DialogueManager.Instance.PlayDialogue(targetDialogue);
            }
        }

        studentsProcessedCount++;
        isProcessingStudent = false; 
        
        CheckEndOfDay();
    }

    private void HideAllLargeCards()
    {
        if (largeStudentCardObject != null) largeStudentCardObject.SetActive(false);
        if (largeStaffCardObject != null) largeStaffCardObject.SetActive(false);
        if (largeGatePassObject != null) largeGatePassObject.SetActive(false);
        if (largeIntlCertObject != null) largeIntlCertObject.SetActive(false);
        if (largeLabCertObject != null) largeLabCertObject.SetActive(false);
        if (largeNewspaperObject != null) largeNewspaperObject.SetActive(false);
    }

    private string FormatToDateString(string rawInput)
    {
        if (string.IsNullOrEmpty(rawInput)) return "";
        string clean = rawInput.Trim();
        if (clean.Length == 8)
        {
            return $"{clean.Substring(0, 2)}/{clean.Substring(2, 2)}/{clean.Substring(4, 4)}";
        }
        return clean; 
    }

    private void ShowCitationTicket(string errorLog)
    {
        if (citationPanelUI != null && citationReasonText != null)
        {
            citationReasonText.text = $"<color=#FF3B30><b>HÀNH VI PHẠM:</b></color>\n{errorLog}";
            citationPanelUI.SetActive(true);
            citationPanelUI.transform.SetAsLastSibling(); 

            if (citationHideCoroutine != null) StopCoroutine(citationHideCoroutine);
            citationHideCoroutine = StartCoroutine(AutoHideTicketRoutine(10f));
        }
    }

    private IEnumerator AutoHideTicketRoutine(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        HideCitationTicket();
        citationHideCoroutine = null;
    }

    public void HideCitationTicket() { if (citationPanelUI != null) citationPanelUI.SetActive(false); }

    private void CheckEndOfDay()
    {
        if (studentsProcessedCount >= totalStudentsToday)
        {
            StartCoroutine(WaitAndShowSummaryRoutine());
        }
    }

    private IEnumerator WaitAndShowSummaryRoutine()
    {
        isProcessingStudent = true; 
        yield return new WaitForSeconds(5.0f); 

        if (gameplayCanvas != null) gameplayCanvas.SetActive(false); 

        if (summaryCanvas != null)
        {
            summaryCanvas.SetActive(true); 
            DaySummaryController summaryController = summaryCanvas.GetComponent<DaySummaryController>();
            if (summaryController != null)
            {
                summaryController.DisplayStats(correctDecisionsCount, incorrectDecisionsCount);
            }
        }
    }
}