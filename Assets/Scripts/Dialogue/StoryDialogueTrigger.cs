/*
Trigger để phátDialogue. 
*/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace LetMeIn.Dialogue
{
    public class StoryDialogueTrigger : MonoBehaviour
    {
        [SerializeField] private StoryDialogueController controller;
        [SerializeField] private StoryDialogueSequence sequence;

        [Header("Trigger")]
        [SerializeField] private bool playOnStart = true;

        private void Start()
        {
            if (playOnStart)
            {
                PlayDialogue();
            }
        }

        public void PlayDialogue()
        {
            if (controller == null)
            {
                Debug.LogError(
                    "StoryDialogueTrigger chưa được gán Controller.",
                    this
                );
                return;
            }

            if (sequence == null)
            {
                Debug.LogError(
                    "StoryDialogueTrigger chưa được gán Sequence.",
                    this
                );
                return;
            }

            controller.Play(sequence);
        }
    }
}