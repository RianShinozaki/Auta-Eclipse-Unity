using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    // Start is called before the first frame update
    PlayerController parent;
    Animator anim;

    void Start()
    {
        parent = transform.root.GetComponent<PlayerController>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("XSpeed", parent.Velocity.x);
        anim.SetFloat("YSpeed", parent.Velocity.y);

        anim.SetBool("Grounded", parent.Grounded);
        anim.SetFloat("GhostJumpTimer", parent.GhostJumpTimer);

        if(parent.Grounded && parent.Velocity.x != 0)
        {
            transform.localScale = new Vector3( parent.Velocity.x > 0 ? 1 : -1, 1, 1);
        }

        anim.SetBool("Orb", parent.Orb);
    }

    public void SetJump()
    {
        anim.SetTrigger("Jump");
    }

    public void SwordAttack(int comboNum, int attackType)
    {
        anim.SetTrigger("Sword Attack");
        anim.SetInteger("Combo Number", comboNum);
        anim.SetInteger("Attack Type", attackType);
        anim.SetBool("Grounded", parent.Grounded);

    }

    public void AttackEnd()
    {
        parent.AttackEnd();
    }

}
