#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LetMeIn.Dialogue.Editor
{
    public static class StoryDialogueCreator
    {
        private const string FolderPath =
            "Assets/GameData/Dialogue";

        [MenuItem("Let Me In/Dialogue/Create Story Scene Dialogue Assets")]
        public static void CreateStorySceneDialogueAssets()
        {
            EnsureFolder("Assets", "GameData");
            EnsureFolder("Assets/GameData", "Dialogue");

            CreateDay1OpeningSceneSequence();
            CreateDay2OpeningSceneSequence();
            CreateDay3OpeningSceneSequence();
            CreateDay4OpeningSceneSequence();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log(
                "Đã tạo story scene dialogue assets tại " + FolderPath
            );
        }

        private static void CreateDay1OpeningSceneSequence()
        {
            StoryDialogueSequence sequence =
                ScriptableObject.CreateInstance<StoryDialogueSequence>();

            sequence.sequenceId = "DAY1_OPENING_SCENE";
            sequence.sequenceTitle = "Day1_Opening";
            sequence.description =
                "Mở đầu ngày 1 + title card + tutorial trước Level_01.";
            sequence.defaultCharacterInterval = 0.025f;

            sequence.lines = new List<StoryDialogueLine>
            {
                Narration(
                    "Năm 2026, thế giới bước vào kỉ nguyên vươn mình.",
                    true,
                    "ChangeBG_Era"
                ),

                Narration(
                    "Đại học Back khoa Hà Nội đã tiến vào top 36 QS Ranking trên toàn vũ trụ, thu hút hàng triệu sinh viên kì lạ từ khắp các hành tinh."
                ),

                Narration(
                    "Bạn là Vũ - con người, cổ đông năm 3 Đại học Back khoa Hà Nội - sắp bị đuổi học vì sắp 2 kì liên tiếp dưới 36 đ**."
                ),

                Dialogue(
                    "Vũ",
                    "Quả này sắp ăn cảnh cáo 3 rồi!"
                ),

                Narration(
                    "Với sự giới thiệu từ Uncle Hải, hiện đang làm chủ nhiệm CLB Tình nguyện trong trường, bạn được sắp xếp đi hỗ trợ công việc bảo vệ cho tòa C7-xịn nhất trường."
                ),

                Narration(
                    "Nhiệm vụ của bạn là đảm bảo tất cả sinh viên đều đeo thẻ sinh viên đầy đủ và được xác minh thân phận trước khi vào C-xịn nhất trường-7."
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
                    "Day1TitleCard"
                ),

                Narration(
                    "Cổng C7-xịn nhất trường bắt đầu một ngày làm việc mới.",
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
                    ""
                ),

                Dialogue(
                    "Uncle Hải",
                    "Nhiệm vụ của em là đối chiếu thông tin trên Thẻ Sinh Viên và các Giấy tờ liên quan với Sổ tay hướng dẫn (STHD) để quyết định cho Sinh Viên vào hay cook."
                ),

                Dialogue(
                    "Uncle Hải",
                    "Sau khi nhận được thẻ từ người ra vào, em hãy cầm sang bàn bên phải để check thông tin thật kỹ, rồi bấm nút ở kia để mở cửa nhé. Không đủ thì đừng cho vào."
                ),

                Dialogue(
                    "Uncle Hải",
                    "Nhớ là hôm nay là có Hội nghị Đổi mới sáng tạo ở sảnh C7 và tầng 8, nên chỉ có sinh viên SEEE và giảng viên được vào nhé!"
                ),

                Dialogue(
                    "Vũ",
                    "Vậy là em cần kiểm tra Thẻ Sinh Viên và Thẻ Cán Bộ đúng không ạ?"
                ),

                Dialogue(
                    "Uncle Hải",
                    "Đúng rồi em, kiểm tra kỹ vào, đừng có mở cửa sau cho đứa nào đấy!"
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

            SaveOrReplace(
                sequence,
                FolderPath + "/Day1_Opening.asset"
            );
        }

        private static void CreateDay2OpeningSceneSequence()
        {
            StoryDialogueSequence sequence =
                ScriptableObject.CreateInstance<StoryDialogueSequence>();

            sequence.sequenceId = "DAY2_OPENING_SCENE";
            sequence.sequenceTitle = "Day2_Opening";
            sequence.description =
                "Cuối ngày 1 + title ngày 2 + mở đầu ngày 2.";
            sequence.defaultCharacterInterval = 0.025f;

            sequence.lines = new List<StoryDialogueLine>
            {
                Narration(
                    "Cuối ngày, một nhóm sinh viên SEEE khi thí nghiệm Lý thuyết mạch đã vô tình cắm mạch vào nguồn điện tổng.",
                    true,
                    "ChangeBG_C7_Evening"
                ),

                BlackScreen(
                    "",
                    "[Tiếng nổ lớn]",
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
                    "Uncle Hải xuất hiện với mái tóc dựng ngược, tay cầm một tờ biên bản.",
                    false,
                    "ShowUncleHaiBurned"
                ),

                Dialogue(
                    "",
                    "*tiếng thở dài",
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
                    "Từ mai sinh viên và cán bộ muốn vào C7 phải có Giấy Phép ra vào có hiệu lực trong ngày."
                ),

                Dialogue(
                    "Vũ",
                    "Sao tự nhiên chặt vậy thầy?"
                ),

                Dialogue(
                    "Uncle Hải",
                    "Có sinh viên làm hỏng hệ thống điện chính của toàn bộ tầng 7 tầng 8 rồi. Trong ngày mai trường sẽ sửa lại hệ thống điện của phòng nghiên cứu."
                ),

                Dialogue(
                    "Vũ",
                    "Ôi luật sinh ra từ những vụ nổ lớn."
                ),

                TitleCard(
                    "— Hết ngày 1 —",
                    "Day1EndTitle",
                    true,
                    1.0f
                ),

                TitleCard(
                    "Ngày 2\nThứ hai, ngày 1/6/2026",
                    "Day2TitleCard"
                ),

                Narration(
                    "Cổng C7 hôm nay được dán một tờ giấy chữ viết tay do Vũ viết.",
                    true,
                    "ChangeBG_Day2_C7Gate"
                ),

                Narration(
                    "Trên giấy ghi: “Đang sửa điện. Đừng chạm vào cỏ ổ điện.”\n\nChữ “cỏ” bị gạch đi như kiểu viết nhầm."
                ),

                Dialogue(
                    "Uncle Hải",
                    "Vũ, hôm nay luật mới."
                ),

                Dialogue(
                    "Vũ",
                    "Đã rõ thưa sếp!",
                    eventKey: "Day2OpeningFinished"
                )
            };

            SaveOrReplace(
                sequence,
                FolderPath + "/Day2_Opening.asset"
            );
        }

        private static void CreateDay3OpeningSceneSequence()
        {
            StoryDialogueSequence sequence =
                ScriptableObject.CreateInstance<StoryDialogueSequence>();

            sequence.sequenceId = "DAY3_OPENING_SCENE";
            sequence.sequenceTitle = "Day3_Opening";
            sequence.description =
                "Cuối ngày 2 + title ngày 3 + mở đầu ngày 3.";
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
                    1.0f
                ),

                TitleCard(
                    "Ngày 3\nThứ ba, ngày 2/6/2026",
                    "Day3TitleCard"
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
                    "Vậy là chỉ được vào vì mục đích Sự kiện hoặc Nghiên cứu ạ?"
                ),

                Dialogue(
                    "Uncle Hải",
                    "Đúng. Nhưng sinh viên nghiên cứu phải có Giấy Chứng Nhận Thành Viên Phòng Nghiên Cứu."
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

            SaveOrReplace(
                sequence,
                FolderPath + "/Day3_Opening.asset"
            );
        }

        private static void CreateDay4OpeningSceneSequence()
        {
            StoryDialogueSequence sequence =
                ScriptableObject.CreateInstance<StoryDialogueSequence>();

            sequence.sequenceId = "DAY4_OPENING_SCENE";
            sequence.sequenceTitle = "Day4_Opening";
            sequence.description =
                "Cuối ngày 3, sinh viên trao đổi quốc tế xuất hiện. Đây là cầu nối sang ngày 4.";
            sequence.defaultCharacterInterval = 0.025f;

            sequence.lines = new List<StoryDialogueLine>
            {
                Narration(
                    "Cuối ngày, khi Vũ đang chuẩn bị dọn cổng, một sinh viên lạ mặt xuất hiện trước C7.",
                    true,
                    "ChangeBG_Day3_C7Gate_Evening"
                ),

                Dialogue(
                    "Eimi Fukada",
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
                    "Eimi Fukada",
                    "*Tớ muốn vào C7*"
                ),

                Dialogue(
                    "Vũ",
                    "Nà ní?"
                ),

                Dialogue(
                    "Uncle Hải",
                    "Ồ đây là sinh viên trao đổi mới đến trường mình hôm nay. Kể từ mai trường chúng ta sẽ đón đoàn sinh viên quốc tế."
                ),

                Dialogue(
                    "Vũ",
                    "Yayyy",
                    eventKey: "Day4OpeningFinished"
                )
            };

            SaveOrReplace(
                sequence,
                FolderPath + "/Day4_Opening.asset"
            );
        }

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

        private static void EnsureFolder(
            string parent,
            string folderName
        )
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