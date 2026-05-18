using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "New Student Card", menuName = "Student Card")]
public class StudentCard : ScriptableObject
{
    public string StudentName;
    public string StudentID;
    public DateTime DateOfBirth;
    public Sprite StudentImage;
    public string Faculty;
    public string Major;
    public DateTime ExpirationDate;

}
