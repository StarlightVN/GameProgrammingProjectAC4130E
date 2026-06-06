using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;
using TMPro;


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

    [Header("--- MỞ RỘNG: DANH SÁCH PREFAB PHÔI NHỎ RIÊNG BIỆT ---")]
    [Tooltip("Prefab phôi nhỏ của Thẻ Sinh Viên / Thẻ Cán Bộ chính")]
    public GameObject mainCardSmallPrefab;         
    [Tooltip("Prefab phôi nhỏ của Giấy ra vào tòa nhà")]
    public GameObject gatePassSmallPrefab;        
    [Tooltip("Prefab phôi nhỏ của Giấy chứng nhận SV quốc tế")]
    public GameObject intlCertSmallPrefab;        
    [Tooltip("Prefab phôi nhỏ của Giấy thành viên phòng nghiên cứu")]
    public GameObject labCertSmallPrefab;         

    [Header("--- CƠ CHẾ NHÂN VẬT CỐ ĐỊNH (STORY LINE) ---")]
    public List<FixedCharacterConfig> fixedCharactersToday;

    [Header("--- LEVEL SETTINGS ---")]
    public int totalStudentsToday = 5;          
    private int studentsProcessedCount = 0;     
    public Transform studentReturnZone;        
    public GameObject largeCardDisplay;        

    [Header("--- HỆ THỐNG VÉ PHẠT UI AUTOHIDE ---")]
    public GameObject citationPanelUI; 
    public TextMeshProUGUI citationReasonText; 

    private GameObject currentSmallCard;
    private bool isProcessingStudent = false;
    private PersonProfile currentPersonProfile; 
    private List<PersonProfile> activeStudentDayPool = new List<PersonProfile>();
    private Coroutine citationHideCoroutine;

    void Start()
    {
        if (studentPool != null && studentPool.Count > 0)
        {
            activeStudentDayPool = new List<PersonProfile>(studentPool);
        }
        if (citationPanelUI != null) citationPanelUI.SetActive(false);
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
        isProcessingStudent = true;
        currentPersonProfile = null;

        foreach (FixedCharacterConfig fixedChar in fixedCharactersToday)
        {
            if (fixedChar.appearanceTurn == studentsProcessedCount && fixedChar.personProfile != null)
            {
                currentPersonProfile = fixedChar.personProfile;
                string characterName = currentPersonProfile.personType == PersonType.Student ? 
                    currentPersonProfile.cardStudentName : currentPersonProfile.cardStaffName;
                Debug.Log($"<color=orange><b>[STORY]:</b> Nhân vật cố định {characterName} xuất hiện tại lượt số {studentsProcessedCount}!</color>");
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

        // --- LUỒNG PHÁT ĐA TÀI LIỆU SỬ DỤNG PREFAB ĐỘC LẬP TƯƠNG ỨNG ---
        Vector3 basePos = spawnPointSmallDesk.position;

        if (currentPersonProfile.hasMainCard)
        {
            SpawnSmallCardObject(basePos, currentPersonProfile, mainCardSmallPrefab, DocumentType.MainCard, true);
        }

        if (currentPersonProfile.hasGatePass)
        {
            Vector3 passPos = basePos + new Vector3(50f, -30f, 0f); // Tạo độ lệch nhẹ cho bừa bộn cơ khí
            SpawnSmallCardObject(passPos, currentPersonProfile, gatePassSmallPrefab, DocumentType.GatePass, true);
        }

        if (currentPersonProfile.hasIntlCertificate)
        {
            Vector3 intlPos = basePos + new Vector3(-50f, 30f, 0f);
            SpawnSmallCardObject(intlPos, currentPersonProfile, intlCertSmallPrefab, DocumentType.IntlCertificate, true);
        }

        if (currentPersonProfile.hasLabCertificate)
        {
            Vector3 labPos = basePos + new Vector3(0f, -50f, 0f);
            SpawnSmallCardObject(labPos, currentPersonProfile, labCertSmallPrefab, DocumentType.LabCertificate, true);
        }
    }

    // Hàm sinh phôi nhỏ được mở rộng để nhận diện chính xác Prefab tương thích của loại giấy đó
    private void SpawnSmallCardObject(Vector3 spawnPos, PersonProfile profile, GameObject prefabToSpawn, DocumentType docType, bool randomRotation)
    {
        if (profile == null || prefabToSpawn == null) return;

        GameObject newSmallCard = Instantiate(prefabToSpawn, spawnPointSmallDesk);
        RectTransform cardRect = newSmallCard.GetComponent<RectTransform>();
        cardRect.position = spawnPos;

        if (randomRotation)
        {
            float randomZ = Random.Range(-30f, 30f);
            cardRect.rotation = Quaternion.Euler(0, 0, randomZ);
        }

        CardDisplay display = newSmallCard.GetComponent<CardDisplay>();
        if (display != null)
        {
            display.currentProfile = profile; 
            display.smallCardDocType = docType; // Đóng dấu loại giấy tờ cho phôi thô
        }

        CanvasGroup cg = newSmallCard.GetComponent<CanvasGroup>() ?? newSmallCard.AddComponent<CanvasGroup>();
        cg.alpha = 1f;
        cg.blocksRaycasts = true;

        UIDragDrop drag = newSmallCard.GetComponent<UIDragDrop>();
        if (drag != null) drag.SetStablePosition(cardRect.position);

        newSmallCard.tag = "SmallCard";
    }

    // ĐÃ MỞ RỘNG HAI CHIỀU: Hàm cất giấy tờ tự động phân loại để tái sinh đúng phôi đặc thù
    public void ReturnToSmallCard(Vector3 dropPosition, DocumentType docType)
        {
            if (largeCardDisplay != null) largeCardDisplay.SetActive(false);

            GameObject targetPrefab = mainCardSmallPrefab; 
            switch (docType)
            {
                case DocumentType.MainCard: targetPrefab = mainCardSmallPrefab; break;
                case DocumentType.GatePass: targetPrefab = gatePassSmallPrefab; break;
                case DocumentType.IntlCertificate: targetPrefab = intlCertSmallPrefab; break;
                case DocumentType.LabCertificate: targetPrefab = labCertSmallPrefab; break;
            }

            // 1. Sinh mới chiếc phôi nhỏ làm con của bàn nhỏ
            GameObject newSmallCard = Instantiate(targetPrefab, spawnPointSmallDesk);
            RectTransform cardRect = newSmallCard.GetComponent<RectTransform>();
            
            // 2. GIẢI PHÁP TRIỆT ĐỂ CHỐNG VĂNG: Chuyển đổi tọa độ màn hình sang tọa độ phẳng UI Canvas
            Canvas parentCanvas = newSmallCard.GetComponentInParent<Canvas>();
            if (parentCanvas != null)
            {
                // Tìm khung hình phẳng cha trực tiếp điều phối phôi nhỏ
                RectTransform parentRect = spawnPointSmallDesk.GetComponent<RectTransform>();
                
                // Ép Unity tính toán vị trí chuột tương đối trong lòng bàn nhỏ vân gỗ
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    parentRect, 
                    dropPosition, 
                    parentCanvas.worldCamera, 
                    out Vector2 localPoint))
                {
                    // Gán vị trí phẳng chuẩn xác, Z sẽ luôn giữ bằng 0 an toàn!
                    cardRect.anchoredPosition = localPoint;
                }
            }
            else
            {
                // Phương án dự phòng nếu không tìm thấy Canvas cha
                cardRect.position = dropPosition;
            }

            // 3. Tạo độ xoay tự nhiên và truyền dữ liệu
            float randomZ = Random.Range(-30f, 30f);
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

        // ====================================================
        // KHỐI 1: KIỂM TRA LỖI LẼ THƯỜNG & GIẤY TỜ CƠ BẢN (LUÔN CHẠY)
        // ====================================================
        bool missingMainCard = !currentPersonProfile.hasMainCard;
        bool photoMismatch = !missingMainCard && !currentPersonProfile.isPhotoMatching;
        bool nameMismatch = !missingMainCard && !currentPersonProfile.isNameMatching;
        bool dataMismatch = !missingMainCard && !currentPersonProfile.isDataMatching;
        
        // ĐÃ TÍCH HỢP: Kiểm tra lỗi lệch ngành học (Nếu isMajorMatching == false nghĩa là bị lỗi)
        bool majorMismatch = !missingMainCard && currentPersonProfile.personType == PersonType.Student && !currentPersonProfile.isMajorMatching;

        if (missingMainCard) { violationCount++; specificErrorString = currentPersonProfile.personType == PersonType.Student ? "Sinh viên không xuất trình được thẻ sinh viên quy định." : "Cán bộ không xuất trình được thẻ công chức/giảng viên."; }
        if (photoMismatch) { violationCount++; specificErrorString = "Ảnh chân dung trên các giấy tờ không trùng khớp với người thực tế."; }
        if (nameMismatch) { violationCount++; specificErrorString = "Thông tin họ và tên giữa các loại giấy tờ bị sai lệch, không đồng nhất."; }
        if (dataMismatch) { violationCount++; specificErrorString = "Dữ liệu hành chính giữa các tài liệu đối chiếu bị đá nhau."; }
        
        // Nếu dính lỗi lệch ngành học, tăng bộ đếm lỗi và gán câu thông báo phạt chỉ đích danh
        if (majorMismatch) 
        { 
            violationCount++; 
            specificErrorString = "Sai quy chế hành chính! Ngành học ghi trên thẻ không thuộc thẩm quyền đào tạo của Khoa quản lý."; 
        }

        // Đồng bộ hóa chuỗi ID trên Giấy ra vào của Sinh viên nếu có mang giấy
        if (!missingMainCard && currentPersonProfile.personType == PersonType.Student && currentPersonProfile.hasGatePass)
        {
            if (currentPersonProfile.passIDOrStaffTag != currentPersonProfile.cardStudentID)
            {
                violationCount++;
                specificErrorString = "Mã số sinh viên in trên Giấy ra vào tòa nhà không trùng khớp với Thẻ sinh viên.";
            }
        }

        // ====================================================
        // KHỐI 2: BỘ CƠ CHẾ QUÉT LUẬT BIẾN ĐỘNG THEO TỪNG NGÀY CHƠI
        // ====================================================
        if (!missingMainCard)
        {
            switch (currentDay)
            {
                case 1:
                    bool invalidStudentID = currentPersonProfile.personType == PersonType.Student && 
                                           (string.IsNullOrEmpty(currentPersonProfile.cardStudentID) || !currentPersonProfile.cardStudentID.StartsWith("2032"));
                    if (invalidStudentID) { violationCount++; specificErrorString = "Mã số sinh viên không hợp lệ (Yêu cầu đầu số 2032)."; }
                    else if (currentPersonProfile.personType == PersonType.Student && currentPersonProfile.cardStudentFaculty != "SEEE")
                    {
                        violationCount++; specificErrorString = "Sai quy định Ngày 1! Tòa nhà hôm nay chỉ tiếp nhận sinh viên thuộc khoa SEEE.";
                    }
                    break;
                    
                case 2:
                    if (!currentPersonProfile.hasGatePass) { violationCount++; specificErrorString = "Thiếu tài liệu! Ngày hôm nay bắt buộc phải xuất trình thêm Giấy ra vào tòa nhà."; }
                    else
                    {
                        if (currentPersonProfile.personType == PersonType.Staff && currentPersonProfile.passIDOrStaffTag != "Cán bộ") { violationCount++; specificErrorString = "Sai quy chế tài liệu! Mục MSSV/Cán bộ trên Giấy ra vào của cán bộ phải ghi chữ 'Cán bộ'."; }
                        if (currentPersonProfile.personType == PersonType.Student && currentPersonProfile.passPurpose == GatePurpose.NghienCuu) { violationCount++; specificErrorString = "Vi phạm lệnh cấm Ngày 2! Sinh viên không được vào tòa nhà với mục đích Nghiên cứu."; }
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
                    else { violationCount++; specificErrorString = "Yêu cầu xuất trình Giấy ra vào tòa nhà để đối chiếu mục đích tham gia Ngày hội."; }
                    break;
                    
                case 4:
                    if (currentPersonProfile.personType == PersonType.Student)
                    {
                        if (currentPersonProfile.hasIntlCertificate)
                        {
                            if (currentPersonProfile.intlStudentID != currentPersonProfile.cardStudentID) { violationCount++; specificErrorString = "Thông tin đá nhau! Mã số sinh viên trên Giấy chứng nhận quốc tế không khớp."; }
                        }
                        else { violationCount++; specificErrorString = "Thiếu giấy tờ Ngày 4! Sinh viên quốc tế phải xuất trình Giấy chứng nhận quốc tế."; }
                    }
                    break;
            }
        }

        // ====================================================
        // KHỐI 3: ĐỐI CHIẾU PHÁN QUYẾT ĐỂ PHÁT VÉ PHẠT UI
        // ====================================================
        bool studentHasAnyViolation = violationCount > 0;
        string finalTicketText = "";

        if (playerApproved && studentHasAnyViolation)
        {
            // Luật gộp lỗi: Nếu sinh viên dính cả lệch ngành lẫn lệch MSSV (tổng >= 2 lỗi) -> In thông báo bao quát
            finalTicketText = violationCount >= 2 ? "Người này mang theo hồ sơ có nhiều lỗi vi phạm quy chế cùng lúc." : specificErrorString;
        }
        else if (!playerApproved && !studentHasAnyViolation)
        {
            finalTicketText = "Từ chối sai lệch! Hồ sơ người này hoàn toàn hợp lệ và đúng quy chế trường.";
        }

        if (!string.IsNullOrEmpty(finalTicketText)) ShowCitationTicket(finalTicketText);

        // Quét dọn phôi rác ở bàn nhỏ dưới để sẵn sàng chuyển lượt
        GameObject[] remainingSmallCards = GameObject.FindGameObjectsWithTag("SmallCard");
        foreach (GameObject card in remainingSmallCards) Destroy(card);

        if (largeCardDisplay != null) largeCardDisplay.SetActive(false);
        isProcessingStudent = false;
        studentsProcessedCount++;
        CheckEndOfDay();
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
    private void CheckEndOfDay() { if (studentsProcessedCount >= totalStudentsToday) Debug.Log($"<color=cyan><b>HẾT NGÀY!</b> Ca trực Ngày {currentDay} đã hoàn thành.</color>"); }
}