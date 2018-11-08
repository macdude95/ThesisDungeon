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

    private NarrativeChunk[] openingDialogue = {
        new NarrativeChunk("Hello?? Is someone there??", () =>
        {
            FindObjectOfType<EmptyRoomController>().startLevelTrigger.gameObject.SetActive(false);
        }),
        new NarrativeChunk("Oh hello, traveler! I THOUGHT I heard something in here."),
        new NarrativeChunk("You might be wondering where this voice is coming from."),
        new NarrativeChunk("Do you remember why you are here?"),
        new NarrativeChunk("..." ),
        new NarrativeChunk("You don't talk much, do you?"),
        new NarrativeChunk("..."),
        new NarrativeChunk("I see how this relationship is going to be."),
        new NarrativeChunk("I'm guessing you are one of the many here in search of the treasure..."),
        new NarrativeChunk("... you DO remember of the treasure, don't you?"),
        new NarrativeChunk("..."),
        new NarrativeChunk("I'm going to need to exaplin everything, aren't I?"),
        new NarrativeChunk("Suffice it to say there is a very valuable pearl here somewhere in the dungeon..."),
        new NarrativeChunk("... and I might just know where to find it."),
        new NarrativeChunk("If you can prove yourself worthy, I might just give it to you."),
        new NarrativeChunk("Are you in?"),
        new NarrativeChunk("..."),
        new NarrativeChunk("... I'll take your silence for a resounding 'YES!'"),
        new NarrativeChunk("To prove yourself worthy of the magnificent pearl, here is what you must do:"),
        new NarrativeChunk("Enter this portal and start your descent into the dungeon.", () =>
        {
            FindObjectOfType<EmptyRoomController>().startLevelTrigger.gameObject.SetActive(true);
        }),
        new NarrativeChunk("Your journey will not be easy, but I promise you the reward is worth it."),
        new NarrativeChunk("Good luck. <3")

    };

void HandleAction()
    {
    }


    private Stack<NarrativeChunk[]> narrativeQueue;

    private void Awake()
    {
        narrativeQueue = new Stack<NarrativeChunk[]>();
        narrativeQueue.Push(openingDialogue);
    }

    public NarrativeChunk[] GetNextDialogueSequence() {
        if (narrativeQueue.Count == 0) {
            return new NarrativeChunk[0];
        }
        return narrativeQueue.Pop();
    }

}
