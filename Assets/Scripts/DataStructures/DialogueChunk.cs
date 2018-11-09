using System.Collections;
using System.Collections.Generic;
using System;

public class DialogueChunk {
    public string dialogueSegment;
    private Action action;

    public DialogueChunk(string dialogueSegment, Action action = null) {
        this.dialogueSegment = dialogueSegment;
        this.action = action;
    }

    public void PerformAction() {
        if (action != null) {
            action();
        }
    }
}
