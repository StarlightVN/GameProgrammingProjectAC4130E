using UnityEngine;
using UnityEngine.SceneManagement;

namespace LetMeIn.Dialogue
{
    public class StoryScenePlayer : MonoBehaviour
    {
        [Header("Dialogue")]
        [SerializeField] private StoryDialogueController controller;
        [SerializeField] private StoryDialogueSequence sequence;

        [Header("Scene Transition")]
        [SerializeField] private string nextSceneName;
        [SerializeField] private bool playOnStart = true;

        private bool isTransitioning;

        private void Start()
        {
            if (controller == null)
            {
                Debug.LogError(
                    "StoryScenePlayer chưa được gán StoryDialogueController.",
                    this
                );
                return;
            }

            if (sequence == null)
            {
                Debug.LogError(
                    "StoryScenePlayer chưa được gán StoryDialogueSequence.",
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

            if (playOnStart)
            {
                controller.Play(sequence);
            }
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

        private void HandleSequenceFinished()
        {
            if (isTransitioning)
            {
                return;
            }

            isTransitioning = true;

            if (string.IsNullOrWhiteSpace(nextSceneName))
            {
                Debug.LogWarning(
                    "StoryScenePlayer chưa được gán Next Scene Name.",
                    this
                );
                return;
            }

            SceneManager.LoadScene(nextSceneName);
        }

        private void HandleDialogueEvent(string eventKey)
        {
            switch (eventKey)
            {
                case "Explosion":
                    Debug.Log("[StoryScenePlayer] Explosion event");
                    break;

                case "LightOn":
                    Debug.Log("[StoryScenePlayer] LightOn event");
                    break;

                case "ShowUncleHaiBurned":
                    Debug.Log("[StoryScenePlayer] Show burned Uncle Hai");
                    break;
            }
        }
    }
}