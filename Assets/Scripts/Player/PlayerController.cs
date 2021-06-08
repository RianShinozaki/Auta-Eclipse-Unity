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



    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();

        animationController = GetComponentInChildren<PlayerAnimationController>();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        NormalState();
    }

    public void NormalState()
    {
        if(Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f) {
            Velocity.x = Mathf.MoveTowards(Velocity.x, Input.GetAxisRaw("Horizontal") * MoveSpeed, Accel * (Grounded ? 1 : AirFrictionDivide));
        } else
        {
            Velocity.x = Mathf.MoveTowards(Velocity.x, 0, Stop * (Grounded ? 1 : AirFrictionDivide));
        }

        if (Input.GetButtonDown("Jump") && Grounded) {
            Velocity.y = JumpPower;
            Debug.Log("Yump");
            transform.position += Vector3.up * 0.1f;
            Grounded = false;
            animationController.SetJump();
            AllowShortJump = true;
        }

        if(Velocity.y > 0)
        {
            Velocity.y -= ShortHopGravity * Time.deltaTime * ((AllowShortJump && !Input.GetButton("Jump")) ? 1 : 0);
        }
    }
}
