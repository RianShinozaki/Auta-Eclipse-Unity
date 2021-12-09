using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergySword : PowerChipMaster
{
    // Start is called before the first frame update
    public override void OnCallPowerChip(PlayerController player)
    {
        Player.stateMachine.SetState(Player.State_Attack);
        Player.animationController.GunSummon();
    }

    public override void OnCallPowerChipAnim(PlayerAnimationController playerAnim)
    {
        playerAnim.parent.Velocity.x = 0;
        playerAnim.parent.Velocity.y = -3;
        GameObject bullet = Instantiate(Resources.Load<GameObject>("Player/pre_EnergySwordAttack"), PlayerAnim.transform.position + new Vector3(0, 1 , 0), Quaternion.identity);
        bullet.transform.localScale = playerAnim.transform.localScale;
        bullet.gameObject.GetComponentInChildren<HitBox>().Entity = playerAnim.parent;
        playerAnim.Shot();

    }
}
