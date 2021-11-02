using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    // Start is called before the first frame update
    PlayerController parent;
    Animator anim;

    GameObject bullet;

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

    public void GunFire()
    {
        parent.Velocity.x = -3 * transform.localScale.x;
        GameObject bullet = ObjectPool.Instance.SpawnObject("AutaBullet", transform.position + new Vector3(transform.localScale.x * 0.4f, 0, 0), Quaternion.identity);
        bullet.transform.localScale = transform.localScale;
        bullet.gameObject.GetComponentInChildren<HitBox>().Entity = parent;
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

    public void SetSpeed(float Spd)
    {
        anim.SetFloat("Anim Speed", Spd);
    }

    void CancelLag()
    {
        parent.AttackLag = false;
    }
}
