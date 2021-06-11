using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Rewired;
public class PlayerController : PhysicsEntity
{

    PlayerAnimationController animationController;

    #region Movement Values

    [FoldoutGroup("Movement Values")] public float MoveSpeed;
    [FoldoutGroup("Movement Values")] public float Accel;
    [FoldoutGroup("Movement Values")] public float Stop;

    [FoldoutGroup("Movement Values")] public float JumpPower;
    [FoldoutGroup("Movement Values")] public float AirFrictionDivide;
    [FoldoutGroup("Movement Values")] public float ShortHopGravity;
    [FoldoutGroup("Movement Values")] public bool AllowShortJump;
    #endregion

    [FoldoutGroup("Abilities")] public bool CanOrb;
    [FoldoutGroup("Abilities")] public bool Orb;
    [FoldoutGroup("Abilities")] public float OrbPower = 10;
    [FoldoutGroup("Abilities")] public float GhostJumpTimer = 0;
    [FoldoutGroup("Abilities")] public float AfterIMGTimer = 0;
    [FoldoutGroup("Abilities")] public bool CreateAfterImg = false;

    [FoldoutGroup("FX")] public GameObject AfterIMG;
    [FoldoutGroup("FX")] public GameObject MaterializeFX;
    [FoldoutGroup("FX")] public GameObject DisintegrateFX;

    public const int MOVEMENT_HORIZONTAL = 1;
    public const int MOVEMENT_VERTICAL = 2;
    public const int JUMP = 3;
    public const int ATTACK = 4;
    public const int ORB = 5;
    public const int SPECIAL = 6;
    public int PlayerID;
    public Player player;

    StateMachine stateMachine;
    BoxCollider2D boxColl;

    float ComboNumber;
    float AttackBuffer;

    Vector2[] CollisionSizes = new[] {
        new Vector2(0.63f,1.42f), //Normal Size
        new Vector2(0.45f,0.45f) //Orb Size
        };


    // Start is called before the first frame update
    public override void Awake()
    {
        player = ReInput.players.GetPlayer(PlayerID);

        base.Awake();

        animationController = GetComponentInChildren<PlayerAnimationController>();
        stateMachine = GetComponent<StateMachine>();
        boxColl = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        stateMachine.SetState(State_Normal);
        stateMachine.SetRunState(true);
        ComboNumber = 1;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        AttackBuffer = Mathf.MoveTowards(AttackBuffer, 0, Time.deltaTime);
        if(player.GetButtonDown(ATTACK))
        {
            AttackBuffer = 0.1f;
        }
    }

    //STATES

    void State_Normal(PhysicsEntity ent)
    {
        boxColl.size = CollisionSizes[0];

        if(Mathf.Abs(player.GetAxisRaw(MOVEMENT_HORIZONTAL)) > 0.1f) {
            Velocity.x = Mathf.MoveTowards(Velocity.x, player.GetAxisRaw(MOVEMENT_HORIZONTAL) * MoveSpeed, Accel * (Grounded ? 1 : AirFrictionDivide));
        } else
        {
            Velocity.x = Mathf.MoveTowards(Velocity.x, 0, Stop * (Grounded ? 1 : AirFrictionDivide));
        }

        if (Grounded) {

            GhostJumpTimer = 0.11f;

            CanOrb = true;
        } else
        {
            GhostJumpTimer -= Time.deltaTime;
        }

        if (player.GetButtonDown(JUMP) && GhostJumpTimer > 0)
        {
            //transform.position += Vector3.up * 0.5f;
            Grounded = false;
            Velocity.y = JumpPower;
            animationController.SetJump();
            AllowShortJump = true;
            GroundPoints.Clear();

            GhostJumpTimer = 0;
        }

        if (player.GetButtonDown(ORB) && CanOrb)
        {
            if (player.GetAxisRaw(MOVEMENT_HORIZONTAL) > 0.1f || Mathf.Abs(player.GetAxisRaw(MOVEMENT_VERTICAL)) > 0.1f) {
                Velocity = new Vector2(player.GetAxisRaw(MOVEMENT_HORIZONTAL), player.GetAxisRaw(MOVEMENT_VERTICAL)).normalized * OrbPower;
                if(Grounded)
                {
                    Velocity.y = Mathf.Clamp(Velocity.y, 2f, 100f);
                }
            }
            else
            {
                Velocity = new Vector2(transform.GetChild(0).localScale.x, 0) * OrbPower;
            }
            stateMachine.SetState(State_Orb);

            Grounded = false;
            AllowShortJump = false;
            GroundPoints.Clear();

            CanOrb = false;
            StartCoroutine(CreateAfterImgEnum());

            GameObject img = Instantiate(DisintegrateFX, transform.position, Quaternion.identity);
            img.GetComponent<SpriteRenderer>().sprite = transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
            img.transform.localScale = transform.GetChild(0).localScale;

        }

        if (Velocity.y > 0)
        {
            Velocity.y -= ShortHopGravity * Time.deltaTime * ((AllowShortJump && !player.GetButton(JUMP)) ? 1 : 0);
        }

        Attack();
        ComboNumber = Mathf.MoveTowards(ComboNumber, 1, Time.deltaTime * 10);
        ComboNumber = Mathf.Clamp(ComboNumber, 1, 3);

    }

    void State_Orb(PhysicsEntity ent)
    {
        boxColl.size = CollisionSizes[1];
        Orb = true;
        Bounce = true;

        if (Mathf.Abs(player.GetAxisRaw(MOVEMENT_HORIZONTAL)) > 0.1f ) {
        
            if ((Mathf.Abs(Velocity.x) < MoveSpeed) || Mathf.Sign(player.GetAxisRaw(MOVEMENT_HORIZONTAL)) != Mathf.Sign(Velocity.x))
                Velocity.x = Mathf.MoveTowards(Velocity.x, player.GetAxisRaw(MOVEMENT_HORIZONTAL) * MoveSpeed, Accel * 0.15f);
        }
        else
        {
            Velocity.x = Mathf.MoveTowards(Velocity.x, 0, Stop * 0.15f);
        }

        if(stateMachine.TimeInState > 0.4f && !player.GetButton(ORB) )
        {
            stateMachine.SetState(State_Normal);
            if( Mathf.Abs(player.GetAxisRaw(MOVEMENT_HORIZONTAL)) > 0.1f || Mathf.Abs(player.GetAxisRaw(MOVEMENT_VERTICAL)) > 0.1f)
            {
                Velocity = new Vector2(player.GetAxisRaw(MOVEMENT_HORIZONTAL), player.GetAxisRaw(MOVEMENT_VERTICAL)) * 3.5f;

            }
            Orb = false;
            Bounce = false;
            Instantiate(MaterializeFX, transform.position + Vector3.back * 0.01f, Quaternion.identity);
            CreateAfterImg = false;
        }
        if(stateMachine.TimeInState > 0.08f)
        {
            Velocity.y -= Gravity * 0.5f * Time.deltaTime;
        } else
        {
            Velocity.y += Gravity * Time.deltaTime*0.8f;
        }

        if(Landed && Mathf.Abs(Velocity.y) < 3)
        {
            stateMachine.SetState(State_Normal);
            Orb = false;
            Bounce = false;
            Instantiate(MaterializeFX, transform.position + Vector3.back * 0.01f, Quaternion.identity);
            CreateAfterImg = false;
        }

        Landed = false;

    }

    void State_Attack(PhysicsEntity ent)
    {

        if (Mathf.Abs(player.GetAxisRaw(MOVEMENT_HORIZONTAL)) > 0.1f && !Grounded)
        {
            Velocity.x = Mathf.MoveTowards(Velocity.x, player.GetAxisRaw(MOVEMENT_HORIZONTAL) * MoveSpeed, Accel * (Grounded ? 1 : AirFrictionDivide));
        }
        else
        {
            Velocity.x = Mathf.MoveTowards(Velocity.x, 0, Stop * AirFrictionDivide);
        }

        AnimatorClipInfo[] CurrentClipInfo;
        if (animationController.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Idle") ) {
            AttackEnd();
        }



    }

    public void Attack()
    {
        if(AttackBuffer > 0)
        {
            stateMachine.SetState(State_Attack);

            if (player.GetAxisRaw(MOVEMENT_VERTICAL) > 0.1f)
            {
                animationController.SwordAttack(0, 2);
                return;
            }
            if (player.GetAxisRaw(MOVEMENT_VERTICAL) < -0.1f)
            {
                animationController.SwordAttack(0, 1);
                return;
            }

            animationController.SwordAttack(Mathf.CeilToInt(ComboNumber), 0);
            ComboNumber = Mathf.CeilToInt(ComboNumber) + 1;
            
        }
    }

    public void AttackEnd()
    {
        stateMachine.SetState(State_Normal);
    }

    IEnumerator CreateAfterImgEnum()
    {
        CreateAfterImg = true;
        while (CreateAfterImg)
        {
            AfterIMGTimer += Time.deltaTime;

            if (AfterIMGTimer > 0.07f)
            {
                GameObject img = Instantiate(AfterIMG, transform.position, Quaternion.identity);
                img.GetComponent<SpriteRenderer>().sprite = transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
                img.transform.localScale = transform.localScale;
                AfterIMGTimer = 0;
            }
            yield return null;
        }

        yield break;
    }
}
