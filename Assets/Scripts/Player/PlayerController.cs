using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PhysicsEntity
{


    #region Movement Values

    public float MoveSpeed;
    public float Accel;
    public float Stop;

    public float JumpPower;
    public float AirFrictionDivide;

    #endregion



    // Start is called before the first frame update
    public override void Start()
    {
        
    }

    // Update is called once per frame
    public override void Update()
    {
        NormalState();
    }

    public void NormalState()
    {
        if(Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f) {
            Velocity.x = Mathf.MoveTowards(Velocity.x, Input.GetAxisRaw("Horizontal") * MoveSpeed, Accel * (AirFrictionDivide * (Grounded ? 0 : 1)));
        } else
        {
            Velocity.x = Mathf.MoveTowards(Velocity.x, 0, Stop * (AirFrictionDivide * (Grounded ? 0 : 1) ) );
        }

        if (Input.GetButtonDown("Jump") && Grounded) {
            Velocity.y = JumpPower;
        }
    }
}
