using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class BaseEnemy : PhysicsEntity
{

    [FoldoutGroup("Setup")] public StateMachine stateMachine;
    [FoldoutGroup("Setup")] public bool CanSeePlayer;
    [FoldoutGroup("Setup")] public float Interest;
    [FoldoutGroup("Setup")] public GameObject Player;
    [FoldoutGroup("Setup")] public float StunTime;
    [FoldoutGroup("Setup")] public bool stunned;
    [FoldoutGroup("Setup")] public PlayerController GrabbedBy;
    [FoldoutGroup("Manual Setup")] public GameObject CannonballHitBox;

    [FoldoutGroup("Base Enemy Stats")] public float EyeDist;
    [FoldoutGroup("Base Enemy Stats")] public float EyeAngleRange;
    [FoldoutGroup("Base Enemy Stats")] public float MaxInterest;
    [FoldoutGroup("Base Enemy Stats")] public bool SeeThroughWalls;
    [FoldoutGroup("Base Enemy Stats")] public float Accel;
    [FoldoutGroup("Base Enemy Stats")] public float Stop;
    [FoldoutGroup("Base Enemy Stats")] public float MoveSpeed;
    [FoldoutGroup("Base Enemy Stats")] public bool CanStun;
    
    public override void Awake()
    {
        base.Awake();
        stateMachine = GetComponent<StateMachine>();
        Player = GameObject.Find("Auta");
        
        
        CamVariables.Screenshake = 0.3f;
        
    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if (stateMachine.CurrentState == State_Cannonball)
        {
            CamVariables.Screenshake = 0.2f;
        }
    }

    public virtual void DeathProcedure()
    {
        GameObject hfx = Instantiate(HitFX, transform.position, Quaternion.identity);
        hfx.GetComponent<HitFXController>().type = DamageType.EnemyDeath;
        Destroy(gameObject);
    }

    public override void Update()
    {
        base.Update();

        if (Player == null)
            return;

        CanSeePlayer = false;
        Interest = Mathf.MoveTowards(Interest, 0, Time.deltaTime * TimeScale);

        if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(Player.transform.position.x, Player.transform.position.y)) < EyeDist * (Interest > 0 ? 1.5f : 1))
        {
            Vector2 playerDir = new Vector2(transform.position.x, transform.position.y) - new Vector2(Player.transform.position.x, Player.transform.position.y);
            Ray ray = new Ray(transform.position, playerDir.normalized);
            if(!Physics.Raycast(ray, EyeDist * (Interest > 0 ? 1.5f : 1), layerMask:EnvironmentMask))
            {
                Interest = MaxInterest;
                CanSeePlayer = true;
            }
        }

        if(HP <= 0)
        {
            if(CanStun && !stunned)
            {
                HP = 0.1f;
                stunned = true;
                
            }
            else
            {
                DeathProcedure();
            }
        }

        if(GrabbedBy != null)
        {
            transform.localPosition = Vector2.zero;
        } else if(stunned && stateMachine.CurrentState != State_Cannonball)
        {
            stateMachine.SetRunState(false);
            Anim.SetBool("Stunned", true);

            TimeScale = 0.5f;

            if (Grounded)
                Velocity.x = Mathf.MoveTowards(Velocity.x, 0, Stop * Time.deltaTime * TimeScale);

            StunTime += Time.deltaTime * TimeScale;

            if (StunTime > 1f)
            {
                DeathProcedure();
            }

        }

    }

    public void State_Cannonball(PhysicsEntity ent)
    {
        CannonballHitBox.GetComponent<HitBox>().ActiveTime = 5;

        Bounce = true;
        BounceMult = 1;

        TimeScale = 1;

        StunTime += Time.deltaTime * TimeScale;

        Anim.transform.Rotate(new Vector3(0, 0, 360) * Time.deltaTime);

        if (StunTime > 4f)
        {
            DeathProcedure();
        }
    }

    public override void HitResponse(GameObject attacker, GameObject defender)
    {
        base.HitResponse(attacker, defender);

        if(attacker == CannonballHitBox && stateMachine.CurrentState == State_Cannonball)
        {
            float directionToOther;

            directionToOther = Vector2.SignedAngle(new Vector2(defender.transform.position.x, defender.transform.position.y), new Vector2(attacker.transform.position.x, attacker.transform.position.y));

            float Speed = Velocity.magnitude;

            Velocity.x = Speed * Mathf.Cos(directionToOther * Mathf.Deg2Rad);
            Velocity.y = Speed * Mathf.Sin(directionToOther * Mathf.Deg2Rad);

            defender.GetComponent<PhysicsEntity>().Velocity = Velocity * -1;
        }
    }
}
