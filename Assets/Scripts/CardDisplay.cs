using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Định nghĩa bộ phân loại giấy tờ cho phôi nhỏ ngoài Editor
public enum DocumentType { MainCard, GatePass, IntlCertificate, LabCertificate }

public class CardDisplay : MonoBehaviour
{
    [Header("--- PHÂN LOẠI TÀI LIỆU (CHỈ GÁN TRÊN PREFAB PHÔI NHỎ) ---")]
    [Tooltip("Nếu đây là Prefab phôi nhỏ ngoài Project, hãy chọn đúng loại giấy tờ tương ứng")]
    public DocumentType smallCardDocType;

    [Header("--- HỒ SƠ MANG THEO (TÚI CHỨA DỮ LIỆU CHUNG) ---")]
    public PersonProfile currentProfile; 

    [Header("--- 1. UI CỤM THÈ SINH VIÊN (Chỉ kéo gán trên THÈ TO) ---")]
    public GameObject studentCardGroup; 
    public TextMeshProUGUI studentNameText;
    public TextMeshProUGUI studentIdText;
    public TextMeshProUGUI studentDobText;
    public TextMeshProUGUI studentFacultyText;
    public TextMeshProUGUI studentMajorText;
    public TextMeshProUGUI studentExpDateText;
    public Image studentPhotoImage;

    [Header("--- 2. UI CỤM THÈ CÁN BỘ / GIẢNG VIÊN ---")]
    public GameObject staffCardGroup; 
    public TextMeshProUGUI staffNameText;
    public TextMeshProUGUI staffFacultyText;
    public TextMeshProUGUI staffDobText;
    public Image staffPhotoImage;

    [Header("--- 3. UI GIẤY RA VÀO TÒA NHÀ ---")]
    public GameObject gatePassGroup; 
    public TextMeshProUGUI passNameText;
    public TextMeshProUGUI passIdOrTagText; 
    public TextMeshProUGUI passPurposeText; 
    public TextMeshProUGUI passDurationText;

    [Header("--- 4. UI GIẤY CHỨNG NHẬN SV QUỐC TẾ ---")]
    public GameObject intlCertGroup; 
    public TextMeshProUGUI intlNameText;
    public TextMeshProUGUI intlIdText;
    public TextMeshProUGUI intlFacultyText;

    [Header("--- 5. UI GIẤY THÀNH VIÊN PHÒNG NGHIÊN CỨU ---")]
    public GameObject labCertGroup; 
    public TextMeshProUGUI labNameText;
    public TextMeshProUGUI labIdText;
    public TextMeshProUGUI labFacultyText;

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
    }

    // HÀM NẠP CHỮ TOÀN NĂNG: Đã xóa toàn bộ các dòng SetActive tự động cũ để phục vụ luồng lật mở từng tờ một
    public void RenderCardData()
    {
        if (currentProfile == null) return;

        // 1. Điền dữ liệu Thẻ sinh viên
        if (currentProfile.personType == PersonType.Student)
        {
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
        }
        // 2. Điền dữ liệu Thẻ cán bộ
        else if (currentProfile.personType == PersonType.Staff)
        {
            if (staffNameText != null) staffNameText.text = currentProfile.cardStaffName;
            if (staffFacultyText != null) staffFacultyText.text = currentProfile.cardStaffFaculty;
            if (staffDobText != null) staffDobText.text = currentProfile.cardStaffDoB;
            if (staffPhotoImage != null && currentProfile.cardStaffImage != null)
            {
                staffPhotoImage.sprite = currentProfile.cardStaffImage;
            }
        }

        // 3. Điền dữ liệu Giấy ra vào tòa nhà
        if (currentProfile.hasGatePass)
        {
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
        }

        // 4. Điền dữ liệu Giấy chứng nhận sinh viên quốc tế
        if (currentProfile.hasIntlCertificate)
        {
            if (intlNameText != null) intlNameText.text = currentProfile.intlName;
            if (intlIdText != null) intlIdText.text = currentProfile.intlStudentID;
            if (intlFacultyText != null) intlFacultyText.text = currentProfile.intlFaculty;
        }

        // 5. Điền dữ liệu Giấy thành viên phòng nghiên cứu
        if (currentProfile.hasLabCertificate)
        {
            if (labNameText != null) labNameText.text = currentProfile.labName;
            if (labIdText != null) labIdText.text = currentProfile.labStudentID;
            if (labFacultyText != null) labFacultyText.text = currentProfile.labFaculty;
        }
    }
}