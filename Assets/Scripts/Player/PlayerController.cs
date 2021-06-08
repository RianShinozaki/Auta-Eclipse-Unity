using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PhysicsEntity
{

    PlayerAnimationController animationController;

    #region Movement Values

    public float MoveSpeed;
    public float Accel;
    public float Stop;

    public float JumpPower;
    public float AirFrictionDivide;
    public float ShortHopGravity;
    public bool AllowShortJump;
    #endregion

    public bool CanOrb;
    public bool Orb;
    public float OrbPower = 10;
    
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

            if (Input.GetButtonDown("Jump")) {
                //transform.position += Vector3.up * 0.5f;
                Grounded = false;
                Velocity.y = JumpPower;
                animationController.SetJump();
                AllowShortJump = true;
                GroundPoints.Clear();
            }

            CanOrb = true;
        }

        if (Input.GetButtonDown("Fire1") && CanOrb)
        {
            Grounded = false;
            AllowShortJump = false;
            GroundPoints.Clear();

            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.1f) {
                Velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * OrbPower;
            }
            else
            {
                Velocity = new Vector2(transform.GetChild(0).localScale.x, 0) * OrbPower;
            }
            stateMachine.SetState(State_Orb);

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
                Velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * 4;

            }
            Orb = false;
            Bounce = false;
        }
        if(stateMachine.TimeInState > 0.1f)
        {
            Velocity.y -= Gravity * 0.5f * Time.deltaTime;
        } else
        {
            Velocity.y += Gravity * Time.deltaTime;
        }
        
    }
}
