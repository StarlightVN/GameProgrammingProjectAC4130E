/* Story Flow Day 1 */
using UnityEngine;

namespace LetMeIn.Dialogue
{
    public class Day1StoryFlow : MonoBehaviour
    {
        [Header("Dialogue")]
        [SerializeField] private StoryDialogueController controller;

        [SerializeField] private StoryDialogueSequence openingSequence;
        [SerializeField] private StoryDialogueSequence tutorialSequence;
        [SerializeField] private StoryDialogueSequence ruleSequence;
        [SerializeField] private StoryDialogueSequence endingSequence;

        [Header("Gameplay")]
        [SerializeField] private GameObject gameplayRoot;
        [SerializeField] private GameObject explosionEffect;
        [SerializeField] private AudioSource sceneAudioSource;
        [SerializeField] private AudioClip explosionClip;
        [SerializeField] private AudioClip lightOnClip;

        private StoryStep currentStep;

        private enum StoryStep
        {
            None,
            Opening,
            Tutorial,
            Rule,
            Gameplay,
            Ending
        }

        private void Start()
        {
            if (controller == null)
            {
                Debug.LogError(
                    "Day1StoryFlow chưa được gán DialogueController.",
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

            StartOpening();
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

        public void StartOpening()
        {
            currentStep = StoryStep.Opening;
            SetGameplayActive(false);
            controller.Play(openingSequence);
        }

        public void StartEnding()
        {
            currentStep = StoryStep.Ending;
            SetGameplayActive(false);
            controller.Play(endingSequence);
        }

        private void HandleSequenceFinished()
        {
            switch (currentStep)
            {
                case StoryStep.Opening:
                    currentStep = StoryStep.Tutorial;
                    controller.Play(tutorialSequence);
                    break;

                case StoryStep.Tutorial:
                    currentStep = StoryStep.Rule;
                    controller.Play(ruleSequence);
                    break;

                case StoryStep.Rule:
                    currentStep = StoryStep.Gameplay;
                    SetGameplayActive(true);
                    break;

                case StoryStep.Ending:
                    FinishDay1();
                    break;
            }
        }

        private void HandleDialogueEvent(string eventKey)
        {
            switch (eventKey)
            {
                case "Explosion":
                    PlayExplosion();
                    break;

                case "LightOn":
                    TurnLightsOn();
                    break;

                case "Tutorial_CheckDocument":
                    ShowDocumentTutorial();
                    break;

                case "Day1Finished":
                    Debug.Log("Ngày 1 đã hoàn thành.");
                    break;
            }
        }

        private void PlayExplosion()
        {
            if (explosionEffect != null)
            {
                explosionEffect.SetActive(true);
            }

            if (
                sceneAudioSource != null &&
                explosionClip != null
            )
            {
                sceneAudioSource.PlayOneShot(explosionClip);
            }
        }

        private void TurnLightsOn()
        {
            if (explosionEffect != null)
            {
                explosionEffect.SetActive(false);
            }

            if (
                sceneAudioSource != null &&
                lightOnClip != null
            )
            {
                sceneAudioSource.PlayOneShot(lightOnClip);
            }
        }

        private void ShowDocumentTutorial()
        {
            Debug.Log(
                "Hiển thị hướng dẫn kéo giấy tờ sang bàn kiểm tra."
            );

            // Sau này mở panel tutorial tại đây.
        }

        private void SetGameplayActive(bool active)
        {
            if (gameplayRoot != null)
            {
                gameplayRoot.SetActive(active);
            }
        }

        private void FinishDay1()
        {
            currentStep = StoryStep.None;

            Debug.Log(
                "Lưu tiến trình và chuyển sang Day 2."
            );

            // SaveManager.Instance.CompleteDay(1);
            // SceneManager.LoadScene("Day2");
        }
    }
}