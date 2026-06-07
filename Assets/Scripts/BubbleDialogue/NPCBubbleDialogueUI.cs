/*using System;
using System.Collections;
using UnityEngine;

namespace LetMeIn.NPCDialogue
{
    public class NPCBubbleDialogueController : MonoBehaviour
    {
        [SerializeField]
        private NPCBubbleDialogueUI dialogueUI;

        private Coroutine dialogueCoroutine;
        private bool waitingForClick;
        private bool receivedClick;

        public bool IsPlaying { get; private set; }

        public event Action<NPCDialogueAction> OnDialogueAction;
        public event Action OnDialogueStarted;
        public event Action OnDialogueFinished;

        public void Play(
            NPCBubbleDialogueData data,
            NPCDialogueMoment moment)
        {
            if (data == null)
            {
                return;
            }

            NPCBubbleDialogueSequence sequence =
                data.GetSequence(moment);

            if (sequence == null || sequence.lines.Count == 0)
            {
                return;
            }

            if (dialogueCoroutine != null)
            {
                StopCoroutine(dialogueCoroutine);
            }

            dialogueCoroutine =
                StartCoroutine(PlaySequence(sequence));
        }

        private IEnumerator PlaySequence(
            NPCBubbleDialogueSequence sequence)
        {
            IsPlaying = true;
            OnDialogueStarted?.Invoke();

            dialogueUI.gameObject.SetActive(true);

            foreach (NPCBubbleDialogueLine line in sequence.lines)
            {
                dialogueUI.ShowLine(line);

                if (line.waitForClick)
                {
                    waitingForClick = true;
                    receivedClick = false;

                    yield return new WaitUntil(() => receivedClick);

                    waitingForClick = false;
                }
                else
                {
                    yield return new WaitForSeconds(
                        line.displayDuration);
                }

                ExecuteAction(line.actionAfterLine);
            }

            dialogueUI.HideAll();

            IsPlaying = false;
            dialogueCoroutine = null;

            OnDialogueFinished?.Invoke();
        }

        public void ContinueDialogue()
        {
            if (!waitingForClick)
            {
                return;
            }

            receivedClick = true;
        }

        private void ExecuteAction(NPCDialogueAction action)
        {
            if (action == NPCDialogueAction.None)
            {
                return;
            }

            OnDialogueAction?.Invoke(action);
        }

        public void StopDialogue()
        {
            if (dialogueCoroutine != null)
            {
                StopCoroutine(dialogueCoroutine);
                dialogueCoroutine = null;
            }

            IsPlaying = false;
            waitingForClick = false;
            receivedClick = false;

            dialogueUI.HideAll();
        }
    }
}
*/