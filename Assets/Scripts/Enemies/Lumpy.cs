using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Lumpy : BaseEnemy {
    [FoldoutGroup("States")] public float PatrolRegion;
    [FoldoutGroup("States")] float WaitTimeRand;
    [FoldoutGroup("States")] public Vector2 WaitTimeRandRange;
    float initX;
    [FoldoutGroup("States")] public float AttackDist;
    [FoldoutGroup("Manual Setup")] public HitBox hb;

    public void Start() {
        stateMachine.SetState(State_Decide);
        stateMachine.SetRunState(true);
        WaitTimeRand = Random.Range(WaitTimeRandRange.x, WaitTimeRandRange.y);
        initX = transform.position.x;
        Anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    public override void Update() {

        base.Update();

        Anim.SetFloat("XSpeed", Velocity.x);
        Anim.SetBool("Grounded", Grounded);
        Anim.SetFloat("HurtState", HurtState);

        stateMachine.SetRunState(HitStun == 0);

        if(HitStun > 0)
            return;

        if(HurtState > 0) {
            HurtState = Mathf.MoveTowards(HurtState, 0, Time.deltaTime * TimeScale * 60);
            stateMachine.SetRunState(false);
            if(HurtState == 0 && Grounded) {
                WaitTimeRand = Random.Range(WaitTimeRandRange.x, WaitTimeRandRange.y);
                stateMachine.SetState(State_Decide);
                stateMachine.SetRunState(true);
            }
        }


    }

    void State_Decide(PhysicsEntity ent) {
        if(stateMachine.TimeInState > WaitTimeRand) {
            WaitTimeRand = Random.Range(WaitTimeRandRange.x, WaitTimeRandRange.y);
            if(Interest > 0) {
                transform.localScale = new Vector3(Player.transform.position.x > transform.position.x ? 1 : -1, 1, 1);

            }
            else {
                int rand = Random.Range(0, 1);
                rand *= 2;
                rand -= 1;

                if(MoveSpeed * WaitTimeRand > initX + PatrolRegion / 2 || MoveSpeed * WaitTimeRand < initX - PatrolRegion / 2) {
                    rand *= -1;
                }

                transform.localScale = new Vector3(rand, 1, 1);
            }
            stateMachine.SetState(State_Move);
        }
        if(Grounded)
            Velocity.x = Mathf.MoveTowards(Velocity.x, 0, Stop * Time.deltaTime * TimeScale);
    }

    void State_Move(PhysicsEntity ent) {
        if(stateMachine.TimeInState > WaitTimeRand) {
            stateMachine.SetState(State_Decide);
        }

        if(Grounded)
            Velocity.x = Mathf.MoveTowards(Velocity.x, MoveSpeed * transform.localScale.x, Accel * Time.deltaTime * TimeScale);

        if(Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(Player.transform.position.x, Player.transform.position.y)) <= AttackDist && CanSeePlayer && Grounded) {
            stateMachine.SetState(State_Null);
            Velocity.x = 0;
            Anim.SetTrigger("Attack");
            transform.localScale = new Vector3(Player.transform.position.x > transform.position.x ? 1 : -1, 1, 1);
        }

        if(stateMachine.TimeInState > WaitTimeRand) {
            WaitTimeRand = Random.Range(WaitTimeRandRange.x, WaitTimeRandRange.y);
            stateMachine.SetState(State_Decide);
        }
    }

    void State_Null(PhysicsEntity ent) {

    }

    public void Strike() {
        Velocity.x = MoveSpeed * 1.8f * transform.localScale.x;
        stateMachine.SetState(State_Attack);

    }

    void State_Attack(PhysicsEntity ent) {
        hb.ActiveTime = 4;
        Velocity.x = Mathf.MoveTowards(Velocity.x, 0, Stop * Time.deltaTime * TimeScale * 0.25f);
        if(Velocity.x == 0) {
            WaitTimeRand = Random.Range(WaitTimeRandRange.x, WaitTimeRandRange.y);
            stateMachine.SetState(State_Decide);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        Handles.color = Color.white;
        Handles.DrawWireDisc(transform.position, Vector3.forward, EyeDist);

        //Gizmos.color = Color.white;
        //Gizmos.DrawWireSphere(transform.position, EyeDist);

        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, Vector3.forward, AttackDist);

        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position, AttackDist);
        Debug.DrawLine(transform.position - new Vector3(PatrolRegion / 2, 0, 0), transform.position + new Vector3(PatrolRegion / 2, 0, 0), Color.green);
    }
#endif
}
