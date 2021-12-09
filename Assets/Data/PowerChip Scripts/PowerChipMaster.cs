using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerChipMaster : MonoBehaviour
{
    public PlayerController Player;
    public PlayerAnimationController PlayerAnim;
    public virtual void OnCallPowerChip(PlayerController player) { }
    public virtual void OnCallPowerChipAnim(PlayerAnimationController playerAnim) { }
    public void Start()
    {
        Player.PowerChipUse += OnCallPowerChip;
        PlayerAnim.PowerChipUseAnim += OnCallPowerChipAnim;
    }
    public void OnDestroy()
    {
        Player.PowerChipUse -= OnCallPowerChip;
        PlayerAnim.PowerChipUseAnim -= OnCallPowerChipAnim;
    }

}
