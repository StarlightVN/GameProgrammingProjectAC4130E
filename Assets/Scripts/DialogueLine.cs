using UnityEngine;
using System.Collections.Generic;

// Định nghĩa danh tính người nói
public enum Speaker { Player, NPC }

[System.Serializable]
public struct DialogueLine
{
    public Speaker speaker; // Ai nói? (Player hay NPC)
    [TextArea(2, 5)] 
    public string text;     // Nội dung câu thoại
}

// Tạo mục ScriptableObject riêng biệt cho Hội thoại trong Project
[CreateAssetMenu(fileName = "NewDialogue", menuName = "Custom/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [Tooltip("Danh sách các câu thoại đối đáp theo thứ tự từ trên xuống dưới (Độ dài tùy ý)")]
    public List<DialogueLine> lines = new List<DialogueLine>();
}