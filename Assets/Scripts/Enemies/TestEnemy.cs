using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TestEnemy : BaseEnemy
{
    public TextMeshPro text;
    public override void Update()
    {
        base.Update();

        if(Grounded)
            Velocity.x = 0;

    }
}
