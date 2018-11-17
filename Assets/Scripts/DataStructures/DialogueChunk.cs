using System.Collections;
using System.Collections.Generic;
using System;

public class DialogueChunk {
    public string dialogueSegment;
    private Action action;
    private Func<bool> precondition;

    public DialogueChunk(string dialogueSegment, Action action = null, Func<bool> precondition = null) {
        this.dialogueSegment = dialogueSegment;
        this.action = action;
        this.precondition = precondition;
    }

    public void PerformAction() {
        if (action != null) {
            action();
        }
    }

    public bool AdvanceWithKey() {
        return precondition == null;
    }

    public bool CheckCondition() {
        if (precondition == null) {
            return true;
        }
        return precondition();
    }
}
