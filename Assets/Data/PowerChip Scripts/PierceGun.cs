using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PierceGun : PowerChipMaster
{
    // Start is called before the first frame update
    public override void OnCallPowerChip(PlayerController player)
    {
        Player.stateMachine.SetState(Player.State_Attack);
        Player.animationController.MultiGunAttack();
    }

    public override void OnCallPowerChipAnim(PlayerAnimationController playerAnim)
    {
        playerAnim.parent.Velocity.x = -3 * playerAnim.transform.localScale.x;
        GameObject bullet = ObjectPool.Instance.SpawnObject("AutaBullet", playerAnim.transform.position + new Vector3(playerAnim.transform.localScale.x * 0.4f, 0, 0), Quaternion.identity);
        bullet.transform.localScale = playerAnim.transform.localScale;
        bullet.gameObject.GetComponentInChildren<HitBox>().Entity = playerAnim.parent;
        playerAnim.Shot();

    }
}
