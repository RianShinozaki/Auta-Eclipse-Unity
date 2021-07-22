using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelAfterimage : StateMachineBehaviour
{
    PlayerController player;
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = animator.transform.root.GetComponent<PlayerController>();
        player.CreateAfterImg = false;
    }

}
