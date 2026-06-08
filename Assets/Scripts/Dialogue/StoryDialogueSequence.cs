using System;
using System.Collections.Generic;
using UnityEngine;

namespace LetMeIn.Dialogue
{
    // Định nghĩa các chế độ hiển thị đặc biệt cho câu thoại cutscene
    public enum DialogueDisplayMode
    {
        Normal,       // Hiện hộp thoại, tên nhân vật, avatar bình thường
        Narration,    // Chế độ người kể chuyện (Ẩn khung tên speaker)
        BlackScreen,  // Chế độ thoại trên nền đen hoàn toàn (tạo chiều sâu hồi tưởng)
        Notification  // Chế độ hiện chữ thông báo hệ thống ẩn hộp thoại chính
    }

    // =========================================================================
    // ĐÃ KHẮC PHỤC LỖI: Định nghĩa cấu trúc dữ liệu cho từng câu thoại đơn lẻ
    // =========================================================================
    [Serializable]
    public class StoryDialogueLine
    {
        [Header("Speaker Info")]
        [Tooltip("Tên của người đang nói câu thoại này (Để trống nếu là Narration)")]
        public string speakerName;
        [Tooltip("Bật tắt hình ảnh nhân vật")]
        public bool showAvatar = true;
        [Tooltip("Ảnh Sprite chân dung nhân vật cutscene")]
        public Sprite avatar;

        [Header("Content")]
        [TextArea(3, 10)]
        [Tooltip("Nội dung lời thoại hiển thị")]
        public string content;

        [Header("Display Settings")]
        [Tooltip("Lựa chọn kiểu hiển thị: Bình thường, Kể chuyện, Màn hình đen, hay Thông báo")]
        public DialogueDisplayMode displayMode = DialogueDisplayMode.Normal;

        [Header("Background Transition")]
        [Tooltip("Tích chọn nếu câu thoại này làm thay đổi hình nền phía sau bốt")]
        public bool changeBackground;
        [Tooltip("Tích chọn nếu muốn xóa sạch ảnh nền đưa về rỗng")]
        public bool clearBackground;
        [Tooltip("Ảnh nền mới sẽ thay thế khi câu thoại này bật lên")]
        public Sprite background;
        [Tooltip("Màu sắc bổ trợ cho ảnh nền (Mặc định là trắng hòa trộn)")]
        public Color backgroundColor = Color.white;

        [Header("Audio Elements")]
        [Tooltip("File âm thanh giọng đọc nhân vật (Voice Acting) nếu có")]
        public AudioClip voiceClip;
        [Tooltip("File âm thanh hiệu ứng (Tiếng bước chân, mở cửa...) kích hoạt ngay khi hiện câu này")]
        public AudioClip soundEffect;

        [Header("Triggers & Control")]
        [Tooltip("Chuỗi từ khóa để đồng bộ kích hoạt hiệu ứng vật lý bên ngoài (Ví dụ gõ chữ: Explosion, LightOn)")]
        public string eventKey;
        [Tooltip("Thời gian khóa chuột không cho người chơi bấm Skip nhanh (giây)")]
        public float inputLockDuration = 0f;
        [Tooltip("Tích chọn nếu muốn chữ tự chạy hết thời gian delay là tự chuyển sang câu tiếp theo không cần người chơi click")]
        public bool autoContinue;
        [Tooltip("Thời gian chờ (giây) trước khi tự động nhảy câu thoại")]
        public float autoContinueDelay = 2.0f;
    }

    // =========================================================================
    // ĐÃ KHẮC PHỤC LỖI: Tạo mục ScriptableObject tổng quản lý chuỗi câu thoại
    // =========================================================================
    [CreateAssetMenu(fileName = "NewStorySequence", menuName = "Custom/Story Dialogue Sequence")]
    public class StoryDialogueSequence : ScriptableObject
    {
        [Header("Global Settings")]
        [Tooltip("Tốc độ chạy chữ mặc định (giây/ký tự) cho toàn bộ chuỗi thoại này")]
        public float defaultCharacterInterval = 0.03f;
        [Tooltip("Tự động tắt ẩn khung UI hội thoại khi đọc đến câu cuối cùng")]
        public bool hideAfterFinished = true;

        [Header("Dialogue Flow List")]
        [Tooltip("Danh sách các câu thoại cutscene đối đáp xếp tuần tự từ trên xuống dưới (Độ dài vô hạn tùy biến)")]
        public List<StoryDialogueLine> lines = new List<StoryDialogueLine>();
    }
}