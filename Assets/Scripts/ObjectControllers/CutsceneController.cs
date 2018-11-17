using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CutsceneController : MonoBehaviour {

    public Text textBox;
    public Image dialogueBox;
    private int stepInCutscene = 0;
    private DialogueChunk[] cutscene;
    private DialogueChunk nextDialogueChunk {
        get {
            if (stepInCutscene >= cutscene.Length) {
                return null;
            }
            return cutscene[stepInCutscene];
        }
    }
    private void Start() {
        NarrativeController narratvieController = FindObjectOfType<NarrativeController>();
        if (narratvieController == null) { return; }
        cutscene = narratvieController.GetNextCutscene();
        if (cutscene.Length > 0) {
            dialogueBox.gameObject.SetActive(true);
            AdvanceCutscene();
        } else { dialogueBox.gameObject.SetActive(false); }
    }

    private void Update() {
        if (nextDialogueChunk != null && !nextDialogueChunk.AdvanceWithKey()) {
            if (nextDialogueChunk.CheckCondition()) {
                AdvanceCutscene();
            }
        } else if (Input.GetKeyDown(KeyCode.Space)) {
            AdvanceCutscene();
        }
    }

    private void AdvanceCutscene() {
        if (cutscene == null || stepInCutscene >= cutscene.Length) {
            dialogueBox.gameObject.SetActive(false);
            return;
        }
        textBox.text = nextDialogueChunk.dialogueSegment;
        if (textBox.text == "") {
            dialogueBox.gameObject.SetActive(false);
        } else {
            dialogueBox.gameObject.SetActive(true);
        }
        nextDialogueChunk.PerformAction();
        stepInCutscene++;
    }
}
