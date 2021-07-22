using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox : MonoBehaviour
{
    // Start is called before the first frame update
    public PhysicsEntity entity;
    public GameObject HitFX;
    public float InvincibleTime;
    public List<Collider2D> ignores = new List<Collider2D>();
    public List<Collider2D> DefaultIgnores = new List<Collider2D>();
    public Collider2D coll;

    void Start()
    {
        entity = transform.root.gameObject.GetComponent<PhysicsEntity>();
        coll = GetComponent<Collider2D>();

        foreach(Collider2D ignore in DefaultIgnores)
        {
            Physics2D.IgnoreCollision(coll, ignore);
        }
    }

    public void Update()
    {
        InvincibleTime = Mathf.MoveTowards(InvincibleTime, 0, Time.deltaTime);

        if(ignores.Count > 0)
        {
            foreach(Collider2D testFor in ignores)
            {
                if (testFor.gameObject.activeInHierarchy == false)
                {
                    ignores.Remove(testFor);
                    Physics2D.IgnoreCollision(coll, testFor, false);
                }
            }
        }
    }

    // Update is called once per frame
    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<HitBox>())
        {
            if (!entity.isCombatEntity || InvincibleTime > 0)
                return;
            HitBox hitbox = other.gameObject.GetComponent<HitBox>();
            if (hitbox == null)
                return;
            hitbox.collisions--;

            entity.HP -= hitbox.inflictDamage;
            entity.HitStun = hitbox.inflictHitStun;
            entity.HurtState = hitbox.inflictHurtState;
            entity.Velocity.x = hitbox.inflictXKnockback * other.transform.parent.localScale.x;

            if (hitbox.inflictYKnockback > 0)
            {
                entity.Velocity.y = hitbox.inflictYKnockback;

                entity.GroundPoints.Clear();
                entity.Grounded = false;
            }
            CamVariables.Screenshake = hitbox.inflictXKnockback / 40 + hitbox.inflictYKnockback / 50;
            
            HitFXController hitfx = Instantiate(HitFX, transform.position, Quaternion.identity).GetComponent<HitFXController>();
            hitfx.type = hitbox.type;

            hitbox.AttackConnected(gameObject);

            Physics2D.IgnoreCollision(coll, other, true);
            //ignores.Add(other);
        }
    }
}
