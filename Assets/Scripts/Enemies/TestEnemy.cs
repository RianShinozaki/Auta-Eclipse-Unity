using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : BaseEnemy
{
    public override void Update()
    {
        base.Update();

        if(Grounded)
            Velocity.x = Mathf.MoveTowards(Velocity.x, 0, Stop);
    }
}
