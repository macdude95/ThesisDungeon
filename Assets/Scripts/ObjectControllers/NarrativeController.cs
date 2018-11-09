using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarrativeController : MonoBehaviour {

    /*
     * PROGRESSION OF NARRATIVE
     * 
     * character wakes up in empty room.. presses through dialogue from a voice that exaplins his predicament
     * portal opens...
     * player plays through first level
     * appears again in room with new dialogue. 
     * now the portal to start new levecl only opens when the player picks up the pearl
     * player plays through level holding pearl
     * gets back to empty room after finishing level
     * voice encourages him telling him that he is doing great and how awesome it is that he still has the pearl
     * Voice says he must be getting closer
     * plays another level
     * 
     * this sort of process continues.. player gets first chance to drop pearl into a pit after _____ levels
     * 
     */

    public GameObject PearlPrefab;

    private DialogueChunk[] openingDialogue {
        get {
            return new[] {
                new DialogueChunk("Hello?? Is someone there??", () => {
                    FindObjectOfType<EmptyRoomController>().startLevelTrigger.gameObject.SetActive(false);
                }),
                new DialogueChunk("Oh hello, traveler! I THOUGHT I heard something in here."),
                new DialogueChunk("You might be wondering where this voice is coming from."),
                new DialogueChunk("Do you remember why you are here?"),
                new DialogueChunk("..."),
                new DialogueChunk("You don't talk much, do you?"),
                new DialogueChunk("..."),
                new DialogueChunk("I see how this relationship is going to be."),
                new DialogueChunk("I'm guessing you are one of the many here in search of the treasure..."),
                new DialogueChunk("... you DO remember of the treasure, don't you?"),
                new DialogueChunk("..."),
                new DialogueChunk("I'm going to need to exaplin everything, aren't I?"),
                new DialogueChunk("Suffice it to say there is a very valuable pearl here somewhere in the dungeon..."),
                new DialogueChunk("... and I might just know where to find it."),
                new DialogueChunk("If you can prove yourself worthy, I might just give it to you."),
                new DialogueChunk("Are you in?"),
                new DialogueChunk("..."),
                new DialogueChunk("... I'll take your silence for a resounding 'YES!'"),
                new DialogueChunk("To prove yourself worthy of the magnificent pearl, here is what you must do:"),
                new DialogueChunk("Enter this portal and start your descent into the dungeon.", () => {
                    FindObjectOfType<EmptyRoomController>().startLevelTrigger.gameObject.SetActive(true);
                }),
                new DialogueChunk("Your journey will not be easy, but I promise you the reward is worth it."),
                new DialogueChunk("Good luck. <3")
            };
        }
    }

    private DialogueChunk[] passedTestDialogue {
        get {
            return new[] {
                new DialogueChunk("It seems you faired well against the monsters."),
                new DialogueChunk("You have passed my test!"),
                new DialogueChunk("As promised... the pearl"),
                new DialogueChunk("", () =>
                {
                    Instantiate(PearlPrefab);
                }),
                new DialogueChunk("Alright now go ahead.")
            };
        }
    }
    private Stack<DialogueChunk[]> narrativeQueue;

    private void Awake() {
        narrativeQueue = new Stack<DialogueChunk[]>();
        narrativeQueue.Push(openingDialogue);
        narrativeQueue.Push(passedTestDialogue);
    }

    public DialogueChunk[] GetNextDialogueSequence() {
        if (narrativeQueue.Count == 0) {
            return new DialogueChunk[0];
        }
        return narrativeQueue.Pop();
    }

}
