/*
Đây là controller chính:

Chạy hiệu ứng typewriter.
Click/Space khi chữ đang chạy → hiện toàn bộ câu.
Click/Space khi câu đã đầy đủ → sang câu tiếp.
Chỉ hiện >> khi chữ đã chạy xong.
Hiển thị avatar tương ứng.
Hỗ trợ narrator, notification và màn hình đen.
Phát sự kiện để scene xử lý tiếng nổ, bật đèn, bắt đầu tutorial hoặc gameplay.
*/

using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace LetMeIn.Dialogue
{
    public class StoryDialogueController : MonoBehaviour
    {
        [Serializable]
        public class DialogueEvent : UnityEvent<string>
        {
        }

        [Header("Root")]
        [SerializeField] private GameObject dialogueRoot;

        [Header("Background")]
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image blackOverlay;

        [Header("Overlay")]
        [SerializeField] private Image dimOverlay;

        [Header("Avatar")]
        [SerializeField] private GameObject avatarRoot;
        [SerializeField] private Image avatarImage;

        [Header("Dialogue Box")]
        [SerializeField] private GameObject dialogueBox;
        [SerializeField] private TMP_Text speakerNameText;
        [SerializeField] private TMP_Text dialogueContentText;
        [SerializeField] private GameObject continueIndicator;

        [Header("Notification")]
        [SerializeField] private GameObject notificationRoot;
        [SerializeField] private TMP_Text notificationText;

        [Header("Audio")]
        [SerializeField] private AudioSource voiceAudioSource;
        [SerializeField] private AudioSource soundEffectAudioSource;

        [Header("Input")]
        [Tooltip("Button trong suốt phủ toàn màn hình.")]
        [SerializeField] private Button screenClickButton;

        [Header("Events")]
        public DialogueEvent onDialogueEvent;
        public UnityEvent onSequenceStarted;
        public UnityEvent onSequenceFinished;

        public bool IsPlaying => currentSequence != null;
        public bool IsTyping => isTyping;
        public int CurrentLineIndex => currentLineIndex;

        private StoryDialogueSequence currentSequence;
        private StoryDialogueLine currentLine;

        private int currentLineIndex = -1;

        private bool isTyping;
        private bool inputLocked;
        private bool waitingForAutoContinue;

        private Coroutine typingCoroutine;
        private Coroutine inputLockCoroutine;
        private Coroutine autoContinueCoroutine;

        private void Awake()
        {
            if (screenClickButton != null)
            {
                screenClickButton.onClick.AddListener(Advance);
            }

            SetContinueIndicator(false);

            if (notificationRoot != null)
            {
                notificationRoot.SetActive(false);
            }

            if (blackOverlay != null)
            {
                blackOverlay.gameObject.SetActive(false);
            }

            if (dimOverlay != null)
            {
                dimOverlay.gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (screenClickButton != null)
            {
                screenClickButton.onClick.RemoveListener(Advance);
            }
        }

        private void Update()
        {
            if (!IsPlaying)
            {
                return;
            }

            bool pressedSpace = false;

#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                pressedSpace = Keyboard.current.spaceKey.wasPressedThisFrame;
            }
#else
            pressedSpace = Input.GetKeyDown(KeyCode.Space);
#endif

            if (pressedSpace)
            {
                Advance();
            }
        }

        public void Play(StoryDialogueSequence sequence)
        {
            if (sequence == null)
            {
                Debug.LogError(
                    "Không thể phát dialogue vì StoryDialogueSequence đang null.",
                    this
                );
                return;
            }

            StopCurrentSequence(false);

            currentSequence = sequence;
            currentLineIndex = -1;

            if (dialogueRoot != null)
            {
                dialogueRoot.SetActive(true);
            }

            onSequenceStarted?.Invoke();
            ShowNextLine();
        }

        public void Advance()
        {
            if (!IsPlaying || inputLocked)
            {
                return;
            }

            if (isTyping)
            {
                CompleteCurrentLineImmediately();
                return;
            }

            if (waitingForAutoContinue)
            {
                StopAutoContinue();
            }

            ShowNextLine();
        }

        public void SkipSequence()
        {
            if (!IsPlaying)
            {
                return;
            }

            FinishSequence();
        }

        public void StopCurrentSequence(bool hideUI = true)
        {
            StopRunningCoroutines();

            currentSequence = null;
            currentLine = null;
            currentLineIndex = -1;

            isTyping = false;
            inputLocked = false;
            waitingForAutoContinue = false;

            StopAudio();

            if (hideUI && dialogueRoot != null)
            {
                dialogueRoot.SetActive(false);
            }
        }

        private void ShowNextLine()
        {
            if (currentSequence == null)
            {
                return;
            }

            currentLineIndex++;

            if (currentLineIndex >= currentSequence.lines.Count)
            {
                FinishSequence();
                return;
            }

            currentLine = currentSequence.lines[currentLineIndex];

            if (currentLine == null)
            {
                Debug.LogWarning(
                    $"Dialogue line {currentLineIndex} đang null.",
                    currentSequence
                );

                ShowNextLine();
                return;
            }

            StopRunningCoroutines();
            ApplyCurrentLinePresentation();
            PlayCurrentLineAudio();
            InvokeCurrentLineEvent();

            float lockDuration = Mathf.Max(0f, currentLine.inputLockDuration);

            if (lockDuration > 0f)
            {
                inputLockCoroutine = StartCoroutine(
                    LockInputRoutine(lockDuration)
                );
            }

            typingCoroutine = StartCoroutine(TypeCurrentLineRoutine());
        }

        private void ApplyCurrentLinePresentation()
        {
            SetContinueIndicator(false);

            if (dialogueContentText != null)
            {
                dialogueContentText.text = string.Empty;
            }

            if (notificationText != null)
            {
                notificationText.text = string.Empty;
            }

            bool isNotification =
                currentLine.displayMode == DialogueDisplayMode.Notification;

            bool isBlackScreen =
                currentLine.displayMode == DialogueDisplayMode.BlackScreen;

            bool isNarration =
                currentLine.displayMode == DialogueDisplayMode.Narration;

            if (dialogueBox != null)
            {
                dialogueBox.SetActive(!isNotification);
            }

            if (notificationRoot != null)
            {
                notificationRoot.SetActive(isNotification);
            }

            if (blackOverlay != null)
            {
                blackOverlay.gameObject.SetActive(isBlackScreen);
                blackOverlay.color = Color.black;
            }

            ApplyBackground();

            if (speakerNameText != null)
            {
                bool hasSpeaker =
                    !string.IsNullOrWhiteSpace(currentLine.speakerName);

                speakerNameText.gameObject.SetActive(
                    hasSpeaker && !isNarration
                );

                speakerNameText.text = currentLine.speakerName;
            }

            bool shouldShowAvatar =
                currentLine.showAvatar &&
                currentLine.avatar != null &&
                !isNarration &&
                !isBlackScreen &&
                !isNotification;

            if (avatarRoot != null)
            {
                avatarRoot.SetActive(shouldShowAvatar);
            }

            if (avatarImage != null)
            {
                avatarImage.sprite = currentLine.avatar;
                avatarImage.enabled = shouldShowAvatar;
            }
        }

        private void ApplyBackground()
        {
            if (backgroundImage == null || !currentLine.changeBackground)
            {
                return;
            }

            if (currentLine.clearBackground)
            {
                backgroundImage.sprite = null;
            }
            else if (currentLine.background != null)
            {
                backgroundImage.sprite = currentLine.background;
            }

            backgroundImage.color = currentLine.backgroundColor;

            backgroundImage.enabled =
                backgroundImage.sprite != null ||
                currentLine.backgroundColor.a > 0f;
        }

        private IEnumerator TypeCurrentLineRoutine()
        {
            isTyping = true;

            TMP_Text targetText =
                currentLine.displayMode == DialogueDisplayMode.Notification
                    ? notificationText
                    : dialogueContentText;

            if (targetText == null)
            {
                isTyping = false;
                yield break;
            }

            string fullContent = currentLine.content ?? string.Empty;

            targetText.text = fullContent;
            targetText.maxVisibleCharacters = 0;
            targetText.ForceMeshUpdate();

            int totalCharacters = targetText.textInfo.characterCount;

            float interval = Mathf.Max(
                0.001f,
                currentSequence.defaultCharacterInterval
            );

            for (int i = 0; i <= totalCharacters; i++)
            {
                targetText.maxVisibleCharacters = i;
                yield return new WaitForSecondsRealtime(interval);
            }

            FinishTyping();
        }

        private void CompleteCurrentLineImmediately()
        {
            if (!isTyping)
            {
                return;
            }

            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }

            TMP_Text targetText =
                currentLine.displayMode == DialogueDisplayMode.Notification
                    ? notificationText
                    : dialogueContentText;

            if (targetText != null)
            {
                targetText.text = currentLine.content ?? string.Empty;
                targetText.maxVisibleCharacters = int.MaxValue;
            }

            FinishTyping();
        }

        private void FinishTyping()
        {
            typingCoroutine = null;
            isTyping = false;

            SetContinueIndicator(true);

            if (currentLine != null && currentLine.autoContinue)
            {
                autoContinueCoroutine = StartCoroutine(
                    AutoContinueRoutine(currentLine.autoContinueDelay)
                );
            }
        }

        private IEnumerator AutoContinueRoutine(float delay)
        {
            waitingForAutoContinue = true;

            yield return new WaitForSecondsRealtime(
                Mathf.Max(0f, delay)
            );

            waitingForAutoContinue = false;
            autoContinueCoroutine = null;

            ShowNextLine();
        }

        private IEnumerator LockInputRoutine(float duration)
        {
            inputLocked = true;

            yield return new WaitForSecondsRealtime(duration);

            inputLocked = false;
            inputLockCoroutine = null;
        }

        private void InvokeCurrentLineEvent()
        {
            if (string.IsNullOrWhiteSpace(currentLine.eventKey))
            {
                return;
            }

            onDialogueEvent?.Invoke(currentLine.eventKey);
        }

        private void PlayCurrentLineAudio()
        {
            if (voiceAudioSource != null)
            {
                voiceAudioSource.Stop();
                voiceAudioSource.clip = currentLine.voiceClip;

                if (currentLine.voiceClip != null)
                {
                    voiceAudioSource.Play();
                }
            }

            if (
                soundEffectAudioSource != null &&
                currentLine.soundEffect != null
            )
            {
                soundEffectAudioSource.PlayOneShot(
                    currentLine.soundEffect
                );
            }
        }

        private void FinishSequence()
        {
            StoryDialogueSequence finishedSequence = currentSequence;

            StopRunningCoroutines();
            StopAudio();

            currentSequence = null;
            currentLine = null;
            currentLineIndex = -1;

            isTyping = false;
            inputLocked = false;
            waitingForAutoContinue = false;

            SetContinueIndicator(false);

            if (
                finishedSequence != null &&
                finishedSequence.hideAfterFinished &&
                dialogueRoot != null
            )
            {
                dialogueRoot.SetActive(false);
            }

            onSequenceFinished?.Invoke();
        }

        private void SetContinueIndicator(bool visible)
        {
            if (continueIndicator != null)
            {
                continueIndicator.SetActive(visible);
            }
        }

        private void StopRunningCoroutines()
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }

            if (inputLockCoroutine != null)
            {
                StopCoroutine(inputLockCoroutine);
                inputLockCoroutine = null;
            }

            StopAutoContinue();
        }

        private void StopAutoContinue()
        {
            if (autoContinueCoroutine != null)
            {
                StopCoroutine(autoContinueCoroutine);
                autoContinueCoroutine = null;
            }

            waitingForAutoContinue = false;
        }

        private void StopAudio()
        {
            if (voiceAudioSource != null)
            {
                voiceAudioSource.Stop();
                voiceAudioSource.clip = null;
            }

            if (soundEffectAudioSource != null)
            {
                soundEffectAudioSource.Stop();
            }
        }

        public void ShowDimOverlay(float alpha = 0.6f)
        {
            if (dimOverlay == null)
            {
                return;
            }

            Color color = dimOverlay.color;
            color.r = 0f;
            color.g = 0f;
            color.b = 0f;
            color.a = Mathf.Clamp01(alpha);

            dimOverlay.color = color;
            dimOverlay.gameObject.SetActive(true);
        }

        public void HideDimOverlay()
        {
            if (dimOverlay != null)
            {
                dimOverlay.gameObject.SetActive(false);
            }
        }

        public void HideDialogueBackground()
        {
            if (backgroundImage == null)
            {
                return;
            }

            backgroundImage.sprite = null;
            backgroundImage.color = Color.clear;
            backgroundImage.gameObject.SetActive(false);
        }
    }
}