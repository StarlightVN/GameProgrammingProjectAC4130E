using System;
using System.Collections.Generic;
using UnityEngine;

namespace LetMeIn.Dialogue
{
    public enum DialogueDisplayMode
    {
        Normal,
        Narration,
        BlackScreen,
        Notification
    }

    [Serializable]
    public class StoryDialogueLine
    {
        [Header("Nội dung")]
        public string speakerName;

        [TextArea(3, 8)]
        public string content;

        [Header("Hiển thị")]
        public DialogueDisplayMode displayMode = DialogueDisplayMode.Normal;

        [Tooltip("Avatar của nhân vật đang nói.")]
        public Sprite avatar;

        [Tooltip("Có hiển thị avatar hay không.")]
        public bool showAvatar = true;

        [Tooltip("Có thay background tại câu thoại này không.")]
        public bool changeBackground;

        [Tooltip("Background mới. Để null nếu muốn chỉ đổi màu nền.")]
        public Sprite background;

        [Tooltip("Màu nền dùng khi không có background sprite.")]
        public Color backgroundColor = Color.white;

        [Tooltip("Xóa background sprite hiện tại khi chuyển đến dòng này.")]
        public bool clearBackground;

        [Header("Âm thanh")]
        public AudioClip voiceClip;
        public AudioClip soundEffect;

        [Header("Điều khiển gameplay")]
        [Tooltip(
            "Tên sự kiện được gửi ra ngoài, ví dụ: Explosion, LightOn, StartTutorial."
        )]
        public string eventKey;

        [Tooltip("Thời gian chờ trước khi cho phép người chơi tiếp tục.")]
        [Min(0f)]
        public float inputLockDuration;

        [Tooltip("Tự động chuyển câu sau khi đọc xong.")]
        public bool autoContinue;

        [Tooltip("Thời gian chờ trước khi tự động chuyển câu.")]
        [Min(0f)]
        public float autoContinueDelay = 1f;
    }

    [CreateAssetMenu(
        fileName = "NewStoryDialogue",
        menuName = "Let Me In/Dialogue/Story Dialogue Sequence"
    )]
    public class StoryDialogueSequence : ScriptableObject
    {
        [Header("Thông tin")]
        public string sequenceId;

        public string sequenceTitle;

        [TextArea(2, 5)]
        public string description;

        [Header("Cấu hình")]
        [Min(0.001f)]
        public float defaultCharacterInterval = 0.025f;

        public bool hideAfterFinished = true;

        [Header("Nội dung")]
        public List<StoryDialogueLine> lines = new();
    }
}