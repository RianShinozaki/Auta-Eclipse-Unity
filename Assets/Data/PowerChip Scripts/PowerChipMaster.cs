using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerChipMaster : MonoBehaviour
{
    public PlayerController Player;
    public virtual void OnCallPowerChip(PlayerController player) { }
    public void Start()
    {
        Player.PowerChipUse += OnCallPowerChip;
    }
    public void OnDestroy()
    {
        Player.PowerChipUse -= OnCallPowerChip;
    }

}
