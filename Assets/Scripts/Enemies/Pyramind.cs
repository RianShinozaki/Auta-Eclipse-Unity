using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
using AK.Wwise;
#endif

public class Pyramind : BaseEnemy
{
    [FoldoutGroup("States")] public float PatrolRegion;
    [FoldoutGroup("States")] float WaitTimeRand;
    [FoldoutGroup("States")] public Vector2 WaitTimeRandRange;
    float initX;
    [FoldoutGroup("States")] public float AttackDist;
    [FoldoutGroup("States")] public int AttackQueued;
    public const int NO_ATTACK = 0;
    public const int BASIC_ATTACK = 1;
    public const int SHOTGUN_STRIKE = 2;
    public const int BULLET_SLICE = 3;
    public const int SUMMON = 4;

    [FoldoutGroup("Sounds")] public AK.Wwise.Event Sound_SlashAttack;
    [FoldoutGroup("Sounds")] public AK.Wwise.Event Sound_Step;
    [FoldoutGroup("Sounds")] public AK.Wwise.Event Sound_Skid;
    [FoldoutGroup("Sounds")] public AK.Wwise.Event Sound_Beam;
    [FoldoutGroup("Sounds")] public AK.Wwise.Event Sound_Dash;


    [FoldoutGroup("Manual Setup")] public HitBox[] hb;

    public GameObject EnemySpawner;
    public GameObject EnemyToSpawn;

    public void Start()
    {
        stateMachine.SetState(State_Decide);
        stateMachine.SetRunState(true);
        WaitTimeRand = Random.Range(WaitTimeRandRange.x, WaitTimeRandRange.y);
        initX = transform.position.x;
        Anim = GetComponent<Animator>();

        EnemySpawner = Resources.Load<GameObject>("pre_EnemySpawner");
    }

    // Update is called once per frame
    public override void Update()
    {
        Anim.ResetTrigger("Guard");

        base.Update();

        Anim.SetFloat("XSpeed", Velocity.x);
        Anim.SetBool("Grounded", Grounded);
        Anim.SetFloat("HurtState", HurtState);

        stateMachine.SetRunState(HitStun == 0);

        if (HitStun > 0)
            return;

        if (HurtState > 0)
        {
            HurtState = Mathf.MoveTowards(HurtState, 0, Time.deltaTime * TimeScale * 60);
            //stateMachine.SetRunState(false);
            if (HurtState == 0)
            {
                WaitTimeRand = Random.Range(WaitTimeRandRange.x, WaitTimeRandRange.y);
                stateMachine.SetState(State_Decide);
                stateMachine.SetRunState(true);
            }
        }

        if(!Grounded || stateMachine.CurrentState == State_Shotgun2)
        {
            KnockbackHurtstateThreshold = 0;
            BaseSturdiness = 0;
        } else
        {
            KnockbackHurtstateThreshold = 2;
            BaseSturdiness = 2;
        }

    }

    void State_Decide(PhysicsEntity ent)
    {
        if (stateMachine.TimeInState > WaitTimeRand)
        {
            
            if (Interest > 0)
            {
                transform.localScale = new Vector3(Player.transform.position.x > transform.position.x ? 1 : -1, 1, 1);

                int rand = Random.Range(0, 5);
                if(rand == 0 || rand == 1)
                {
                    AttackQueued = BASIC_ATTACK;
                    stateMachine.SetState(State_Move);
                    if(Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(Player.transform.position.x, Player.transform.position.y)) < AttackDist*0.8f)
                    {

                        StartBackJump();
                        
                    }
                    WaitTimeRand = Random.Range(WaitTimeRandRange.x, WaitTimeRandRange.y)*2;
                }
                else if (rand == 2)
                {
                    AttackQueued = BULLET_SLICE;

                    if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(Player.transform.position.x, Player.transform.position.y)) < AttackDist * 1.5f)
                    {
                        StartBackJump();
                    }
                    else
                    {
                        stateMachine.SetState(State_Attack);
                        Anim.SetTrigger("BulletSlice");
                        Summon_AttackTitle("BULLET SLICE");
                    }
                }
                else if(rand == 3)
                {
                    AttackQueued = SHOTGUN_STRIKE;

                    if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(Player.transform.position.x, Player.transform.position.y)) < AttackDist * 1.5f)
                    {
                        StartBackJump();
                    } else
                    {
                        stateMachine.SetState(State_Shotgun1);
                        Anim.SetTrigger("Shotgun");
                    }
                }
                else if (rand == 4)
                {
                    AttackQueued = SUMMON;

                    if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(Player.transform.position.x, Player.transform.position.y)) < AttackDist * 1.5f)
                    {
                        StartBackJump();
                    }
                    else
                    {
                        stateMachine.SetState(State_Attack);
                        Anim.SetTrigger("BulletSlice");
                        Summon_AttackTitle("SUMMON");
                    }
                }
            }
            else
            {
                int rand = Random.Range(0, 2);
                rand *= 2;
                rand -= 1;

                if (MoveSpeed * WaitTimeRand > initX + PatrolRegion / 2 || MoveSpeed * WaitTimeRand < initX - PatrolRegion / 2)
                {
                    //rand *= -1;
                }

                transform.localScale = new Vector3(rand, 1, 1);
                stateMachine.SetState(State_Move);
                WaitTimeRand = Random.Range(WaitTimeRandRange.x, WaitTimeRandRange.y);
            }
            
        }
        if (Grounded)
            Velocity.x = Mathf.MoveTowards(Velocity.x, 0, Stop * Time.deltaTime * TimeScale);
    }

    void StartBackJump()
    {
        Anim.SetTrigger("BackJump");
        stateMachine.SetState(State_Null);
        transform.localScale = new Vector3(Player.transform.position.x > transform.position.x ? 1 : -1, 1, 1);
        Velocity.x = 0;
    }

    public void BackJump()
    {
        Sound_SlashAttack.Post(gameObject);
        Velocity.y = 5;
        Velocity.x = -MoveSpeed * 1.7f * transform.localScale.x;
        //stateMachine.SetState(State_Jump);
        Grounded = false;
        GroundPoints.Clear();
    }

    public void BackJumpLand()
    {
        stateMachine.SetState(State_Shotgun3);
    }

    public void BackJump_Recovered()
    {
        switch (AttackQueued)
        {
            case BASIC_ATTACK:
                if (Grounded)
                {
                    stateMachine.SetState(State_Move);
                    Velocity.x = 0;
                }
                break;
            case BULLET_SLICE:
                if (Grounded)
                {
                    stateMachine.SetState(State_Attack);
                    Summon_AttackTitle("BULLET SLICE");
                    Anim.SetTrigger("BulletSlice");
                    Velocity.x = 0;
                }
                break;
            case SUMMON:
                if (Grounded)
                {
                    stateMachine.SetState(State_Attack);
                    Summon_AttackTitle("SUMMON");
                    Anim.SetTrigger("BulletSlice");
                    Velocity.x = 0;
                }
                break;
            case SHOTGUN_STRIKE:
                if (Grounded)
                {
                    stateMachine.SetState(State_Shotgun1);
                    Anim.SetTrigger("Shotgun");
                    Velocity.x = 0;
                }
                break;
        }

    }

    void State_Jump(PhysicsEntity ent)
    {
        switch (AttackQueued) {
            case BASIC_ATTACK:
                if (Grounded)
                {
                    stateMachine.SetState(State_Move);
                    Velocity.x = 0;
                }
                break;
            case BULLET_SLICE:
                if (Grounded)
                {
                    stateMachine.SetState(State_Attack);
                    Summon_AttackTitle("BULLET SLICE");
                    Anim.SetTrigger("BulletSlice");
                    Velocity.x = 0;
                }
                break;
            case SHOTGUN_STRIKE:
                if (Grounded)
                {
                    stateMachine.SetState(State_Shotgun1);
                    Anim.SetTrigger("Shotgun");
                    Velocity.x = 0;
                }
                break;
        }
    }

    void State_Move(PhysicsEntity ent)
    {
        if (stateMachine.TimeInState > WaitTimeRand)
        {
            stateMachine.SetState(State_Decide);
        }

        RaycastHit2D hit;
        hit = Physics2D.Raycast(new Vector2(transform.position.x + 0.5f * Mathf.Sign(Velocity.x), coll.bounds.center.y - coll.bounds.extents.y), Vector2.down, 5, EnvironmentMask);

        if (Grounded)
        {
            if (hit.collider == null) //Check for a collision
            {
                //transform.localScale = new Vector3(-transform.localScale.x, 1, 1);
                //Velocity.x = -Velocity.x;
            }
        }

        /*if(Interest > 0 && Grounded && (CanSeePlayer && Player.transform.position.y > (transform.position.y + 1) || Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(Player.transform.position.x, Player.transform.position.y)) > EyeDist))
        {
            Velocity.y = 10;
            Velocity.x = MoveSpeed * 1.5f * transform.localScale.x;
            Grounded = false;
            GroundPoints.Clear();
        }*/

        hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.right * Mathf.Sign(Velocity.x), 1);

        if (!hit.transform.root.gameObject == gameObject)
        {
            if (hit.transform.root.GetComponent<Lumpy>())
            {
                transform.localScale = new Vector3(-transform.localScale.x, 1, 1);
                Velocity.x = -Velocity.x;
            }
        }
        if (Grounded)
            Velocity.x = Mathf.MoveTowards(Velocity.x, MoveSpeed * transform.localScale.x, Accel * Time.deltaTime * TimeScale);

        if (Mathf.Abs(transform.position.x - Player.transform.position.x) <= AttackDist && CanSeePlayer)
        {
            if (AttackQueued == BASIC_ATTACK)
            {
                stateMachine.SetState(State_Null);
                if(Grounded)
                    Velocity.x = 0;
                Anim.SetTrigger("Attack");
                transform.localScale = new Vector3(Player.transform.position.x > transform.position.x ? 1 : -1, 1, 1);
            }
        }

        if (stateMachine.TimeInState > WaitTimeRand)
        {
            
            if (AttackQueued == BASIC_ATTACK)
            {
                stateMachine.SetState(State_Null);
                if (Grounded)
                    Velocity.x = 0;
                Anim.SetTrigger("Attack");
                transform.localScale = new Vector3(Player.transform.position.x > transform.position.x ? 1 : -1, 1, 1);
            }
            else
            {
                WaitTimeRand = Random.Range(WaitTimeRandRange.x, WaitTimeRandRange.y);
                stateMachine.SetState(State_Decide);
            }
        }
    }

    void State_Hurt(PhysicsEntity ent)
    {
        if (Grounded)
            Velocity.x = Mathf.MoveTowards(Velocity.x, 0, Stop * Time.deltaTime * TimeScale);
        if(!Staggered)
        {
            stateMachine.SetState(State_Decide);
        }
    }
    void State_Null(PhysicsEntity ent)
    {

    }

    public void Strike(int ind)
    {
        if (Grounded)
        {
            //transform.localScale = new Vector3(Player.transform.position.x > transform.position.x ? 1 : -1, 1, 1);
            Velocity.x = MoveSpeed * transform.localScale.x;
        }
            
        stateMachine.SetState(State_Attack);
        Sound_SlashAttack.Post(gameObject);
        hb[ind].ActiveTime = 5;
    }

    public void BulletStrike()
    {
        Sound_Beam.Post(gameObject);
        hb[2].ActiveTime = 5;

        if (AttackQueued == BULLET_SLICE)
        {
            for (int i = 0; i < 3; i++)
            {
                GameObject bullet = ObjectPool.Instance.SpawnObject("EnemyBullet", transform.position + new Vector3(0, -0.25f, 0), Quaternion.identity);
                bullet.transform.localScale = transform.localScale;

                bullet.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 50 * i - 50));
            }
        }
        if(AttackQueued == SUMMON)
        {
            GameObject Spawner = Instantiate(EnemySpawner, transform.position + new Vector3(Random.Range(-1, 1), 0, 0), Quaternion.identity);
            Spawner.GetComponent<EnemySpawn>().Enemy = EnemyToSpawn;
        }
    }

    public override void OnLand()
    {
        base.OnLand();
        Anim.ResetTrigger("Guard");
        Velocity.x = 0;
    }
    void State_Attack(PhysicsEntity ent)
    {
        Velocity.x = Mathf.MoveTowards(Velocity.x, 0, Stop * Time.deltaTime * TimeScale * 0.25f);

    }

    public void Attack_Finished()
    {
        Velocity.x = 0;
        WaitTimeRand = Random.Range(WaitTimeRandRange.x, WaitTimeRandRange.y);
        stateMachine.SetState(State_Decide);

        Disable_AttackTitle();
    }

    void State_Shotgun1(PhysicsEntity ent)
    {
        Velocity.x = 0;
        transform.localScale = new Vector3(Player.transform.position.x > transform.position.x ? 1 : -1, 1, 1);
        Summon_AttackTitle("SHOTGUN STRIKE");
    }

    public void Shotgun_Charged()
    {
        Sound_Dash.Post(gameObject);
        stateMachine.SetState(State_Shotgun2);
        Disable_AttackTitle();
        StartCreatingAfterImgs();
    }

    void State_Shotgun2(PhysicsEntity ent)
    {
        Velocity.x = MoveSpeed * 3 * transform.localScale.x;
        hb[3].ActiveTime = 5;
    }

    public void Shotgun_Finished()
    {
        CreateAfterImg = false;
        stateMachine.SetState(State_Shotgun3);
        Sound_Skid.Post(gameObject);
    }


    void State_Shotgun3(PhysicsEntity ent)
    {
        Velocity.x = Mathf.MoveTowards(Velocity.x, 0, Stop * Time.deltaTime * TimeScale * 2);
    }

    public void Shotgun_Recovered()
    {
        WaitTimeRand = Random.Range(WaitTimeRandRange.x, WaitTimeRandRange.y);
        stateMachine.SetState(State_Decide);

    }

    public override void HurtResponse(float damage = 0, float knockbackx = 0, float knockbacky = 0)
    {
        base.HurtResponse(damage, knockbackx, knockbacky);

        CreateAfterImg = false;

        if (!Staggered)
        {
            if (stateMachine.CurrentState == State_Move)
            {
                stateMachine.SetState(State_Decide);
            }
        } else
        {
            stateMachine.SetState(State_Hurt);
        }
    }
    public void Step()
    {
        //Sound_Step.Post(gameObject);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
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
