using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // BẮT BUỘC: Để điều phối bốc dỡ và chuyển đổi giữa các Scene
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
    public GameObject studentCardSmallPrefab;
    public GameObject staffCardSmallPrefab;  
    public GameObject gatePassSmallPrefab;  
    public GameObject intlCertSmallPrefab;  
    public GameObject labCertSmallPrefab;  
    public GameObject newspaperSmallPrefab; 

    [Header("--- KHUNG ẢNH BỐT TRỰC (BOOTH CAMERA UI) ---")]
    public Image boothAvatarDisplayUI;

    [Header("--- CƠ CHẾ NHÂN VẬT CỐ ĐỊNH (STORY LINE) ---")]
    public List<FixedCharacterConfig> fixedCharactersToday;

    [Header("--- HỒ SƠ NHÂN VẬT CỐT TRUYỆN ĐẶC BIỆT DÙNG ĐỂ CHECK RẼ NHÁNH ---")]
    [Tooltip("Kéo file hồ sơ PersonProfile của nhân vật đặc biệt Ngày 3 vào đây")]
    public PersonProfile day3SpecialCharacterProfile;
    [Tooltip("Kéo file hồ sơ PersonProfile của nhân vật A Ngày 5 vào đây")]
    public PersonProfile day5CharacterAProfile;
    [Tooltip("Kéo file hồ sơ PersonProfile của nhân vật B Ngày 5 vào đây")]
    public PersonProfile day5CharacterBProfile;

    [Header("--- LEVEL SETTINGS ---")]
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
    public GameObject largeRulebookObject;

    [Header("--- HỆ THỐNG ĐỒNG HỒ ĐIỆN TỬ (TIMER SYSTEM) ---")]
    public TextMeshProUGUI timerText;
    public float dayDurationSeconds = 240f;
    private float timeRemaining;
    private bool isDayEnded = false;

    [Header("--- THỜI GIAN CHỜ GỌI NGƯỜI TỰ ĐỘNG ---")]
    public float minSpawnDelay = 1f;
    public float maxSpawnDelay = 3f;

    [Header("--- HỆ THỐNG VÉ PHẠT UI AUTOHIDE ---")]
    public GameObject citationPanelUI; 
    public TextMeshProUGUI citationReasonText; 

    [Header("--- THỐNG KÊ KẾT QUẢ CUỐI NGÀY & ĐIỂM KPI ---")]
    public int correctDecisionsCount = 0;  
    public int incorrectDecisionsCount = 0;  
    [Tooltip("Ngưỡng điểm tổng kết tối thiểu hằng ngày bắt buộc phải đạt được để sống sót qua ngày")]
    public int totalScoreThreshold = 20;

    [Header("--- LINH KIỆN TMP TEXT THỂ HIỆN TRÊN BẢNG TỔNG KẾT ---")]
    [Tooltip("Ô TMP hiển thị tỷ lệ người đúng (Ví dụ: 4/5)")]
    public TextMeshProUGUI summaryRatioText;
    [Tooltip("Ô TMP hiển thị tổng điểm net tích lũy cuối ngày")]
    public TextMeshProUGUI summaryScoreText;
    [Tooltip("Ô TMP hiển thị điểm ngưỡng quy định của ca trực")]
    public TextMeshProUGUI summaryThresholdText;
    [Tooltip("Ô TMP hiển thị phần trăm độ chính xác hành chính")]
    public TextMeshProUGUI summaryAccuracyText;

    [Header("--- 🔥 TÊN CÁC SCENE CHUYỂN TRONG CÁC TÌNH HUỐNG ---")]
    [Tooltip("Tên Scene ngày chơi tiếp theo nếu vượt ca trực thành công (Ví dụ: Day_02)")]
    public string nextRegularDaySceneName = "Day_02";
    [Tooltip("Tên Scene kết thúc tệ do hiệu suất làm việc kém hoặc dưới ngưỡng điểm")]
    public string sceneBadEndingPerformance = "Scene_BadEnding_Performance";
    [Tooltip("Tên Scene kết thúc tệ Ngày 3 do từ chối nhân vật đặc biệt kịch bản")]
    public string sceneBadEndingDay3Special = "Scene_BadEnding_Day3_Special";
    [Tooltip("Tên Scene kết thúc tệ Ngày 5 do đồng ý cho nhân vật A vào cửa khẩu")]
    public string sceneBadEndingDay5A = "Scene_BadEnding_Day5_A";
    [Tooltip("Tên Scene kết thúc tốt số 1 (Từ chối A và Đồng ý B)")]
    public string sceneGoodEnding1 = "Scene_GoodEnding_1";
    [Tooltip("Tên Scene kết thúc tốt số 2 (Từ chối A và Từ chối B)")]
    public string sceneGoodEnding2 = "Scene_GoodEnding_2";

    [Header("--- THÀNH PHẦN KHUNG CHỨA (CANVAS COMP) ---")]
    public GameObject summaryCanvas;
    public GameObject gameplayCanvas;

    private bool isProcessingStudent = false;
    private PersonProfile currentPersonProfile; 
    private List<PersonProfile> activeStudentDayPool = new List<PersonProfile>();
    private Coroutine citationHideCoroutine;

    private bool canInteractButtons = false; 
    private bool hasPlayerDecided = false;  

    private List<FixedCharacterConfig> remainingFixedChars = new List<FixedCharacterConfig>();

    // Bộ cờ ghi nhận chuỗi trạng thái kịch bản nội bộ (Story Flags)
    private bool isDay3SpecialCharacterDenied = false;
    private bool isDay5CharAAccepted = false;
    private bool isDay5CharBAccepted = false;

    public PersonProfile GetCurrentProfile() { return currentPersonProfile; }

    void Start()
    {
        if (studentPool != null && studentPool.Count > 0)
        {
            activeStudentDayPool = new List<PersonProfile>(studentPool);
        }
        if (citationPanelUI != null) citationPanelUI.SetActive(false);
        if (summaryCanvas != null) summaryCanvas.SetActive(false); 

        timeRemaining = dayDurationSeconds;
        isDayEnded = false;

        if (fixedCharactersToday != null)
        {
            remainingFixedChars = new List<FixedCharacterConfig>(fixedCharactersToday);
            remainingFixedChars.Sort((a, b) => a.appearanceTurn.CompareTo(b.appearanceTurn));
        }

        HideAllLargeCards(false);

        if (largeNewspaperObject != null)
        {
            largeNewspaperObject.SetActive(true);
            largeNewspaperObject.transform.SetAsLastSibling();
            if (largeNewspaperObject.TryGetComponent<UIDragDrop>(out UIDragDrop newsDrag))
            {
                newsDrag.SetStablePosition(largeNewspaperObject.transform.position);
            }
        }

        if (largeRulebookObject != null)
        {
            largeRulebookObject.SetActive(true);
            largeRulebookObject.transform.SetAsLastSibling();
            if (largeRulebookObject.TryGetComponent<UIDragDrop>(out UIDragDrop bookDrag))
            {
                bookDrag.SetStablePosition(largeRulebookObject.transform.position);
            }
        }

        if (boothAvatarDisplayUI != null)
        {
            boothAvatarDisplayUI.color = new Color(0f, 0f, 0f, 0f); 
            boothAvatarDisplayUI.gameObject.SetActive(false);
        }

        isDay3SpecialCharacterDenied = false;
        isDay5CharAAccepted = false;
        isDay5CharBAccepted = false;

        StartCoroutine(GameWorkflowRoutine());
    }

    void Update()
    {
        if (!isDayEnded && timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining < 0) timeRemaining = 0;

            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
            if (timerText != null)
            {
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
        }
    }

    private IEnumerator GameWorkflowRoutine()
    {
        yield return new WaitForSeconds(1.0f);

        while (!isDayEnded)
        {
            if (remainingFixedChars.Count == 0)
            {
                if (timeRemaining <= 0 || studentsProcessedCount >= totalStudentsToday)
                {
                    isDayEnded = true;
                    break;
                }
            }

            float randomDelay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(randomDelay);

            if (citationPanelUI != null && citationPanelUI.activeSelf) HideCitationTicket();

            isProcessingStudent = true;
            hasPlayerDecided = false;
            canInteractButtons = false; 
            currentPersonProfile = null;

            FixedCharacterConfig foundFixed = default;
            bool isFixedTurn = false;

            if (timeRemaining > 0)
            {
                foreach (var fixedChar in remainingFixedChars)
                {
                    if (fixedChar.appearanceTurn == studentsProcessedCount)
                    {
                        foundFixed = fixedChar;
                        isFixedTurn = true;
                        break;
                    }
                }
            }
            else
            {
                if (remainingFixedChars.Count > 0)
                {
                    foundFixed = remainingFixedChars[0];
                    isFixedTurn = true;
                }
            }

            if (isFixedTurn)
            {
                currentPersonProfile = foundFixed.personProfile;
                remainingFixedChars.Remove(foundFixed); 
            }
            else
            {
                if (timeRemaining > 0 && activeStudentDayPool != null && activeStudentDayPool.Count > 0)
                {
                    int randomIndex = Random.Range(0, activeStudentDayPool.Count);
                    currentPersonProfile = activeStudentDayPool[randomIndex];
                    activeStudentDayPool.RemoveAt(randomIndex);
                }
                else
                {
                    yield return null;
                    continue;
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
                yield return StartCoroutine(FadeAvatarRoutine(true, 2.0f)); 
            }

            Vector3 basePos = spawnPointSmallDesk.position;
            if (currentPersonProfile.hasMainCard)
            {
                GameObject targetMainPrefab = (currentPersonProfile.personType == PersonType.Student) ? studentCardSmallPrefab : staffCardSmallPrefab;
                SpawnSmallCardObject(basePos, currentPersonProfile, targetMainPrefab, DocumentType.MainCard, true);
            }

            if (currentPersonProfile.hasGatePass)
                SpawnSmallCardObject(basePos + new Vector3(1f, -0.3f, 0f), currentPersonProfile, gatePassSmallPrefab, DocumentType.GatePass, true);
            if (currentPersonProfile.hasIntlCertificate)
                SpawnSmallCardObject(basePos + new Vector3(-0.5f, 0.3f, 0f), currentPersonProfile, intlCertSmallPrefab, DocumentType.IntlCertificate, true);
            if (currentPersonProfile.hasLabCertificate)
                SpawnSmallCardObject(basePos + new Vector3(0f, -0.1f, 0f), currentPersonProfile, labCertSmallPrefab, DocumentType.LabCertificate, true);

            if (DialogueManager.Instance != null && currentPersonProfile.greetingDialogue != null)
            {
                DialogueManager.Instance.PlayDialogue(currentPersonProfile.greetingDialogue);
                while (DialogueManager.Instance.isDialoguePlaying)
                {
                    yield return null;
                }
            }

            canInteractButtons = true; 

            while (!hasPlayerDecided)
            {
                yield return null;
            }

            canInteractButtons = false; 

            if (DialogueManager.Instance != null)
            {
                while (DialogueManager.Instance.isDialoguePlaying)
                {
                    yield return null;
                }
            }

            yield return StartCoroutine(FadeAvatarRoutine(false, 0.5f));
            if (boothAvatarDisplayUI != null) boothAvatarDisplayUI.gameObject.SetActive(false);

            GameObject[] remainingSmallCards = GameObject.FindGameObjectsWithTag("SmallCard");
            foreach (GameObject card in remainingSmallCards) Destroy(card);

            studentsProcessedCount++;
            isProcessingStudent = false;
        }

        StartCoroutine(EvaluateEndingBranchesRoutine());
    }

    private IEnumerator FadeAvatarRoutine(bool fadeIn, float duration)
    {
        if (boothAvatarDisplayUI == null) yield break;
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

        // Ghi nhận cờ trạng thái kịch bản dựa trên tham chiếu file trực tiếp
        if (currentPersonProfile == day3SpecialCharacterProfile && !playerApproved)
        {
            isDay3SpecialCharacterDenied = true;
        }
        if (currentPersonProfile == day5CharacterAProfile)
        {
            isDay5CharAAccepted = playerApproved;
        }
        if (currentPersonProfile == day5CharacterBProfile)
        {
            isDay5CharBAccepted = playerApproved;
        }

        int violationCount = 0;
        string specificErrorString = "";

        bool missingMainCard = !currentPersonProfile.hasMainCard;
        bool nameMismatch = !missingMainCard && !currentPersonProfile.isNameMatching;
        bool dataMismatch = !missingMainCard && !currentPersonProfile.isDataMatching;
        bool majorMismatch = !missingMainCard && currentPersonProfile.personType == PersonType.Student && !currentPersonProfile.isMajorMatching;
        bool avatarMismatch = !missingMainCard && !currentPersonProfile.isAvatarMatching;
        bool paperExpired = !missingMainCard && !currentPersonProfile.IsPaperValid;

        if (missingMainCard) { violationCount++; specificErrorString = currentPersonProfile.personType == PersonType.Student ? "Sinh viên không xuất trình được thẻ sinh viên." : "Giảng viên không xuất trình được thẻ cán bộ."; }
        if (nameMismatch) { violationCount++; specificErrorString = "Thông tin họ và tên không đồng nhất."; }
        if (dataMismatch) { violationCount++; specificErrorString = "Dữ liệu không đồng nhất."; }
        if (majorMismatch) { violationCount++; specificErrorString = "Sai quy chế hành chính! Ngành học ghi trên thẻ không thuộc thẩm quyền khoa."; }
        if (avatarMismatch) { violationCount++; specificErrorString = "Ảnh chân dung không khớp."; }
        if (paperExpired) { violationCount++; specificErrorString = "Giấy tờ đã hết hạn."; }

        if (!missingMainCard)
        {
            if (currentDay >= 2)
            {
                if (!currentPersonProfile.hasGatePass)
                {
                    violationCount++;
                    specificErrorString = "Thiếu Giấy ra vào tòa nhà.";
                }
            }

            if (currentDay >= 3)
            {
                if (currentPersonProfile.personType == PersonType.Student && currentPersonProfile.passPurpose == GatePurpose.NghienCuu)
                {
                    if (!currentPersonProfile.hasLabCertificate)
                    {
                        violationCount++;
                        specificErrorString = "Sinh viên thiếu giấy chứng nhận thành viên Lab.";
                    }
                }
            }

            if (currentDay >= 4)
            {
                if (currentPersonProfile.personType == PersonType.Student && currentPersonProfile.isForeignStudent)
                {
                    if (!currentPersonProfile.hasIntlCertificate)
                    {
                        violationCount++;
                        specificErrorString = "Sinh viên quốc tế phải xuất trình thêm Giấy chứng nhận quốc tế.";
                    }
                }
            }

            switch (currentDay)
            {
                case 1:
                    if (currentPersonProfile.personType == PersonType.Student && currentPersonProfile.cardStudentFaculty != "Trường Điện - Điện tử")
                    {
                        violationCount++; 
                        specificErrorString = "Không phải sinh viên trường Điện - Điện tử.";
                    }
                    break;

                case 2:
                    if (currentPersonProfile.personType == PersonType.Student && currentPersonProfile.passPurpose == GatePurpose.NghienCuu)
                    {
                        violationCount++;
                        specificErrorString = "Sinh viên không được phép vào tòa nhà với mục đích nghiên cứu hôm nay.";
                    }
                    break;

                case 3:
                    if (currentPersonProfile.passPurpose == GatePurpose.HocTap_GiangDay)
                    {
                        violationCount++;
                        specificErrorString = "Không có hoạt động Học tập/Giảng dạy ngày hôm nay.";
                    }
                    break;
            }
        }

        HideAllLargeCards(false);

        bool studentHasAnyViolation = violationCount > 0;
        string finalTicketText = "";

        if (playerApproved && studentHasAnyViolation)
        {
            incorrectDecisionsCount++; 
            finalTicketText = violationCount >= 2 ? "Giấy tờ có nhiều lỗi vi phạm cùng lúc." : specificErrorString;
        }
        else if (!playerApproved && !studentHasAnyViolation)
        {
            incorrectDecisionsCount++; 
            finalTicketText = "Giấy tờ hợp lệ .";
        }
        else
        {
            correctDecisionsCount++; 
        }

        if (!string.IsNullOrEmpty(finalTicketText)) ShowCitationTicket(finalTicketText);

        if (DialogueManager.Instance != null && currentPersonProfile != null)
        {
            DialogueData targetDialogue = playerApproved ? currentPersonProfile.approveDialogue : currentPersonProfile.denyDialogue;
            if (targetDialogue != null)
            {
                DialogueManager.Instance.PlayDialogue(targetDialogue);
            }
        }

        hasPlayerDecided = true; 
    }

    // =========================================================================
    // 🔥 ĐỔ DỮ LIỆU CHỮ LÊN MÀN HÌNH TỔNG KẾT NGAY KHI CA TRỰC KHÉP LẠI
    // =========================================================================
    private IEnumerator EvaluateEndingBranchesRoutine()
    {
        yield return new WaitForSeconds(4.0f); 

        if (gameplayCanvas != null) gameplayCanvas.SetActive(false); 
        if (summaryCanvas != null)
        {
            summaryCanvas.SetActive(true); 

            // Tính toán số liệu hành chính
            int totalDecisions = correctDecisionsCount + incorrectDecisionsCount;
            int positiveScore = correctDecisionsCount * 10;
            int negativePenalty = incorrectDecisionsCount * 5;
            int netTotalScore = positiveScore - negativePenalty;
            float accuracyPercentage = (totalDecisions > 0) ? ((float)correctDecisionsCount / totalDecisions) * 100f : 100f;

            // Bắn chữ trực tiếp lên các ô TextMeshPro đã đấu nối trên khay tổng kết
            if (summaryRatioText != null) summaryRatioText.text = $"{correctDecisionsCount}/{totalDecisions}";
            if (summaryScoreText != null) summaryScoreText.text = $"{netTotalScore}đ";
            if (summaryThresholdText != null) summaryThresholdText.text = $"{totalScoreThreshold}đ";
            if (summaryAccuracyText != null) summaryAccuracyText.text = string.Format("{0:0.0}%", accuracyPercentage);

            DaySummaryController summaryController = summaryCanvas.GetComponent<DaySummaryController>();
            if (summaryController != null)
            {
                summaryController.DisplayStats(correctDecisionsCount, incorrectDecisionsCount);
            }
        }
    }

    // =========================================================================
    // 🔥 SỰ KIỆN KHI NGƯỜI CHƠI BẤM NÚT CONTINUE: QUÉT MẠCH RẼ NHÁNH ENDING CHÍNH XÁC
    // =========================================================================
    public void OnSummaryContinuePressed()
    {
        int totalDecisions = correctDecisionsCount + incorrectDecisionsCount;
        int positiveScore = correctDecisionsCount * 10;
        int negativePenalty = incorrectDecisionsCount * 5;
        int netTotalScore = positiveScore - negativePenalty;
        float accuracyPercentage = (totalDecisions > 0) ? ((float)correctDecisionsCount / totalDecisions) * 100f : 100f;

        // 🛑 LỚP ƯU TIÊN SỐ 1: Cốt truyện Ngày 5 - Duyệt nhầm nhân vật A (Tối cao, đè lên điểm số)
        if (currentDay == 5 && isDay5CharAAccepted)
        {
            if (!string.IsNullOrEmpty(sceneBadEndingDay5A)) SceneManager.LoadScene(sceneBadEndingDay5A);
            return;
        }

        // 🛑 LỚP ƯU TIÊN SỐ 2: Chỉ số KPI Hành chính (Độ chính xác < 50% HOẶC tổng điểm dưới ngưỡng sàn)
        if (accuracyPercentage < 50f || netTotalScore < totalScoreThreshold)
        {
            if (!string.IsNullOrEmpty(sceneBadEndingPerformance)) SceneManager.LoadScene(sceneBadEndingPerformance);
            return;
        }

        // 🛑 LỚP ƯU TIÊN SỐ 3: Cốt truyện phụ Ngày 3 - Từ chối nhân vật đặc biệt
        if (currentDay == 3 && isDay3SpecialCharacterDenied)
        {
            if (!string.IsNullOrEmpty(sceneBadEndingDay3Special)) SceneManager.LoadScene(sceneBadEndingDay3Special);
            return;
        }

        // 🛑 LỚP ƯU TIÊN SỐ 3: Cốt truyện phụ Ngày 5 - Phân tích lựa chọn nhân vật B (Khi A đã bị từ chối)
        if (currentDay == 5)
        {
            if (!isDay5CharBAccepted)
            {
                if (!string.IsNullOrEmpty(sceneGoodEnding2)) SceneManager.LoadScene(sceneGoodEnding2); // Từ chối A + Từ chối B
            }
            else
            {
                if (!string.IsNullOrEmpty(sceneGoodEnding1)) SceneManager.LoadScene(sceneGoodEnding1); // Từ chối A + Đồng ý B
            }
            return;
        }

        // =========================================================================
        // NẾU KPI TỐT VÀ KHÔNG DÍNH BẪY CỐT TRUYỆN -> CHO PHÉP CHUYỂN SANG NGÀY TIẾP THEO
        // =========================================================================
        if (!string.IsNullOrEmpty(nextRegularDaySceneName))
        {
            Debug.Log($"HỆ THỐNG: Vượt ca trực ca trực an toàn. Tiến sang Scene: {nextRegularDaySceneName}");
            SceneManager.LoadScene(nextRegularDaySceneName);
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
        if (display != null) { display.currentProfile = profile; display.smallCardDocType = docType; }

        CanvasGroup cg = newSmallCard.GetComponent<CanvasGroup>() ?? newSmallCard.AddComponent<CanvasGroup>();
        cg.alpha = 1f; cg.blocksRaycasts = true;

        UIDragDrop drag = newSmallCard.GetComponent<UIDragDrop>();
        if (drag != null) drag.SetStablePosition(cardRect.position);
        newSmallCard.tag = "SmallCard";
    }

    public void ReturnToSmallCard(Vector3 dropPosition, DocumentType docType)
    {
        GameObject targetPrefab = null; 
        switch (docType)
        {
            case DocumentType.MainCard:
                if (currentPersonProfile != null) targetPrefab = (currentPersonProfile.personType == PersonType.Student) ? studentCardSmallPrefab : staffCardSmallPrefab;
                else targetPrefab = studentCardSmallPrefab; 
                break;
            case DocumentType.GatePass:         targetPrefab = gatePassSmallPrefab; break;
            case DocumentType.IntlCertificate: targetPrefab = intlCertSmallPrefab; break;
            case DocumentType.LabCertificate:  targetPrefab = labCertSmallPrefab; break;
            case DocumentType.Newspaper:       targetPrefab = newspaperSmallPrefab; break;
        }
        if (targetPrefab == null) return;

        GameObject newSmallCard = Instantiate(targetPrefab, spawnPointSmallDesk);
        RectTransform cardRect = newSmallCard.GetComponent<RectTransform>();
        cardRect.anchoredPosition3D = new Vector3(Random.Range(-30f, 30f), Random.Range(-20f, 20f), 0f);
        cardRect.rotation = Quaternion.Euler(0, 0, Random.Range(-25f, 25f));

        CardDisplay smallDisplay = newSmallCard.GetComponent<CardDisplay>();
        if (smallDisplay != null) { smallDisplay.currentProfile = currentPersonProfile; smallDisplay.smallCardDocType = docType; }
        if (docType == DocumentType.Newspaper) newSmallCard.tag = "SmallNewspaper";
    }

    private void HideAllLargeCards(bool hideGlobalDeskTools = false)
    {
        if (largeStudentCardObject != null) largeStudentCardObject.SetActive(false);
        if (largeStaffCardObject != null) largeStaffCardObject.SetActive(false);
        if (largeGatePassObject != null) largeGatePassObject.SetActive(false);
        if (largeIntlCertObject != null) largeIntlCertObject.SetActive(false);
        if (largeLabCertObject != null) largeLabCertObject.SetActive(false);
        if (hideGlobalDeskTools)
        {
            if (largeNewspaperObject != null) largeNewspaperObject.SetActive(false);
            if (largeRulebookObject != null) largeRulebookObject.SetActive(false);
        }
    }

    private string FormatToDateString(string rawInput) { if (string.IsNullOrEmpty(rawInput)) return ""; string clean = rawInput.Trim(); if (clean.Length == 8) { return $"{clean.Substring(0, 2)}/{clean.Substring(2, 2)}/{clean.Substring(4, 4)}"; } return clean; }
    private void ShowCitationTicket(string errorLog) { if (citationPanelUI != null && citationReasonText != null) { citationReasonText.text = $"<color=#FF3B30><b>HÀNH VI PHẠM:</b></color>\n{errorLog}"; citationPanelUI.SetActive(true); citationPanelUI.transform.SetAsLastSibling(); if (citationHideCoroutine != null) StopCoroutine(citationHideCoroutine); citationHideCoroutine = StartCoroutine(AutoHideTicketRoutine(10f)); } }
    private IEnumerator AutoHideTicketRoutine(float delaySeconds) { yield return new WaitForSeconds(delaySeconds); HideCitationTicket(); citationHideCoroutine = null; }
    private void HideCitationTicket() { if (citationPanelUI != null) citationPanelUI.SetActive(false); }
    public void HandleHideCitationTicket() { if (citationPanelUI != null) citationPanelUI.SetActive(false); }
}