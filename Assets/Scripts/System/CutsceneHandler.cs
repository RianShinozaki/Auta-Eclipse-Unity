using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneHandler : MonoBehaviour {

    public float TimeInEvent;
    public float TimeInScene;

    CutsceneEvent[] Events;
    bool InScene;
    int CurrentEvent;

    private void Update() {
        if(InScene) {
            TimeInScene += Time.deltaTime;
            TimeInEvent += Time.deltaTime;

            Events[CurrentEvent].Update();
            if(Events[CurrentEvent].Next) {
                TimeInScene = 0f;
                TimeInEvent = 0f;
                CurrentEvent++;
            }
        }
    }

    public void StartCutscene(CutsceneEvent[] events) {
        if(events.Length == 0) {
            return;
        }

        Events = events;
        InScene = true;
    }
}

[System.Serializable]
public class CutsceneEvent {
    public virtual bool Next { get; private set; }
    public virtual void Update() { }
}