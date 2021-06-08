using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour {
    public Action<PhysicsEntity> CurrentState { get; private set; }
    public Action<PhysicsEntity> PrevState { get; private set; }
    public bool RunState { get; private set; }

    public float TimeInState;

    PhysicsEntity entity;

    private void Awake() {
        entity = GetComponent<PhysicsEntity>();
    }

    private void Update() {
        if(!RunState || CurrentState == null) {
            return;
        }

        CurrentState.Invoke(entity);

        TimeInState += Time.deltaTime;
    }

    public void SetState(Action<PhysicsEntity> action, bool resetMode = false) {
        if(action == null) {
            return;
        }

        PrevState = CurrentState;
        CurrentState = action;
        TimeInState = 0f;

        if(resetMode) {
            entity.Mode = 0;
            entity.SubMode = 0;
        }
    }

    public void SetRunState(bool enable) {
        RunState = enable;
    }
}