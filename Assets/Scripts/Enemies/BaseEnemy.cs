using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using AK.Wwise;

public class BaseEnemy : PhysicsEntity
{
    

    [FoldoutGroup("Setup")] public StateMachine stateMachine;
    [FoldoutGroup("Setup")] public bool CanSeePlayer;
    [FoldoutGroup("Setup")] public float Interest;
    [FoldoutGroup("Setup")] public GameObject Player;
    [FoldoutGroup("Setup")] public float StunTime;
    [FoldoutGroup("Setup")] public bool stunned;
    [FoldoutGroup("Setup")] public PlayerController GrabbedBy;
    [FoldoutGroup("Manual Setup")] public Transform StaggerMeter;
    [FoldoutGroup("Manual Setup")] public Transform HPMeter;
    [FoldoutGroup("Manual Setup")] public GameObject AttackTitle;
    [FoldoutGroup("Manual Setup")] public GameObject CannonballHitBox;

    [FoldoutGroup("Base Enemy Stats")] public float EyeDist;
    [FoldoutGroup("Base Enemy Stats")] public float EyeAngleRange;
    [FoldoutGroup("Base Enemy Stats")] public float MaxInterest;
    [FoldoutGroup("Base Enemy Stats")] public bool SeeThroughWalls;
    [FoldoutGroup("Base Enemy Stats")] public float Accel;
    [FoldoutGroup("Base Enemy Stats")] public float Stop;
    [FoldoutGroup("Base Enemy Stats")] public float MoveSpeed;
    [FoldoutGroup("Base Enemy Stats")] public bool CanStun;
    [FoldoutGroup("Base Enemy Stats")] public Vector2Int MoneyDrop;
    [FoldoutGroup("Base Enemy Stats")] public bool ShowStats;
    [FoldoutGroup("Base Enemy Stats")] public bool LightEnemy;
    [FoldoutGroup("Base Enemy Stats")] public bool Tumble;
    [FoldoutGroup("Base Enemy Stats")] public float TumbleSpeed;

    [FoldoutGroup("Sounds")] public AK.Wwise.Event Sound_WallSlam;

    public ParticleSystem bloodPsys;

    GameObject affinityHUD;

    public override void Awake()
    {
        base.Awake();
        stateMachine = GetComponent<StateMachine>();
        Player = GameObject.Find("Auta");
        affinityHUD = Resources.Load<GameObject>("pre_AffinityHUD");
        Instantiate(affinityHUD, transform);
        
    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if (stateMachine.CurrentState == State_Cannonball)
        {
            CamVariables.Screenshake = 0.2f;
            Sound_WallSlam.Post(gameObject);
        }
    }

    public virtual void DeathProcedure()
    {
        GameObject hfx = Instantiate(HitFX, transform.position, Quaternion.identity);
        hfx.GetComponent<HitFXController>().type = EffectType.EnemyDeath;

        int toDrop = Random.Range(MoneyDrop.x, MoneyDrop.y);
        if(toDrop > 0 && toDrop < 5)
        {
            for(int i = 0; i < toDrop; i++)
                ObjectPool.Instance.SpawnObject("Coin", transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    public override void Update()
    {
        base.Update();

        if(ShowStats)
        {
            StaggerMeter.transform.localScale = new Vector3(Mathf.Clamp(Stagger / MaxStagger, 0, 1), 1, 1);
            HPMeter.transform.localScale = new Vector3(Mathf.Clamp(HP / MaxHP, 0, 1), 1, 1);
        }

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

        if (LightEnemy)
        {
            if (Tumble && stateMachine.CurrentState != State_Cannonball)
            {
                Bounce = true;
                Anim.transform.eulerAngles -= new Vector3(0, 0, TumbleSpeed) * Time.deltaTime;
                HurtState = 15;
            }
            else
            {
                Anim.transform.eulerAngles = new Vector3(0, 0, Mathf.MoveTowardsAngle(Anim.transform.eulerAngles.z, 0, 360 * Time.deltaTime)); 
            }
        }

    }

    public override void OnBounce()
    {
        base.OnBounce();

        if(Tumble)
        {
            Velocity.x = Velocity.x / 2;
            Velocity.y = Velocity.y / 2;
            if(Mathf.Abs(Velocity.x) < 2 && Mathf.Abs(Velocity.y) < 4)
            {
                Tumble = false;
                TumbleSpeed = 0;
                Bounce = false;
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

            if (defender != null)
            {
                defender.GetComponent<PhysicsEntity>().Velocity = Velocity * -1;
            }
        }
    }

    public void Summon_AttackTitle(string name)
    {
        AttackTitle.SetActive(true);
        AttackTitle.GetComponent<AttackTitle>().text = name;
    }
    public void Disable_AttackTitle()
    {
        AttackTitle.SetActive(false);
    }
    public override void HurtResponse(float damage = 0, float knockbackx = 0, float knockbacky = 0)
    {
        base.HurtResponse(damage, knockbackx, knockbacky);

        if(!Staggered)
        {
            Anim.SetTrigger("Guard");
        }

        if(LightEnemy && Grounded == false || Mathf.Abs(knockbackx) > 3)
        {
            Tumble = true;
            TumbleSpeed = Mathf.Clamp(Velocity.x + Velocity.y * Mathf.Sign(Velocity.x), -3, 3) * 60;
        }

        bloodPsys.Emit(Mathf.CeilToInt(damage));
    }
}

