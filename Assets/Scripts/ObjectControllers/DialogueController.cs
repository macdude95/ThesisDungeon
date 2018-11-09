using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class DialogueController : MonoBehaviour {

    public Text textBox;
    public Image dialogueBox;
    private int stepInDialogue = 0;
    private DialogueChunk[] sectionOfNarrative;
    private void Start() {
        sectionOfNarrative = FindObjectOfType<NarrativeController>().GetNextDialogueSequence();
        if (sectionOfNarrative.Length > 0) {
            dialogueBox.gameObject.SetActive(true);
            SetNewTextAndPerformAction();
        } else { dialogueBox.gameObject.SetActive(false); }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (stepInDialogue >= sectionOfNarrative.Length) {
                dialogueBox.gameObject.SetActive(false);
            } else {
                SetNewTextAndPerformAction();
            }
        }
    }

    private void SetNewTextAndPerformAction() {
        textBox.text = sectionOfNarrative[stepInDialogue].dialogueSegment;
        if (textBox.text == "") {
            dialogueBox.gameObject.SetActive(false);
        } else {
            dialogueBox.gameObject.SetActive(true);
        }
        sectionOfNarrative[stepInDialogue].PerformAction();
        stepInDialogue++;
    }
}
