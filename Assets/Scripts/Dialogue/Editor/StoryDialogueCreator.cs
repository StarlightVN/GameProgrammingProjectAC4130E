#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LetMeIn.Dialogue.Editor
{
    public static class StoryDialogueCreator
    {
        private const string FolderPath = "Assets/GameData/Dialogue";

        [MenuItem("Let Me In/Dialogue/Create All Story Dialogue Assets")]
        public static void CreateAllStoryDialogueAssets()
        {
            EnsureFolder("Assets", "GameData");
            EnsureFolder("Assets/GameData", "Dialogue");

            CreateDay1Opening();
            CreateDay2Opening();
            CreateDay3Opening();
            CreateDay4Opening();
            CreateDay5Opening();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log(
                "Đã tạo/cập nhật Story Dialogue GameData tại: " + FolderPath
            );
        }

        // ============================================================
        // DAY 1 OPENING
        // ============================================================

        private static void CreateDay1Opening()
        {
            StoryDialogueSequence sequence =
                ScriptableObject.CreateInstance<StoryDialogueSequence>();

            sequence.sequenceId = "DAY1_OPENING";
            sequence.sequenceTitle = "Day1_Opening";
            sequence.description =
                "Mở đầu ngày 1, giới thiệu Vũ và tutorial kiểm tra giấy tờ.";
            sequence.defaultCharacterInterval = 0.025f;

            sequence.lines = new List<StoryDialogueLine>
            {
                Narration(
                    "Năm 2026, thế giới bước vào kỉ nguyên vươn mình.",
                    true,
                    "ChangeBG_Era"
                ),

                Narration(
                    "Đại học Back khoa Hà Nội đã tiến vào trong top 36 QS Ranking trên toàn vũ trụ, thu hút hàng triệu sinh viên kì lạ từ khắp các hành tinh."
                ),

                Narration(
                    "Bạn là Vũ - con người, cổ đông năm 3 Đại học Back khoa Hà Nội - sắp bị đuổi học vì sắp 2 kì liên tiếp dưới 36 đ**."
                ),

                Dialogue(
                    "Vũ",
                    "Quả này sắp ăn cảnh cáo 3 rồi!"
                ),

                Narration(
                    "Với sự giới thiệu từ Uncle Hải hiện đang làm chủ nhiệm CLB Tình nguyện trong trường, bạn được sắp xếp đi hỗ trợ công việc bảo vệ cho tòa C7-xịn nhất trường."
                ),

                Narration(
                    "Nhiệm vụ của bạn là đảm bảo tất cả các sinh viên đều đeo thẻ sinh viên đầy đủ, được xác minh thân phận trước khi vào C-xịn nhất trường-7."
                ),

                Dialogue(
                    "Vũ",
                    "Sinh viên làm tình"
                ),

                Dialogue(
                    "Vũ",
                    "nguyện hết mình!"
                ),

                TitleCard(
                    "Ngày 1\nChủ nhật, ngày 31/5/2026",
                    "Day1TitleCard",
                    true,
                    1.5f
                ),

                Narration(
                    "C7-xịn nhất trường bắt đầu một ngày làm việc mới.",
                    true,
                    "ChangeBG_MainScreen"
                ),

                Dialogue(
                    "Uncle Hải",
                    "Vũ à Vũ, từ nay em sẽ tham gia hỗ trợ check thông tin người ra vào C7."
                ),

                Dialogue(
                    "Vũ",
                    "Dạ, em tưởng làm tình nguyện là xách nước bổ cam ạ?"
                ),

                Dialogue(
                    "Uncle Hải",
                    "Không em."
                ),

                Dialogue(
                    "Uncle Hải",
                    "Nhiệm vụ của em là đối chiếu thông tin trên <color=#FFD54A>Thẻ Sinh Viên</color> và các <color=#FFD54A>Giấy tờ</color> liên quan với <color=#FFD54A>Sổ tay hướng dẫn (STHD) </color> để quyết định cho Sinh Viên được vào C7 hay cook."
                ),

                Dialogue(
                    "Uncle Hải",
                    "Sau khi nhận được <color=#FFD54A>Giấy tờ</color> từ người ra vào ở <color=#FFD54A>Bàn Nhỏ</color> bên trái, em hãy kéo sang <color=#FFD54A>Bàn To</color> bên phải để check thông tin thật kỹ, rồi <color=#FFD54A>bấm nút</color> ở kia để mở cửa nhé. Không đủ thì đừng cho vào.",
                    eventKey: "Tutorial_CheckDocument"
                ),

                Dialogue(
                    "Uncle Hải",
                    "Nhớ là hôm nay là có Hội nghị Đổi mới sáng tạo ở sảnh C7 và tầng 8, nên <color=#FFD54A>chỉ có sinh viên SEEE và giảng viên được vào</color> nhé!"
                ),

                Dialogue(
                    "Vũ",
                    "Vậy là em cần kiểm tra <color=#FFD54A>Thẻ Sinh Viên</color> và <color=#FFD54A>Thẻ Cán Bộ</color> đúng không ạ?"
                ),

                Dialogue(
                    "Uncle Hải",
                    "Đúng rồi em. Kiểm tra kỹ vào, đừng có mở cửa sau cho đứa nào đấy!"
                ),

                Dialogue(
                    "Vũ",
                    "Có cần kiểm tra thần thái học thuật không sếp?"
                ),

                Dialogue(
                    "Uncle Hải",
                    "..."
                ),

                Dialogue(
                    "Uncle Hải",
                    "Thôi kiểm tra cái đó thì nửa trường không được vào."
                ),

                Dialogue(
                    "Vũ",
                    "Đã rõ thưa sếp!",
                    eventKey: "Day1OpeningFinished"
                )
            };

            SaveOrReplace(sequence, FolderPath + "/Day1_Opening.asset");
        }

        // ============================================================
        // DAY 2 OPENING = END DAY 1 + OPEN DAY 2
        // ============================================================

        private static void CreateDay2Opening()
        {
            StoryDialogueSequence sequence =
                ScriptableObject.CreateInstance<StoryDialogueSequence>();

            sequence.sequenceId = "DAY2_OPENING";
            sequence.sequenceTitle = "Day2_Opening";
            sequence.description =
                "Cuối ngày 1, sự cố mất điện và mở đầu ngày 2.";
            sequence.defaultCharacterInterval = 0.025f;

            sequence.lines = new List<StoryDialogueLine>
            {
                BlackScreen(
                    "",
                    "*Một tiếng nổ lớn vang lên*",
                    "Explosion",
                    true,
                    1.2f
                ),

                BlackScreen(
                    "Vũ",
                    "Cái quái gì vậy???"
                ),

                BlackScreen(
                    "Người qua đường 1",
                    "Vãi nãy có thằng làm nổ luôn cầu dao tổng kìa m."
                ),

                BlackScreen(
                    "Người qua đường 2",
                    "Oách xà lách. Chắc mới tạch Giải tích à?"
                ),

                BlackScreen(
                    "Người qua đường 1",
                    "Ông cháu học lại Lý thuyết mạch lần 3 rồi mà nghịch điện thế nào nổ luôn mới kinh."
                ),

                BlackScreen(
                    "Người qua đường 2",
                    "Chịu thợ. Này học lại lần 4 cho nhớ."
                ),

                BlackScreen(
                    "Vũ",
                    "Vãi chưởng thật =))))"
                ),

                Narration(
                    "[Đèn sáng lại]",
                    false,
                    "LightOn"
                ),

                Narration(
                    "Uncle Hải xuất hiện với mái tóc dựng ngược, tay cầm biên bản.",
                    false,
                    "ShowUncleHaiBurned"
                ),

                Dialogue(
                    "",
                    "*tiếng thở dài*",
                    false,
                    false,
                    "UncleHaiSigh"
                ),

                Dialogue(
                    "Uncle Hải",
                    "Vũ à…"
                ),

                Dialogue(
                    "Uncle Hải",
                    "Từ mai sinh viên và cán bộ muốn vào C7 phải có <color=#FFD54A>Giấy Phép ra vào có hiệu lực trong ngày.</color>"
                ),

                Dialogue(
                    "Vũ",
                    "Sao tự nhiên chặt vậy thầy?"
                ),

                Dialogue(
                    "Uncle Hải",
                    "Có sinh viên làm hỏng hệ thống điện chính của toàn bộ tầng 7 và tầng 8 rồi. Trong ngày mai trường sẽ sửa lại hệ thống điện của phòng nghiên cứu."
                ),

                Dialogue(
                    "Vũ",
                    "Ôi luật sinh ra từ những vụ nổ lớn."
                ),

                TitleCard(
                    "— Hết ngày 1 —",
                    "Day1EndTitle",
                    true,
                    1.2f
                ),

                TitleCard(
                    "Ngày 2\nThứ hai, ngày 1/6/2026",
                    "Day2TitleCard",
                    true,
                    1.5f
                ),

                Narration(
                    "Cổng C7 được dán một tờ giấy chữ viết tay do Vũ viết.",
                    true,
                    "ChangeBG_Day2_C7Gate"
                ),

                Narration(
                    "Trên giấy ghi: “Đang sửa điện. Đừng chạm vào cỏ ổ điện.”\n\nChữ “cỏ” bị gạch đi như kiểu viết nhầm."
                ),

                Dialogue(
                    "Uncle Hải",
                    "Vũ, hôm nay luật mới. Người ra vào cần phải có <color=#FFD54A>Giấy Phép có hiệu lực trong ngày.</color>"
                ),

                Dialogue(
                    "Vũ",
                    "Đã rõ thưa sếp!",
                    eventKey: "Day2OpeningFinished"
                )
            };

            SaveOrReplace(sequence, FolderPath + "/Day2_Opening.asset");
        }

        // ============================================================
        // DAY 3 OPENING = END DAY 2 + OPEN DAY 3
        // ============================================================

        private static void CreateDay3Opening()
        {
            StoryDialogueSequence sequence =
                ScriptableObject.CreateInstance<StoryDialogueSequence>();

            sequence.sequenceId = "DAY3_OPENING";
            sequence.sequenceTitle = "Day3_Opening";
            sequence.description =
                "Cuối ngày 2 và mở đầu Ngày hội sinh viên ngày 3.";
            sequence.defaultCharacterInterval = 0.025f;

            sequence.lines = new List<StoryDialogueLine>
            {
                Narration(
                    "Cuối ngày, vài sinh viên đi ngang qua cổng C7.",
                    true,
                    "ChangeBG_Day2_C7Gate_Evening"
                ),

                Dialogue(
                    "Người qua đường 1",
                    "Mai là Ngày hội sinh viên đấy, được nghỉ học nhé. Đi net không anh em?"
                ),

                Dialogue(
                    "Người qua đường 2",
                    "Ok đi làm tí Javalorant."
                ),

                Dialogue(
                    "Người qua đường 3",
                    "Điểm rèn luyện thì sao m?"
                ),

                Dialogue(
                    "Người qua đường 1",
                    "Quan trọng gì tạch Lý thuyết mạch lần 3 rồi. Chill."
                ),

                TitleCard(
                    "— Hết ngày 2 —",
                    "Day2EndTitle",
                    true,
                    1.2f
                ),

                TitleCard(
                    "Ngày 3\nThứ ba, ngày 2/6/2026",
                    "Day3TitleCard",
                    true,
                    1.5f
                ),

                Narration(
                    "Cổng C7 hôm nay được dán một tờ giấy viết tay mới của Vũ.",
                    true,
                    "ChangeBG_Day3_C7Gate"
                ),

                Narration(
                    "Trên giấy ghi: “Ngày hội sinh viên - Vui là chính, giấy tờ là bắt buộc.”"
                ),

                Dialogue(
                    "Uncle Hải",
                    "Nay là Ngày hội sinh viên."
                ),

                Dialogue(
                    "Vũ",
                    "Yasss, vậy là em không cần check giấy tờ nữa đúng không ạ?"
                ),

                Dialogue(
                    "Uncle Hải",
                    "Không, hôm nay sẽ có rất nhiều người, check cẩn thận vào. Hôm nay không có lớp học. Ai vào với mục đích đi học thì từ chối."
                ),

                Dialogue(
                    "Vũ",
                    "Vậy là <color=#FFD54A>chỉ được vào vì mục đích Sự kiện hoặc Nghiên cứu</color> ạ?"
                ),

                Dialogue(
                    "Uncle Hải",
                    "Đúng. Nhưng sinh viên nghiên cứu phải có <color=#FFD54A>Giấy Chứng Nhận Thành Viên Phòng Nghiên Cứu.</color>"
                ),

                Dialogue(
                    "Vũ",
                    "Nếu không có giấy thì sao ạ?"
                ),

                Dialogue(
                    "Uncle Hải",
                    "Thì dừng ở cổng Trần Đại Nghĩa kia kìa.",
                    eventKey: "Day3OpeningFinished"
                )
            };

            SaveOrReplace(sequence, FolderPath + "/Day3_Opening.asset");
        }

        // ============================================================
        // DAY 4 OPENING = END DAY 3 + OPEN DAY 4
        // ============================================================

        private static void CreateDay4Opening()
        {
            StoryDialogueSequence sequence =
                ScriptableObject.CreateInstance<StoryDialogueSequence>();

            sequence.sequenceId = "DAY4_OPENING";
            sequence.sequenceTitle = "Day4_Opening";
            sequence.description =
                "Cuối ngày 3, sinh viên trao đổi xuất hiện và mở đầu ngày 4.";
            sequence.defaultCharacterInterval = 0.025f;

            sequence.lines = new List<StoryDialogueLine>
            {
                Narration(
                    "Cuối ngày, khi Vũ đang chuẩn bị dọn cổng, một sinh viên lạ mặt xuất hiện trước C7.",
                    true,
                    "ChangeBG_Day3_C7Gate_Evening"
                ),

                Dialogue(
                    "Eimi-chan",
                    "Koko wa C7 desuka? *Đây là tòa C7 phải không ạ?*"
                ),

                Dialogue(
                    "Vũ",
                    "Hả?"
                ),

                Dialogue(
                    "Vũ",
                    "Hello Ajinomoto. Oắt can ai hép du?"
                ),

                Dialogue(
                    "Eimi-chan",
                    "C7 ni haite mo iidesu ka? *Tớ có thể vào C7 được không?*"
                ),

                Dialogue(
                    "Vũ",
                    "Nà ní?"
                ),

                Dialogue(
                    "Uncle Hải",
                    "Ồ, đây là sinh viên trao đổi mới đến trường mình hôm nay. Kể từ mai trường chúng ta sẽ đón đoàn sinh viên quốc tế."
                ),

                Dialogue(
                    "Vũ",
                    "Yayyy tuyệt quá!"
                ),

                TitleCard(
                    "— Hết ngày 3 —",
                    "Day3EndTitle",
                    true,
                    1.2f
                ),

                TitleCard(
                    "Ngày 4\nThứ tư, ngày 3/6/2026",
                    "Day4TitleCard",
                    true,
                    1.5f
                ),

                Narration(
                    "Cổng C7 hôm nay có banner: “Wellcome Internationnal Students”.",
                    true,
                    "ChangeBG_Day4_C7Gate"
                ),

                Dialogue(
                    "Uncle Hải",
                    "Vũ, từ hôm nay trường mình sẽ nhận sinh viên trao đổi quốc tế."
                ),

                Dialogue(
                    "Vũ",
                    "Vậy hôm nay em có phải nói tiếng Anh ạ? Hello? How are you?"
                ),

                Dialogue(
                    "Uncle Hải",
                    "Không cần. Em chỉ cần ngôn ngữ chung của nhân loại thôi."
                ),

                Dialogue(
                    "Vũ",
                    "Dạ? Tình yêu phải không thầy? Thật lãng mạn…"
                ),

                Dialogue(
                    "Uncle Hải",
                    "Giấy tờ."
                ),

                Dialogue(
                    "Vũ",
                    "À…"
                ),

                Dialogue(
                    "Uncle Hải",
                    "Sinh viên quốc tế cần xuất trình thêm <color=#FFD54A>Giấy Chứng Nhận Sinh Viên Quốc Tế</color>. Nhớ <color=#FFD54A>đối chiếu thông tin</color> giữa các loại giấy tờ."
                ),

                Dialogue(
                    "Vũ",
                    "Nếu các bạn ấy nói ‘Let me in?’ thì sao?"
                ),

                Dialogue(
                    "Uncle Hải",
                    "Em nói ‘Paper, please.’"
                ),

                Dialogue(
                    "Vũ",
                    "Nói vậy là bị đánh bản quyền đó thầy =)))))))))))",
                    eventKey: "Day4OpeningFinished"
                )
            };

            SaveOrReplace(sequence, FolderPath + "/Day4_Opening.asset");
        }

        // ============================================================
        // DAY 5 OPENING = END DAY 4 + OPEN DAY 5
        // ============================================================

        private static void CreateDay5Opening()
        {
            StoryDialogueSequence sequence =
                ScriptableObject.CreateInstance<StoryDialogueSequence>();

            sequence.sequenceId = "DAY5_OPENING";
            sequence.sequenceTitle = "Day5_Opening";
            sequence.description =
                "Cuối ngày 4 và mở đầu ngày 5 yên bình bất thường.";
            sequence.defaultCharacterInterval = 0.025f;

            sequence.lines = new List<StoryDialogueLine>
            {
                Narration(
                    "Sau bốn ngày hỗn loạn, C7 cuối cùng cũng vận hành tương đối ổn định.",
                    true,
                    "ChangeBG_Day4_C7Gate_Evening"
                ),

                Narration(
                    "Không ai làm nổ phòng thí nghiệm."
                ),

                Narration(
                    "Không ai giả làm cán bộ."
                ),

                Narration(
                    "Không ai dùng giấy phép tham gia sự kiện để vào phòng nghiên cứu."
                ),

                Narration(
                    "Uncle Hải nhìn bảng tổng kết và gật đầu."
                ),

                Dialogue(
                    "Uncle Hải",
                    "Vũ à, hôm nay làm ăn được đấy."
                ),

                Dialogue(
                    "Vũ",
                    "Uầy, tuyệt quá ạ."
                ),

                Dialogue(
                    "Vũ",
                    "Vậy mai có thay đổi gì mới không thầy?"
                ),

                Dialogue(
                    "Uncle Hải",
                    "Không. <color=#FFD54A>Ngày mai mọi thứ ổn định.</color>"
                ),

                Dialogue(
                    "Vũ",
                    "Ôi bình thường quá lại thành bất bình thường."
                ),

                Dialogue(
                    "Uncle Hải",
                    "Nhưng đừng chủ quan. Không có luật mới không có nghĩa là không có <color=#FFD54A>người</color> mới."
                ),

                Dialogue(
                    "Vũ",
                    "Ủa thế là sao ạ?"
                ),

                Narration(
                    "Uncle Hải nhìn xa xăm về phía C7."
                ),

                Dialogue(
                    "Uncle Hải",
                    "Có những thứ <color=#FFD54A>giấy tờ đúng…</color> nhưng <color=#FFD54A>con người thì sai.</color>"
                ),

                TitleCard(
                    "— Hết ngày 4 —",
                    "Day4EndTitle",
                    true,
                    1.2f
                ),

                TitleCard(
                    "Ngày 5\nThứ năm, ngày 4/6/2026",
                    "Day5TitleCard",
                    true,
                    1.5f
                ),

                Narration(
                    "Cổng C7 yên bình bất thường.",
                    true,
                    "ChangeBG_Day5_C7Gate"
                ),

                Dialogue(
                    "Vũ",
                    "Hôm nay yên tĩnh quá."
                ),

                Dialogue(
                    "Uncle Hải",
                    "Ừ. Không có luật mới."
                ),

                Dialogue(
                    "Vũ",
                    "Thật ạ?"
                ),

                Dialogue(
                    "Uncle Hải",
                    "Thật mà em."
                ),

                Dialogue(
                    "Vũ",
                    "Không giấy tờ mới, không sự kiện mới, không drama mới?"
                ),

                Dialogue(
                    "Uncle Hải",
                    "Không."
                ),

                Dialogue(
                    "Vũ",
                    "Thầy nói thế làm em còn sợ hơn."
                ),

                Dialogue(
                    "Uncle Hải",
                    "Em trưởng thành rồi đấy. Ở Đại học Trăm khoa, yên bình là dấu hiệu trước cơn drama, chú ý nhé em.",
                    eventKey: "Day5OpeningFinished"
                )
            };

            SaveOrReplace(sequence, FolderPath + "/Day5_Opening.asset");
        }

        // ============================================================
        // HELPERS
        // ============================================================

        private static StoryDialogueLine Dialogue(
            string speaker,
            string content,
            bool showAvatar = true,
            bool changeBackground = false,
            string eventKey = ""
        )
        {
            return new StoryDialogueLine
            {
                speakerName = speaker,
                content = content,
                displayMode = DialogueDisplayMode.Normal,
                showAvatar = showAvatar,
                changeBackground = changeBackground,
                eventKey = eventKey
            };
        }

        private static StoryDialogueLine Narration(
            string content,
            bool changeBackground = false,
            string eventKey = ""
        )
        {
            return new StoryDialogueLine
            {
                speakerName = string.Empty,
                content = content,
                displayMode = DialogueDisplayMode.Narration,
                showAvatar = false,
                changeBackground = changeBackground,
                eventKey = eventKey
            };
        }

        private static StoryDialogueLine BlackScreen(
            string speaker,
            string content,
            string eventKey = "",
            bool autoContinue = false,
            float autoContinueDelay = 1f
        )
        {
            return new StoryDialogueLine
            {
                speakerName = speaker,
                content = content,
                displayMode = DialogueDisplayMode.BlackScreen,
                showAvatar = false,
                eventKey = eventKey,
                autoContinue = autoContinue,
                autoContinueDelay = autoContinueDelay
            };
        }

        private static StoryDialogueLine TitleCard(
            string content,
            string eventKey = "",
            bool autoContinue = false,
            float autoContinueDelay = 1f
        )
        {
            return new StoryDialogueLine
            {
                speakerName = string.Empty,
                content = content,
                displayMode = DialogueDisplayMode.TitleCard,
                showAvatar = false,
                eventKey = eventKey,
                autoContinue = autoContinue,
                autoContinueDelay = autoContinueDelay
            };
        }

        private static void SaveOrReplace(
            StoryDialogueSequence sequence,
            string path
        )
        {
            StoryDialogueSequence existing =
                AssetDatabase.LoadAssetAtPath<StoryDialogueSequence>(path);

            if (existing == null)
            {
                AssetDatabase.CreateAsset(sequence, path);
                return;
            }

            EditorUtility.CopySerialized(sequence, existing);
            Object.DestroyImmediate(sequence);
            EditorUtility.SetDirty(existing);
        }

        private static void EnsureFolder(string parent, string folderName)
        {
            string fullPath = parent + "/" + folderName;

            if (!AssetDatabase.IsValidFolder(fullPath))
            {
                AssetDatabase.CreateFolder(parent, folderName);
            }
        }
    }
}

#endif
