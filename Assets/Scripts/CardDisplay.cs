using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 🔥 ĐÃ THÊM LẠI: Bộ phân loại gốc cho phôi nhỏ (Sửa lỗi CS0246 cho toàn dự án)
public enum DocumentType { MainCard, GatePass, IntlCertificate, LabCertificate, Newspaper}

// Bộ phân loại kiểu thẻ lớn standalone ngoài Hierarchy
public enum CardDisplayType { StudentCard, StaffCard, GatePass, IntlCertificate, LabCertificate }

public class CardDisplay : MonoBehaviour
{
    [Header("--- PHÂN LOẠI THẺ LỚN TỰ TRỊ ---")]
    [Tooltip("Hãy chọn đúng phân loại cho đối tượng thẻ lớn này ngoài Hierarchy")]
    public CardDisplayType thisCardType;

    [HideInInspector] public PersonProfile currentProfile; 
    public DocumentType smallCardDocType; // Ô biến dòng 15 bị lỗi đã được cứu sạch

    [Header("--- CẤU HÌNH TEXT THEO TỪNG LOẠI THẺ ---")]
    // Nhóm 1: Thẻ sinh viên
    public TextMeshProUGUI studentNameText;
    public TextMeshProUGUI studentIdText;
    public TextMeshProUGUI studentDobText;
    public TextMeshProUGUI studentFacultyText;
    public TextMeshProUGUI studentMajorText;
    public TextMeshProUGUI studentExpDateText;
    public Image studentPhotoImage;

    // Nhóm 2: Thẻ cán bộ
    public TextMeshProUGUI staffNameText;
    public TextMeshProUGUI staffFacultyText;
    public TextMeshProUGUI staffDobText;
    public Image staffPhotoImage;

    // Nhóm 3: Giấy ra vào
    public TextMeshProUGUI passNameText;
    public TextMeshProUGUI passIdOrTagText; 
    public TextMeshProUGUI passPurposeText; 
    public TextMeshProUGUI passDurationText;

    // Nhóm 4: Sinh viên quốc tế
    public TextMeshProUGUI intlNameText;
    public TextMeshProUGUI intlIdText;
    public TextMeshProUGUI intlFacultyText;

    // Nhóm 5: Phòng nghiên cứu Lab
    public TextMeshProUGUI labNameText;
    public TextMeshProUGUI labIdText;
    public TextMeshProUGUI labFacultyText;

    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }

        // TỰ ĐỘNG ĐỒNG BỘ: Rút hồ sơ nhân vật đang đứng ở bốt từ GameManager sang
        SchoolGateManager gateManager = Object.FindFirstObjectByType<SchoolGateManager>();
        if (gateManager != null)
        {
            currentProfile = gateManager.GetCurrentProfile();
        }

        RenderCardData();
    }

    public void RenderCardData()
    {
        if (currentProfile == null) return;

        // Tự trị đổ dữ liệu cô lập dựa trên phân loại được chọn ngoài Inspector
        switch (thisCardType)
        {
            case CardDisplayType.StudentCard:
                if (studentNameText != null) studentNameText.text = currentProfile.cardStudentName;
                if (studentIdText != null) studentIdText.text = currentProfile.cardStudentID;
                if (studentDobText != null) studentDobText.text = currentProfile.cardStudentDoB;
                if (studentFacultyText != null) studentFacultyText.text = currentProfile.cardStudentFaculty;
                if (studentMajorText != null) studentMajorText.text = currentProfile.cardStudentMajor;
                if (studentExpDateText != null) studentExpDateText.text = currentProfile.cardStudentExpirationDate;
                if (studentPhotoImage != null && currentProfile.cardStudentImage != null)
                {
                    studentPhotoImage.sprite = currentProfile.cardStudentImage;
                }
                break;

            case CardDisplayType.StaffCard:
                if (staffNameText != null) staffNameText.text = currentProfile.cardStaffName;
                if (staffFacultyText != null) staffFacultyText.text = currentProfile.cardStaffFaculty;
                if (staffDobText != null) staffDobText.text = currentProfile.cardStaffDoB;
                if (staffPhotoImage != null && currentProfile.cardStaffImage != null)
                {
                    staffPhotoImage.sprite = currentProfile.cardStaffImage;
                }
                break;

            case CardDisplayType.GatePass:
                if (passNameText != null) passNameText.text = currentProfile.passName;
                if (passIdOrTagText != null) passIdOrTagText.text = currentProfile.passIDOrStaffTag; 
                if (passDurationText != null) passDurationText.text = currentProfile.passDurationDate;
                if (passPurposeText != null)
                {
                    switch (currentProfile.passPurpose)
                    {
                        case GatePurpose.HocTap_GiangDay:
                            passPurposeText.text = currentProfile.personType == PersonType.Student ? "Học tập" : "Giảng dạy";
                            break;
                        case GatePurpose.SuKien:
                            passPurposeText.text = "Sự kiện";
                            break;
                        case GatePurpose.NghienCuu:
                            passPurposeText.text = "Nghiên cứu";
                            break;
                    }
                }
                break;

            case CardDisplayType.IntlCertificate:
                if (intlNameText != null) intlNameText.text = currentProfile.intlName;
                if (intlIdText != null) intlIdText.text = currentProfile.intlStudentID;
                if (intlFacultyText != null) intlFacultyText.text = currentProfile.intlFaculty;
                break;

            case CardDisplayType.LabCertificate:
                if (labNameText != null) labNameText.text = currentProfile.labName;
                if (labIdText != null) labIdText.text = currentProfile.labStudentID;
                if (labFacultyText != null) labFacultyText.text = currentProfile.labFaculty;
                break;
        }
    }
}