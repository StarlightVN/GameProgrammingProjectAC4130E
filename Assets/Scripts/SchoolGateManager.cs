using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
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

    [Header("--- DANH SÁCH PREFAB PHÔI NHỎ RIÊNG BIỆT ---")]
    public GameObject mainCardSmallPrefab;         
    public GameObject gatePassSmallPrefab;        
    public GameObject intlCertSmallPrefab;        
    public GameObject labCertSmallPrefab;         
    public GameObject newspaperSmallPrefab; 

    [Header("--- KHUNG ẢNH BỐT TRỰC (BOOTH CAMERA UI) ---")]
    [Tooltip("Kéo cấu phần Image hiển thị diện mạo nhân vật vào đây")]
    public Image boothAvatarDisplayUI;

    [Header("--- CƠ CHẾ NHÂN VẬT CỐ ĐỊNH (STORY LINE) ---")]
    public List<FixedCharacterConfig> fixedCharactersToday;

    [Header("--- LEVEL SETTINGS ---")]
    [Tooltip("Tổng số lượng người cần xét duyệt định mức trong ngày hôm nay")]
    public int totalStudentsToday = 5;          
    private int studentsProcessedCount = 0;     
    public Transform studentReturnZone;        

    [Header("--- DANH SÁCH THẺ LỚN STANDALONE NGOÀI HIERARCHY ---")]
    public GameObject largeStudentCardObject;
    public GameObject largeStaffCardObject;
    public GameObject largeGatePassObject;
    public GameObject largeIntlCertObject;
    public GameObject largeLabCertObject;
    public GameObject largeNewspaperObject;

    [Header("--- 🔥 HỆ THỐNG ĐỒNG HỒ ĐIỆN TỬ (TIMER SYSTEM) ---")]
    [Tooltip("Kéo linh kiện TextMeshPro hiển thị thời gian (04:00) ngoài Hierarchy vào đây")]
    public TextMeshProUGUI timerText;
    [Tooltip("Tùy chỉnh thời lượng của ca trực ngày hôm nay tính bằng GIÂY (Ví dụ: 240)")]
    public float dayDurationSeconds = 240f;
    private float timeRemaining;
    private bool isDayEnded = false;

    [Header("--- 🔥 THỜI GIAN CHỜ GỌI NGƯỜI TỰ ĐỘNG ---")]
    [Tooltip("Thời gian delay tối thiểu trước khi gọi người tiếp theo (giây)")]
    public float minSpawnDelay = 1f;
    [Tooltip("Thời gian delay tối đa trước khi gọi người tiếp theo (giây)")]
    public float maxSpawnDelay = 3f;

    [Header("--- HỆ THỐNG VÉ PHẠT UI AUTOHIDE ---")]
    public GameObject citationPanelUI; 
    public TextMeshProUGUI citationReasonText; 

    [Header("--- THỐNG KÊ KẾT QUẢ CUỐI NGÀY ---")]
    public int correctDecisionsCount = 0;     
    public int incorrectDecisionsCount = 0;   
    public GameObject summaryCanvas;
    public GameObject gameplayCanvas;

    // Biến điều phối luồng Coroutine nội bộ
    private bool isProcessingStudent = false;
    private PersonProfile currentPersonProfile; 
    private List<PersonProfile> activeStudentDayPool = new List<PersonProfile>();
    private Coroutine citationHideCoroutine;

    // Bộ token quản lý chu kỳ thoại nghiêm ngặt
    private bool canInteractButtons = false; 
    private bool hasPlayerDecided = false;    

    public PersonProfile GetCurrentProfile() { return currentPersonProfile; }

    void Start()
    {
        if (studentPool != null && studentPool.Count > 0)
        {
            activeStudentDayPool = new List<PersonProfile>(studentPool);
        }
        if (citationPanelUI != null) citationPanelUI.SetActive(false);
        if (summaryCanvas != null) summaryCanvas.SetActive(false); 

        // Khởi tạo trạng thái đồng hồ
        timeRemaining = dayDurationSeconds;
        isDayEnded = false;

        // Ẩn toàn bộ giấy tờ to và avatar lúc vào bàn trực
        HideAllLargeCards();
        if (boothAvatarDisplayUI != null)
        {
            boothAvatarDisplayUI.color = new Color(0f, 0f, 0f, 0f); // Mờ đen tuyền trong suốt
            boothAvatarDisplayUI.gameObject.SetActive(false);
        }

        // Cấp phôi báo đầu ngày
        if (newspaperSmallPrefab != null)
        {
            ReturnToSmallCard(Vector3.zero, DocumentType.Newspaper);
        }

        // 🔥 KÍCH HOẠT: Bật chuỗi vòng lặp ca trực tự động gọi người
        StartCoroutine(GameWorkflowRoutine());
    }

    void Update()
    {
        // 🔥 ĐỒNG HỒ ĐẾM NGƯỢC TỰ ĐỘNG KHÔNG DÙNG SPACEBAR NỮA
        if (!isDayEnded && timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining < 0) timeRemaining = 0;

            // Quy đổi thời gian thực sang định dạng hiển thị điện tử MM:SS
            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
            if (timerText != null)
            {
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
        }
    }

    // =========================================================================
    // 🔥 MA TRẬN VÒNG LẶP CA TRỰC TỰ ĐỘNG TOÀN NĂNG (CORE WORKFLOW)
    // =========================================================================
    private IEnumerator GameWorkflowRoutine()
    {
        // Chờ 1 giây đầu ngày cho người chơi ổn định bốt trực
        yield return new WaitForSeconds(1.0f);

        while (!isDayEnded)
        {
            // 🛑 ĐIỀU KIỆN KIỂM TRA BÙ GIỜ / HẾT NGÀY CHÍNH XÁC:
            bool hasStoryNPCLeft = false;
            foreach (FixedCharacterConfig fixedChar in fixedCharactersToday)
            {
                if (fixedChar.appearanceTurn >= studentsProcessedCount && fixedChar.personProfile != null)
                {
                    hasStoryNPCLeft = true;
                    break;
                }
            }

            // Nếu đã hết thời gian VÀ không còn nhân vật cốt truyện đặc biệt nào chờ hiệu lực -> ĐÓNG CỬA KHẨU!
            if (timeRemaining <= 0 && !hasStoryNPCLeft)
            {
                isDayEnded = true;
                break;
            }

            // Nếu chưa hết giờ nhưng đã xử lý hết định mức người quy định (và không có NPC cốt truyện chờ kích hoạt gấp)
            if (timeRemaining > 0 && studentsProcessedCount >= totalStudentsToday && !hasStoryNPCLeft)
            {
                isDayEnded = true;
                break;
            }

            // 1. Chờ một khoảng delay tùy chỉnh giữa các NPC (Tạo nhịp thở cho game)
            float randomDelay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(randomDelay);

            // Ẩn bảng vé phạt cũ của người trước nếu có
            if (citationPanelUI != null && citationPanelUI.activeSelf) HideCitationTicket();

            // 2. Thiết lập dữ liệu cho nhân vật tiếp theo bước vào bốt
            isProcessingStudent = true;
            hasPlayerDecided = false;
            canInteractButtons = false; // Khóa cứng nút bấm
            currentPersonProfile = null;

            // Dò tìm nhân vật đặc biệt cốt truyện
            foreach (FixedCharacterConfig fixedChar in fixedCharactersToday)
            {
                if (fixedChar.appearanceTurn == studentsProcessedCount && fixedChar.personProfile != null)
                {
                    currentPersonProfile = fixedChar.personProfile;
                    break;
                }
            }

            // Nếu không phải lượt đặc biệt -> Bốc ngẫu nhiên từ bể chứa (Chỉ bốc khi còn thời gian)
            if (currentPersonProfile == null)
            {
                if (timeRemaining > 0 && activeStudentDayPool != null && activeStudentDayPool.Count > 0)
                {
                    int randomIndex = Random.Range(0, activeStudentDayPool.Count);
                    currentPersonProfile = activeStudentDayPool[randomIndex];
                    activeStudentDayPool.RemoveAt(randomIndex);
                }
                else
                {
                    // Nếu hết giờ hoặc cạn pool random mà vẫn còn kẹt điều kiện câu chuyện -> Bỏ qua vòng này để quét lại điều kiện kết thúc
                    yield return null;
                    continue;
                }
            }

            // Định dạng chuỗi hành chính văn thư
            currentPersonProfile.cardStudentDoB = FormatToDateString(currentPersonProfile.cardStudentDoB);
            currentPersonProfile.cardStudentExpirationDate = FormatToDateString(currentPersonProfile.cardStudentExpirationDate);
            currentPersonProfile.cardStaffDoB = FormatToDateString(currentPersonProfile.cardStaffDoB);
            currentPersonProfile.passDurationDate = FormatToDateString(currentPersonProfile.passDurationDate);

            // 3. 🔥 HIỆU ỨNG XUẤT HIỆN FADE-IN (2 GIÂY): Từ mờ đen tuyền trong suốt sang rõ trắng đúng
            if (boothAvatarDisplayUI != null && currentPersonProfile.boothAvatarImage != null)
            {
                boothAvatarDisplayUI.sprite = currentPersonProfile.boothAvatarImage;
                boothAvatarDisplayUI.gameObject.SetActive(true);
                yield return StartCoroutine(FadeAvatarRoutine(true, 2.0f)); 
            }

            // 4. Sinh phôi giấy tờ mini rải ra bàn gỗ nhỏ dưới
            Vector3 basePos = spawnPointSmallDesk.position;
            if (currentPersonProfile.hasMainCard)
                SpawnSmallCardObject(basePos, currentPersonProfile, mainCardSmallPrefab, DocumentType.MainCard, true);
            if (currentPersonProfile.hasGatePass)
                SpawnSmallCardObject(basePos + new Vector3(1f, -0.3f, 0f), currentPersonProfile, gatePassSmallPrefab, DocumentType.GatePass, true);
            if (currentPersonProfile.hasIntlCertificate)
                SpawnSmallCardObject(basePos + new Vector3(-0.5f, 0.3f, 0f), currentPersonProfile, intlCertSmallPrefab, DocumentType.IntlCertificate, true);
            if (currentPersonProfile.hasLabCertificate)
                SpawnSmallCardObject(basePos + new Vector3(0f, -0.1f, 0f), currentPersonProfile, labCertSmallPrefab, DocumentType.LabCertificate, true);

            // 5. Chạy thoại mở đầu (Greeting Dialogue)
            if (DialogueManager.Instance != null && currentPersonProfile.greetingDialogue != null)
            {
                DialogueManager.Instance.PlayDialogue(currentPersonProfile.greetingDialogue);
                
                // 🔥 ÉP HOÀN THÀNH ĐỐI THOẠI: Đóng băng nút bấm, đợi cho đến khi DialogueManager chạy xong chữ hoàn toàn
                while (DialogueManager.Instance.IsInConversation)
                {
                    yield return null;
                }
            }

            // Giải phóng mở khóa nút bấm Duyệt / Từ chối sau khi dứt câu thoại mở đầu
            canInteractButtons = true;

            // 6. Treo luồng chờ người chơi đưa ra phán quyết (Bấm Approve hoặc Deny)
            while (!hasPlayerDecided)
            {
                yield return null;
            }

            // Người chơi đã bấm nút $\rightarrow$ Lúc này HandleDecision đã chạy xong và kích hoạt thoại phản hồi kết thúc
            canInteractButtons = false; // Tái khóa nút bấm đề phòng spam

            // 7. 🔥 ÉP HOÀN THÀNH ĐỐI THOẠI KẾT THÚC: Chờ dứt câu phản hồi rồi mới cho nhân vật biến mất
            if (DialogueManager.Instance != null)
            {
                while (DialogueManager.Instance.IsInConversation)
                {
                    yield return null;
                }
            }

            // 8. 🔥 HIỆU ỨNG BIẾN MẤT FADE-OUT (0.5 GIÂY): Nhạt dần về màu đen trong suốt ngược lại
            yield return StartCoroutine(FadeAvatarRoutine(false, 0.5f));
            if (boothAvatarDisplayUI != null) boothAvatarDisplayUI.gameObject.SetActive(false);

            // Tiêu hủy cưỡng chế phôi rác còn sót lại trên bàn soi
            GameObject[] remainingSmallCards = GameObject.FindGameObjectsWithTag("SmallCard");
            foreach (GameObject card in remainingSmallCards) Destroy(card);

            // Tăng đếm chỉ số hành chính, hoàn tất chu kỳ 1 nhân vật
            studentsProcessedCount++;
            isProcessingStudent = false;
        }

        // Thỏa mãn điều kiện đóng ca trực $\rightarrow$ Chạy sang bảng tổng kết điểm cuối ngày
        StartCoroutine(WaitAndShowSummaryRoutine());
    }

    // =========================================================================
    // 🔥 LINH KIỆN TỰ TRỊ FADE LERP MÀU SẮC AVATAR THEO THỜI GIAN
    // =========================================================================
    private IEnumerator FadeAvatarRoutine(bool fadeIn, float duration)
    {
        if (boothAvatarDisplayUI == null) yield break;

        // FadeIn: Từ Đen tuyền trong suốt (0,0,0,0) sang Trắng gốc nguyên bản (1,1,1,1)
        // FadeOut: Từ Trắng gốc (1,1,1,1) về lại Đen tuyền trong suốt (0,0,0,0) theo nửa giây
        Color startColor = fadeIn ? new Color(0f, 0f, 0f, 0f) : new Color(1f, 1f, 1f, 1f);
        Color endColor = fadeIn ? new Color(1f, 1f, 1f, 1f) : new Color(0f, 0f, 0f, 0f);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            boothAvatarDisplayUI.color = Color.Lerp(startColor, endColor, elapsed / duration);
            yield return null;
        }
        boothAvatarDisplayUI.color = endColor;
    }

    public void OnApproveButtonPressed() { if (canInteractButtons) HandleDecision(true); }
    public void OnDenyButtonPressed() { if (canInteractButtons) HandleDecision(false); }

    private void HandleDecision(bool playerApproved)
    {
        if (!isProcessingStudent || hasPlayerDecided) return;

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

        // Tắt ẩn các thẻ lớn soi tài liệu phẳng phẳng
        HideAllLargeCards();

        // Tính điểm phán quyết phát hành vé phạt hành chính
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

        // Kích hoạt nạp câu thoại kết thúc
        if (DialogueManager.Instance != null && currentPersonProfile != null)
        {
            DialogueData targetDialogue = playerApproved ? currentPersonProfile.approveDialogue : currentPersonProfile.denyDialogue;
            if (targetDialogue != null)
            {
                DialogueManager.Instance.PlayDialogue(targetDialogue);
            }
        }

        // Mở khóa tín hiệu báo chu kỳ thoại lặp kết thúc
        hasPlayerDecided = true;
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

        if (docType == DocumentType.Newspaper) newSmallCard.tag = "SmallNewspaper";
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

    private string FormatToDateString(string rawInput) { if (string.IsNullOrEmpty(rawInput)) return ""; string clean = rawInput.Trim(); if (clean.Length == 8) { return $"{clean.Substring(0, 2)}/{clean.Substring(2, 2)}/{clean.Substring(4, 4)}"; } return clean; }
    private void ShowCitationTicket(string errorLog) { if (citationPanelUI != null && citationReasonText != null) { citationReasonText.text = $"<color=#FF3B30><b>HÀNH VI PHẠM:</b></color>\n{errorLog}"; citationPanelUI.SetActive(true); citationPanelUI.transform.SetAsLastSibling(); if (citationHideCoroutine != null) StopCoroutine(citationHideCoroutine); citationHideCoroutine = StartCoroutine(AutoHideTicketRoutine(10f)); } }
    private IEnumerator AutoHideTicketRoutine(float delaySeconds) { yield return new WaitForSeconds(delaySeconds); HideCitationTicket(); citationHideCoroutine = null; }
    public void HideCitationTicket() { if (citationPanelUI != null) citationPanelUI.SetActive(false); }

    private IEnumerator WaitAndShowSummaryRoutine()
    {
        yield return new WaitForSeconds(4.0f); 
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