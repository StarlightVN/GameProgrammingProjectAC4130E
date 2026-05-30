using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StudentCardDisplay : MonoBehaviour
{
    [Header("HỒ SƠ MANG THEO (DÙNG ĐỂ GIỮ HỘ DỮ LIỆU)")]
    public StudentProfile currentProfile; // Thẻ nhỏ sẽ được GameManager nạp profile vào đây

    [Header("UI Elements (Chỉ kéo gán trên THẺ TO - THẺ NHỎ ĐỂ TRỐNG)")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI idText;
    public TextMeshProUGUI dobText;
    public Image studentImage;
    public TextMeshProUGUI facultyText;
    public TextMeshProUGUI majorText;
    public TextMeshProUGUI expirationDateText;

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

    // Hàm vẽ chữ: Chỉ Thẻ To thực thi, Thẻ nhỏ chạy qua an toàn vì các ô text bằng Null
    public void RenderCardData()
    {
        if (currentProfile == null) return;

        // Chỉ khi các ô TextMeshPro được kéo gán (trên Thẻ to) thì mới điền chữ
        if (nameText != null) nameText.text = currentProfile.CardStudentName;
        if (idText != null) idText.text = currentProfile.CardStudentID;
        if (dobText != null) dobText.text = currentProfile.CardDateOfBirth; 
        if (facultyText != null) facultyText.text = currentProfile.CardFaculty;
        if (majorText != null) majorText.text = currentProfile.CardMajor;
        if (expirationDateText != null) expirationDateText.text = currentProfile.CardExpirationDate; 
        
        if (studentImage != null && currentProfile.CardStudentImage != null)
        {
            studentImage.sprite = currentProfile.CardStudentImage;
        }
    }
}