using System;
using System.Collections.Generic;
using UnityEngine;

namespace LetMeIn.Dialogue
{
    // Định nghĩa các chế độ hiển thị đặc biệt cho câu thoại cutscene.
    public enum DialogueDisplayMode
    {
        Normal,        // Hiện hộp thoại, tên nhân vật, avatar bình thường.
        Narration,     // Chế độ người kể chuyện, thường ẩn tên speaker/avatar.
        BlackScreen,   // Thoại trên nền đen hoàn toàn.
        Notification,  // Hiện thông báo hệ thống, ẩn hộp thoại chính.
        TitleCard      // Hiện thẻ tiêu đề ngày, text căn giữa trên nền đen.
    }

    [Serializable]
    public class StoryDialogueLine
    {
        [Header("Speaker Info")]
        [Tooltip("Tên của người đang nói câu thoại này. Để trống nếu là Narration.")]
        public string speakerName;

        [Tooltip("Bật/tắt hình ảnh nhân vật.")]
        public bool showAvatar = true;

        [Tooltip("Ảnh Sprite chân dung nhân vật cutscene.")]
        public Sprite avatar;

        [Header("Content")]
        [TextArea(3, 10)]
        [Tooltip("Nội dung lời thoại hiển thị.")]
        public string content;

        [Header("Display Settings")]
        [Tooltip("Kiểu hiển thị: Normal, Narration, BlackScreen, Notification hoặc TitleCard.")]
        public DialogueDisplayMode displayMode = DialogueDisplayMode.Normal;

        [Header("Background Transition")]
        [Tooltip("Tích chọn nếu câu thoại này làm thay đổi hình nền.")]
        public bool changeBackground;

        [Tooltip("Tích chọn nếu muốn xóa ảnh nền hiện tại.")]
        public bool clearBackground;

        [Tooltip("Ảnh nền mới sẽ thay thế khi câu thoại này bật lên.")]
        public Sprite background;

        [Tooltip("Màu nền hoặc màu hòa trộn với ảnh nền.")]
        public Color backgroundColor = Color.white;

        [Header("Audio Elements")]
        [Tooltip("File âm thanh giọng đọc nhân vật nếu có.")]
        public AudioClip voiceClip;

        [Tooltip("File âm thanh hiệu ứng, ví dụ tiếng nổ, mở cửa, bước chân.")]
        public AudioClip soundEffect;

        [Header("Triggers & Control")]
        [Tooltip("Từ khóa để kích hoạt hiệu ứng bên ngoài, ví dụ: Explosion, LightOn.")]
        public string eventKey;

        [Tooltip("Thời gian khóa input không cho người chơi bấm skip nhanh, tính bằng giây.")]
        [Min(0f)]
        public float inputLockDuration = 0f;

        [Tooltip("Tự động chuyển sang câu tiếp theo sau khi chữ hiện xong.")]
        public bool autoContinue;

        [Tooltip("Thời gian chờ trước khi tự động chuyển câu.")]
        [Min(0f)]
        public float autoContinueDelay = 2.0f;
    }

    [CreateAssetMenu(
        fileName = "NewStoryDialogue",
        menuName = "Let Me In/Dialogue/Story Dialogue Sequence"
    )]
    public class StoryDialogueSequence : ScriptableObject
    {
        [Header("Sequence Info")]
        [Tooltip("ID định danh của sequence, ví dụ DAY1_OPENING_SCENE.")]
        public string sequenceId;

        [Tooltip("Tên hiển thị của sequence, ví dụ Day1_Opening.")]
        public string sequenceTitle;

        [TextArea(2, 5)]
        [Tooltip("Mô tả ngắn nội dung sequence.")]
        public string description;

        [Header("Global Settings")]
        [Tooltip("Tốc độ chạy chữ mặc định, tính bằng giây/ký tự.")]
        [Min(0.001f)]
        public float defaultCharacterInterval = 0.03f;

        [Tooltip("Tự động ẩn UI hội thoại khi đọc đến câu cuối cùng.")]
        public bool hideAfterFinished = true;

        [Header("Dialogue Flow List")]
        [Tooltip("Danh sách các câu thoại cutscene, chạy tuần tự từ trên xuống dưới.")]
        public List<StoryDialogueLine> lines = new List<StoryDialogueLine>();
    }
}