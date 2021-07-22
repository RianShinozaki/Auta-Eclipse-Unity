using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextHandler : MonoBehaviour {
    public GameObject TextUI;
    public Dialogue Conversation;

    int currentDialogue = 0;
    public void SetConvo(Dialogue dialogue) {
        Conversation = dialogue;
        currentDialogue = -1;
        NextLine();
    }

    public void NextLine() {
        currentDialogue++;
    }
}