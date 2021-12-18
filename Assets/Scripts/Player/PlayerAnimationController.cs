using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    // Start is called before the first frame update
    public PlayerController parent;
    public  Animator anim;

    GameObject bullet;

    public delegate void OnPowerChipUseAnim(PlayerAnimationController player);
    public event OnPowerChipUseAnim PowerChipUseAnim;

    void Start()
    {
        parent = transform.parent.GetComponent<PlayerController>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("XSpeed", parent.Velocity.x);
        anim.SetFloat("YSpeed", parent.Velocity.y);

        anim.SetBool("Grounded", parent.Grounded);
        anim.SetFloat("GhostJumpTimer", parent.GhostJumpTimer);

        anim.SetFloat("HurtState", parent.HurtState);

        if(parent.Grounded && parent.stateMachine.CurrentState == parent.State_Normal && parent.Velocity.x != 0)
        {
            float prevScale = transform.localScale.x;
            transform.localScale = new Vector3( parent.Velocity.x > 0 ? 1 : -1, 1, 1);
            if(prevScale != transform.localScale.x)
            {
                if(anim.GetCurrentAnimatorStateInfo(0).IsName("Move") || anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Stop"))
                    Turn();
            }
        }

        anim.SetBool("Orb", parent.Orb);
    }

    public void SetJump()
    {
        anim.SetTrigger("Jump");
    }
    public void SetRunJump()
    {
        anim.SetTrigger("RunJump");
    }

    public void SwordAttack(int comboNum, int attackType)
    {
        anim.SetTrigger("Sword Attack");
        anim.SetInteger("Combo Number", comboNum);
        anim.SetInteger("Attack Type", attackType);
        anim.SetBool("Grounded", parent.Grounded);

    }

    public void GunAttack()
    {
        anim.SetTrigger("Gun Attack");
    }
    public void MultiGunAttack()
    {
        anim.SetTrigger("Multi Gun Attack");
    }

    public void GunFire()
    {
        PowerChipUseAnim?.Invoke(this);
    }

    public void GunSummon()
    {
        anim.SetTrigger("Gun Summon");
    }

    public void GrabState(bool active)
    {
        anim.SetBool("Grabbing", active);
    }
    public void SetHitBoxes(int index)
    {
        parent.SetHitBoxes(index);
    }

    public void AttackEnd()
    {
        parent.AttackEnd();
    }

    public void Land()
    {
        anim.SetTrigger("Land");
        anim.SetBool("Grounded", true);
    }

    public void Turn()
    {
        anim.SetTrigger("Turn");
    }

    public void Stop()
    {
        anim.SetTrigger("Stop");
    }

    public void Step()
    {
        parent.Step();
    }
    public void Swoosh()
    {
        parent.Swoosh();
    }
    public void Shot()
    {
        parent.Shot();
    }

    public void SetSpeed(float Spd)
    {
        anim.SetFloat("Anim Speed", Spd);
    }

    void CancelLag()
    {
        parent.AttackLag = false;
    }
}
