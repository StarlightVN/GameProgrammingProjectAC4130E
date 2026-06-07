using UnityEngine;
using UnityEngine.SceneManagement;

namespace LetMeIn.Dialogue
{
    public class Day1GameplayStoryFlow : MonoBehaviour
    {
        [Header("Dialogue")]
        [SerializeField] private StoryDialogueController controller;
        [SerializeField] private StoryDialogueSequence tutorialSequence;
        [SerializeField] private StoryDialogueSequence ruleSequence;

        [Header("Gameplay")]
        [SerializeField] private GameObject gameplayRoot;

        [Header("Scene Transition")]
        [SerializeField] private string endingSceneName = "Day1_Ending";

        private StoryStep currentStep;
        private bool endingStarted;

        private enum StoryStep
        {
            None,
            Tutorial,
            Rule,
            Gameplay
        }

        private void Start()
        {
            if (controller == null)
            {
                Debug.LogError(
                    "Day1GameplayStoryFlow chưa được gán Controller.",
                    this
                );
                return;
            }

            if (tutorialSequence == null || ruleSequence == null)
            {
                Debug.LogError(
                    "Day1GameplayStoryFlow chưa được gán Tutorial hoặc Rule.",
                    this
                );
                return;
            }

            controller.onSequenceFinished.AddListener(
                HandleSequenceFinished
            );

            controller.onDialogueEvent.AddListener(
                HandleDialogueEvent
            );

            StartTutorial();
        }

        private void OnDestroy()
        {
            if (controller == null)
            {
                return;
            }

            controller.onSequenceFinished.RemoveListener(
                HandleSequenceFinished
            );

            controller.onDialogueEvent.RemoveListener(
                HandleDialogueEvent
            );
        }

        private void StartTutorial()
        {
            currentStep = StoryStep.Tutorial;

            SetGameplayActive(false);

            controller.HideDialogueBackground();
            controller.ShowDimOverlay(0.6f);
            controller.Play(tutorialSequence);
        }

        private void HandleSequenceFinished()
        {
            switch (currentStep)
            {
                case StoryStep.Tutorial:
                    currentStep = StoryStep.Rule;
                    controller.Play(ruleSequence);
                    break;

                case StoryStep.Rule:
                    currentStep = StoryStep.Gameplay;

                    controller.HideDimOverlay();
                    SetGameplayActive(true);
                    break;
            }
        }

        private void HandleDialogueEvent(string eventKey)
        {
            switch (eventKey)
            {
                case "Tutorial_CheckDocument":
                    Debug.Log(
                        "Hiển thị hướng dẫn kiểm tra giấy tờ."
                    );
                    break;

                case "ShowTutorialOverlay":
                    controller.ShowDimOverlay(0.6f);
                    break;

                case "HideTutorialOverlay":
                    controller.HideDimOverlay();
                    break;
            }
        }

        public void FinishDay()
        {
            if (endingStarted)
            {
                return;
            }

            endingStarted = true;

            if (string.IsNullOrWhiteSpace(endingSceneName))
            {
                Debug.LogError(
                    "Chưa gán tên scene Day1_Ending.",
                    this
                );
                return;
            }

            SceneManager.LoadScene(endingSceneName);
        }

        private void SetGameplayActive(bool active)
        {
            if (gameplayRoot != null)
            {
                gameplayRoot.SetActive(active);
            }
        }
    }
}