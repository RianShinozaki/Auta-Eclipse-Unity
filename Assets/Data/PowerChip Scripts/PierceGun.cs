using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PierceGun : PowerChipMaster
{
    // Start is called before the first frame update
    public override void OnCallPowerChip(PlayerController player)
    {
        Player.stateMachine.SetState(Player.State_Attack);
        Player.animationController.GunAttack();
        Player.MP -= 4;
    }
}
