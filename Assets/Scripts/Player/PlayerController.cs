using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

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

    [FoldoutGroup("FX")] public GameObject MaterializeFX;
    StateMachine stateMachine;
    BoxCollider2D boxColl;

    Vector2[] CollisionSizes = new[] {
        new Vector2(0.63f,1.42f), //Normal Size
        new Vector2(0.45f,0.45f) //Orb Size
        };


    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();

        animationController = GetComponentInChildren<PlayerAnimationController>();
        stateMachine = GetComponent<StateMachine>();
        boxColl = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        stateMachine.SetState(State_Normal);
        stateMachine.SetRunState(true);
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    void State_Normal(PhysicsEntity ent)
    {
        boxColl.size = CollisionSizes[0];

        if(Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f) {
            Velocity.x = Mathf.MoveTowards(Velocity.x, Input.GetAxisRaw("Horizontal") * MoveSpeed, Accel * (Grounded ? 1 : AirFrictionDivide));
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

        if (Input.GetButtonDown("Jump") && GhostJumpTimer > 0)
        {
            //transform.position += Vector3.up * 0.5f;
            Grounded = false;
            Velocity.y = JumpPower;
            animationController.SetJump();
            AllowShortJump = true;
            GroundPoints.Clear();

            GhostJumpTimer = 0;
        }

        if (Input.GetButtonDown("Fire1") && CanOrb)
        {
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.1f) {
                Velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * OrbPower;
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

        }

        if (Velocity.y > 0)
        {
            Velocity.y -= ShortHopGravity * Time.deltaTime * ((AllowShortJump && !Input.GetButton("Jump")) ? 1 : 0);
        }
    }

    void State_Orb(PhysicsEntity ent)
    {
        boxColl.size = CollisionSizes[1];
        Orb = true;
        Bounce = true;

        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f ) {
        
            if (Mathf.Abs(Velocity.x) < MoveSpeed)
                Velocity.x = Mathf.MoveTowards(Velocity.x, Input.GetAxisRaw("Horizontal") * MoveSpeed, Accel * 0.2f);
        }
        else
        {
            Velocity.x = Mathf.MoveTowards(Velocity.x, 0, Stop * 0.3f);
        }

        if(stateMachine.TimeInState > 0.4f && !Input.GetButton("Fire1") )
        {
            stateMachine.SetState(State_Normal);
            if( Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.1f)
            {
                Velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * 3.5f;

            }
            Orb = false;
            Bounce = false;
            Instantiate(MaterializeFX, transform.position + Vector3.back * 0.01f, Quaternion.identity); ;
        }
        if(stateMachine.TimeInState > 0.08f)
        {
            Velocity.y -= Gravity * 0.5f * Time.deltaTime;
        } else
        {
            Velocity.y += Gravity * Time.deltaTime*0.8f;
        }
        
    }
}
