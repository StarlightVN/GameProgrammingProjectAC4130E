using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class StudentCardDisplay : MonoBehaviour
{
    public StudentCard studentCard;
    public Text nameText;
    public Text idText;
    public Text dobText;
    public Image studentImage;
    public Text facultyText;
    public Text majorText;
    public Text expirationDateText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nameText.text = studentCard.StudentName;
        idText.text = studentCard.StudentID;
        dobText.text = studentCard.DateOfBirth.ToString("dd/MM/yyyy");
        facultyText.text = studentCard.Faculty;
        majorText.text = studentCard.Major;
        expirationDateText.text = studentCard.ExpirationDate.ToString("dd/MM/yyyy");
    }

}
