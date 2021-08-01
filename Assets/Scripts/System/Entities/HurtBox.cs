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

    public GameObject DamageDrawerPrefab;
    GameObject DamageDrawerInstance;

    void Start()
    {
        DamageDrawerPrefab = Resources.Load<GameObject>("FX/pre_DamageDrawer");

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

            AffinityData.Affinity affinity = AffinityData.Affinity.Normal;

            #region AffinityChecks
            if (hitbox.Affinity == InflictType.Slash) {
                affinity = entity.Affinities.Slash;
            }
            if (hitbox.Affinity == InflictType.Bash)
            {
                affinity = entity.Affinities.Bash;
            }
            if (hitbox.Affinity == InflictType.Pierce)
            {
                affinity = entity.Affinities.Pierce;
            }
            if (hitbox.Affinity == InflictType.Solar)
            {
                affinity = entity.Affinities.Solar;
            }
            if (hitbox.Affinity == InflictType.Crystal)
            {
                affinity = entity.Affinities.Crystal;
            }
            if (hitbox.Affinity == InflictType.Sonic)
            {
                affinity = entity.Affinities.Sonic;
            }
            #endregion

            float AffinityDamageMult = 1;
            float AffinityTechnicalMult = 1;

            if(affinity == AffinityData.Affinity.Weak)
            {
                AffinityDamageMult = 1.5f;
                AffinityTechnicalMult = 2;
            }
            if (affinity == AffinityData.Affinity.Strong)
            {
                AffinityDamageMult = 0.5f;
                AffinityTechnicalMult = 0;
            }
            if (affinity == AffinityData.Affinity.Immune)
            {
                AffinityDamageMult = 0;
                AffinityTechnicalMult = 0;
            }

            float CriticalMult = 1;

            float netAttackerTechnical = hitbox.TechnicalAdd;
            if(hitbox.Entity != null)
            {
                netAttackerTechnical += hitbox.Entity.BaseTechnical;
            }

            netAttackerTechnical *= AffinityTechnicalMult;

            netAttackerTechnical -= entity.BaseTechnical;

            if(netAttackerTechnical > 0)
            {
                int attktechint = Mathf.Clamp(Mathf.CeilToInt(netAttackerTechnical), 1, 15);

                int TechnicalChance = Random.Range(attktechint, 16);

                Debug.Log("possibility of tech:" + attktechint);
                Debug.Log("tech:" + TechnicalChance);

                CriticalMult += 1.5f * (TechnicalChance == 15 ? 1 : 0);
            }

            entity.HP -= ( (hitbox.inflictDamage + entity.BaseAttack) * AffinityDamageMult * CriticalMult - entity.BaseDefense);
            entity.HitStun = hitbox.inflictHitStun + (affinity == AffinityData.Affinity.Weak ? 2 : 0);

            if(DamageDrawerInstance == null || DamageDrawerInstance.activeInHierarchy == false )
            {
                DamageDrawerInstance = ObjectPool.Instance.SpawnObject("DamageDrawer", transform.position, Quaternion.identity); 
            }

            if (CriticalMult != 1)
            {
                DamageDrawerInstance.GetComponent<DamageDrawer>().CriticalHit(transform.position, ((hitbox.inflictDamage + entity.BaseAttack) * AffinityDamageMult * CriticalMult - entity.BaseDefense));
            }
            else
            {
                DamageDrawerInstance.GetComponent<DamageDrawer>().NormalHit(transform.position, ((hitbox.inflictDamage + entity.BaseAttack) * AffinityDamageMult * CriticalMult - entity.BaseDefense));
            }

            float TotalKnockback = new Vector2(hitbox.inflictXKnockback, hitbox.inflictYKnockback).magnitude;

            if (TotalKnockback - entity.BaseSturdiness > entity.KnockbackHurtstateThreshold)
            {
                entity.HurtState = hitbox.inflictHurtState + (affinity == AffinityData.Affinity.Weak ? 30 : 0);
            }

            entity.Velocity.x = Mathf.Clamp(hitbox.inflictXKnockback - entity.BaseSturdiness, 0, 12) * other.transform.parent.localScale.x;

            if (hitbox.inflictYKnockback > 0)
            {
                entity.Velocity.y = Mathf.Clamp(hitbox.inflictYKnockback - entity.BaseSturdiness, 0, 12);

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
