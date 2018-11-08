using System.Collections;
using System.Collections.Generic;
using System;

public class NarrativeChunk
{
    public string dialogueSegment;
    private Action action;

    public NarrativeChunk(string dialogueSegment, Action action = null) {
        this.dialogueSegment = dialogueSegment;
        this.action = action;
    }

    public void PerformAction() {
        if (action != null) {
            action();
        }
    }
}
