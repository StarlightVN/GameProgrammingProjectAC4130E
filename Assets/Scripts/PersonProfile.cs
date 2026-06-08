using UnityEngine;

public enum PersonType { Student, Staff }

// Phân loại các mục đích hợp lệ in trên Giấy ra vào tòa nhà
public enum GatePurpose { HocTap_GiangDay, SuKien, NghienCuu }

[CreateAssetMenu(fileName = "New Person Profile", menuName = "Gameplay/Person Profile")]
public class PersonProfile : ScriptableObject
{
    [Header("--- PHÂN LOẠI ĐỐI TƯỢNG ---")]
    public PersonType personType; 

    [Header("--- QUẢN LÝ SỰ TỒN TẠI CỦA GIẤY TỜ ---")]
    [Tooltip("Dùng để tạo kịch bản học sinh/cán bộ bị thiếu, quên mang theo một loại giấy tờ nào đó")]
    public bool hasMainCard = true;       
    public bool hasGatePass = false;       
    public bool hasIntlCertificate = false; 
    public bool hasLabCertificate = false;  
    
    [Header("--- HỆ THỐNG AVATAR BỐT TRỰC (BOOTH CAMERA) ---")]
    [Tooltip("Ảnh thực tế của nhân vật khi đứng ở bốt trực")]
    public Sprite boothAvatarImage;

    [Header("--- 1. THÔNG TIN THẺ SINH VIÊN (Chỉ dùng nếu là Student) ---")]
    public string cardStudentName;
    public string cardStudentID; 
    public string cardStudentDoB;
    public string cardStudentFaculty;
    public string cardStudentMajor;
    public string cardStudentExpirationDate;
    public Sprite cardStudentImage;

    [Header("--- 2. THÔNG TIN THẺ CÁN BỘ (Chỉ dùng nếu là Staff) ---")]
    public string cardStaffName;
    public string cardStaffFaculty; 
    public string cardStaffDoB;     
    public Sprite cardStaffImage;   

    [Header("--- 3. GIẤY RA VÀO TÒA NHÀ ---")]
    public string passName;
    [Tooltip("Nếu là Sinh viên thì nhập MSSV. Nếu là Cán bộ thì BẮT BUỘC nhập chữ 'Cán bộ'")]
    public string passIDOrStaffTag; 
    public GatePurpose passPurpose; 
    public string passDurationDate; 

    [Header("--- 4. GIẤY CHỨNG NHẬN SINH VIÊN QUỐC TẾ ---")]
    public string intlName;
    public string intlStudentID;
    public string intlFaculty;

    [Header("--- 5. GIẤY CHỨNG NHẬN THÀNH VIÊN PHÒNG NGHIÊN CỨU ---")]
    public string labName;
    public string labStudentID;
    public string labFaculty;

    [Header("--- BỘ KIỂM TRA LỖI THƯỜNG ---")]
    [Tooltip("Nếu tích chọn là TRUE, hệ thống coi như tất cả ảnh chân dung trên các giấy tờ trùng khớp với khuôn mặt nhân vật. Nếu FALSE, sẽ có giấy tờ bị gán ảnh sai hoặc thiếu ảnh")]
    public bool isPhotoMatching = true; 
    
    [Tooltip("Nếu tích chọn là TRUE, tất cả các mục Họ và tên trên mọi giấy tờ của người này đều trùng khớp với nhau. Nếu FALSE, sẽ có giấy tờ bị gõ sai tên")]
    public bool isNameMatching = true;

    [Tooltip("Nếu tích chọn là TRUE, tất cả các mục MSSV / Khoa / Ngành trên mọi giấy tờ của người này đều đồng nhất. Nếu FALSE, thông tin sẽ bị gõ sai hoặc thiếu trên một số giấy tờ")]
    public bool isDataMatching = true; 
    [Tooltip("Nếu tích chọn là True, ngành thuộc đơn vị. Nếu False, ngành không thuộc đơn vị hoặc bị gõ sai")]
    public bool isMajorMatching = true;

    [Tooltip("True là ảnh trên thẻ và ảnh bốt trực trùng khớp. False là kẻ gian hoán đổi ảnh/mạo danh.")]
    public bool isAvatarMatching = true;

    [Tooltip("True nghĩa là giấy tờ còn thời hạn hiệu lực. False nghĩa là giấy tờ đã hết hạn sử dụng.")]
    public bool IsPaperValid = true;

    [Header("--- HỆ THỐNG HỘI THOẠI ĐA DẠNG (DIALOGUE SYSTEM) ---")]
    [Tooltip("Chuỗi hội thoại khi vừa bắt đầu gặp mặt (Độ dài tùy biến)")]
    public DialogueData greetingDialogue;

    [Tooltip("Chuỗi hội thoại khi bấm ĐỒNG Ý (Độ dài tùy biến)")]
    public DialogueData approveDialogue;

    [Tooltip("Chuỗi hội thoại khi bấm TỪ CHỐI (Độ dài tùy biến)")]
    public DialogueData denyDialogue;

}