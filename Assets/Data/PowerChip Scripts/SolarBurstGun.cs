using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarBurstGun : PowerChipMaster
{
    // Start is called before the first frame update
    public override void OnCallPowerChip(PlayerController player)
    {
        Player.stateMachine.SetState(Player.State_Attack);
        Player.animationController.GunAttack();
    }

    public override void OnCallPowerChipAnim(PlayerAnimationController playerAnim)
    {
        playerAnim.parent.Velocity.x = -6f * playerAnim.transform.localScale.x;
        GameObject bullet = Instantiate(Resources.Load<GameObject>("Player/pre_SolarBurst"), PlayerAnim.transform.position + new Vector3(playerAnim.transform.localScale.x * 2, 0, 0), Quaternion.identity);
        bullet.gameObject.GetComponentInChildren<HitBox>().Entity = playerAnim.parent;
        playerAnim.Shot();

    }
}
