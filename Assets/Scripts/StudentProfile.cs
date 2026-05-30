using UnityEngine;

[CreateAssetMenu(fileName = "New Student Profile", menuName = "Student Profile")]
public class StudentProfile : ScriptableObject
{
    [Header("TRẠNG THÁI GIẤY TỜ")]
    public bool HasStudentCard = true; 

    [Header("DỮ LIỆU ĐỐI CHIẾU LỖI LẼ THƯỜNG")]

    public bool IsPhotoMatching = true; 
    
    public bool IsNameMatching = true;  

    [Header("THÔNG TIN TRÊN THẺ SINH VIÊN THỰC TẾ")]
    public string CardStudentName;     
    public string CardStudentID;      
    public string CardDateOfBirth;     
    public Sprite CardStudentImage;    
    public string CardFaculty;         
    public string CardMajor;           
    public string CardExpirationDate;  
}