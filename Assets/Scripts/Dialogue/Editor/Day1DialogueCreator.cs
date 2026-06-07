/*Dialogue Creator for Day 1 Story Sequences*/

#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LetMeIn.Dialogue.Editor
{
    public static class Day1DialogueCreator
    {
        private const string FolderPath =
            "Assets/GameData/Dialogue";

        [MenuItem("Let Me In/Dialogue/Create Day 1 Dialogue Assets")]
        public static void CreateDay1DialogueAssets()
        {
            EnsureFolder("Assets", "GameData");
            EnsureFolder("Assets/GameData", "Dialogue");

            CreateOpeningSequence();
            CreateTutorialSequence();
            CreateDayRuleSequence();
            CreateEndingSequence();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Đã tạo dialogue Day 1 tại " + FolderPath);
        }

        private static void CreateOpeningSequence()
        {
            StoryDialogueSequence sequence =
                ScriptableObject.CreateInstance<StoryDialogueSequence>();

            sequence.sequenceId = "DAY1_OPENING";
            sequence.sequenceTitle = "Ngày 1 - Mở đầu";
            sequence.description =
                "Phần mở đầu ngày 1. Tạm trình bày dưới dạng dialogue.";
            sequence.defaultCharacterInterval = 0.025f;

            sequence.lines = new List<StoryDialogueLine>
            {
                Narration(
                    "Năm 2026, thế giới bước vào kỉ nguyên vươn mình.",
                    true,
                    "ChangeBG_Era"
                ),

                Narration(
                    "Đại học Back khoa Hà Nội đã tiến vào top 36 QS Ranking trên toàn vũ trụ, thu hút hàng triệu sinh viên kỳ lạ từ khắp các hành tinh."
                ),

                Narration(
                    "Bạn là Vũ - con người, cổ đông năm 3 Đại học Back khoa Hà Nội - sắp bị đuổi học vì sắp hai kỳ liên tiếp dưới 36 điểm."
                ),

                Dialogue(
                    "Vũ",
                    "Quả này sắp ăn cảnh cáo 3 rồi!"
                ),

                Narration(
                    "Với sự giới thiệu từ Uncle Hải, hiện đang làm chủ nhiệm CLB Tình nguyện trong trường, bạn được sắp xếp đi hỗ trợ công việc bảo vệ cho tòa C7-xịn nhất trường."
                ),

                Narration(
                    "Nhiệm vụ của bạn là đảm bảo tất cả sinh viên đều đeo thẻ đầy đủ và được xác minh thân phận trước khi vào C7."
                ),

                Dialogue(
                    "Vũ",
                    "Sinh viên làm tình"
                ),

                Dialogue(
                    "Vũ",
                    "nguyện hết mình!",
                    eventKey: "OpeningFinished"
                )
            };

            SaveOrReplace(
                sequence,
                FolderPath + "/Day1_Opening.asset"
            );
        }

        private static void CreateTutorialSequence()
        {
            StoryDialogueSequence sequence =
                ScriptableObject.CreateInstance<StoryDialogueSequence>();

            sequence.sequenceId = "DAY1_TUTORIAL";
            sequence.sequenceTitle = "Ngày 1 - Tutorial";
            sequence.description =
                "Uncle Hải hướng dẫn Vũ làm nhiệm vụ.";
            sequence.defaultCharacterInterval = 0.025f;

            sequence.lines = new List<StoryDialogueLine>
            {
                Dialogue(
                    "Uncle Hải",
                    "Vũ à Vũ, từ nay em sẽ tham gia hỗ trợ kiểm tra thông tin người ra vào C7.",
                    changeBackground: false,
                    eventKey: "ChangeBG_MainScreen"
                ),

                Dialogue(
                    "Vũ",
                    "Dạ, em tưởng làm tình nguyện là xách nước bổ cam ạ?"
                ),

                Dialogue(
                    "Uncle Hải",
                    "..."
                ),

                Dialogue(
                    "Uncle Hải",
                    "Nhiệm vụ của em là đối chiếu thông tin trên Thẻ Sinh Viên và các giấy tờ liên quan với Sổ tay hướng dẫn để quyết định cho người đó vào hay cook."
                ),

                Dialogue(
                    "Uncle Hải",
                    "Sau khi nhận được thẻ từ người ra vào, em hãy cầm sang bàn bên phải để kiểm tra thông tin thật kỹ, rồi bấm nút ở kia để mở cửa nhé. Không đủ thì đừng cho vào.",
                    eventKey: "Tutorial_CheckDocument"
                ),

                Dialogue(
                    "Uncle Hải",
                    "Nhớ là hôm nay có Hội nghị Đổi mới sáng tạo ở sảnh C7 và tầng 8, nên chỉ có sinh viên SEEE và cán bộ được vào nhé!"
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
                    eventKey: "TutorialDialogueFinished"
                )
            };

            SaveOrReplace(
                sequence,
                FolderPath + "/Day1_Tutorial.asset"
            );
        }

        private static void CreateDayRuleSequence()
        {
            StoryDialogueSequence sequence =
                ScriptableObject.CreateInstance<StoryDialogueSequence>();

            sequence.sequenceId = "DAY1_RULE";
            sequence.sequenceTitle = "Ngày 1 - Luật đầu ngày";
            sequence.description =
                "Thông báo luật chính thức của ngày 1.";
            sequence.defaultCharacterInterval = 0.018f;

            sequence.lines = new List<StoryDialogueLine>
            {
                Notification(
                    "Hôm nay là chủ nhật, Trường Điện - Điện tử tổ chức Hội nghị trong tòa nhà và có sinh viên đến tham dự.\n\nChỉ có sinh viên SEEE và cán bộ mới được vào tòa nhà.",
                    "ShowDay1Rule"
                ),

                Notification(
                    "Sinh viên phải xuất trình Thẻ Sinh Viên hợp lệ.\n\nCán bộ phải xuất trình Thẻ Cán Bộ hợp lệ.",
                    "ShowMainCardTutorial"
                ),

                Notification(
                    "Hãy đối chiếu tên, ảnh chân dung, khoa/trường, ngành học và thời hạn của giấy tờ trước khi đưa ra quyết định.",
                    "Day1RuleFinished"
                )
                
            };

            SaveOrReplace(
                sequence,
                FolderPath + "/Day1_Rule.asset"
            );
        }

        private static void CreateEndingSequence()
        {
            StoryDialogueSequence sequence =
                ScriptableObject.CreateInstance<StoryDialogueSequence>();

            sequence.sequenceId = "DAY1_ENDING";
            sequence.sequenceTitle = "Ngày 1 - Cuối ngày";
            sequence.description =
                "Sự cố mất điện và luật mới cho ngày 2.";
            sequence.defaultCharacterInterval = 0.025f;

            sequence.lines = new List<StoryDialogueLine>
            {
                BlackScreen(
                    string.Empty,
                    "[Tiếng nổ lớn]",
                    "Explosion",
                    autoContinue: true,
                    autoContinueDelay: 1.3f
                ),

                BlackScreen(
                    "Vũ",
                    "Cái quái gì vậy???"
                ),

                BlackScreen(
                    "Người qua đường 1",
                    "Vãi, nãy có thằng làm nổ luôn cầu dao tổng kìa m."
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

                Dialogue(
                    string.Empty,
                    "[Đèn sáng trở lại]",
                    showAvatar: false,
                    eventKey: "LightOn"
                ),

                Narration(
                    "Uncle Hải xuất hiện với mái tóc dựng ngược, tay cầm một tờ biên bản.",
                    false,
                    "ShowUncleHaiBurned"
                ),

                Dialogue(
                    string.Empty,
                    "*tiếng thở dài",
                    showAvatar: false,
                    eventKey: "UncleHaiSigh"
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
                    "Có sinh viên làm hỏng hệ thống điện chính của toàn bộ tầng 7 và tầng 8 rồi. Trong ngày mai trường sẽ sửa lại hệ thống điện của phòng nghiên cứu."
                ),

                Dialogue(
                    "Uncle Hải",
                    "Vì vậy, ngày mai sinh viên không được vào C7 với mục đích nghiên cứu."
                ),

                Dialogue(
                    "Vũ",
                    "Ôi, luật sinh ra từ những vụ nổ lớn."
                ),

                Notification(
                    "HẾT NGÀY 1",
                    "Day1Finished"
                )
            };

            SaveOrReplace(
                sequence,
                FolderPath + "/Day1_Ending.asset"
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

        private static StoryDialogueLine Notification(
            string content,
            string eventKey = ""
        )
        {
            return new StoryDialogueLine
            {
                speakerName = string.Empty,
                content = content,
                displayMode = DialogueDisplayMode.Notification,
                showAvatar = false,
                eventKey = eventKey
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