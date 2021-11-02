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
                if (testFor.gameObject.activeInHierarchy == false || testFor.isActiveAndEnabled == false)
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

            Debug.Log("Full hit log -- Defender: " + entity.name + "| Attacker: " + other.gameObject.transform.root.name);

            if (!entity.isCombatEntity || InvincibleTime > 0)
            {
                Debug.Log("Not combat entity, or has iFrames");
                return;
            }
            HitBox hitbox = other.gameObject.GetComponent<HitBox>();
            if (hitbox == null)
            {
                Debug.Log("Error - No hitbox");
                return;
            }

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

            if (!entity.Staggered)
            {
                entity.Stagger -= ((hitbox.inflictDamage + entity.BaseAttack) * AffinityDamageMult * CriticalMult - entity.BaseDefense);
                if(entity.Stagger <= 0)
                {
                    entity.Staggered = true;
                    entity.StaggerRecoverySpd = 1;
                }
            }

            entity.HP -= ( (hitbox.inflictDamage + entity.BaseAttack) * AffinityDamageMult * CriticalMult - entity.BaseDefense) / (entity.Staggered ? 1 : 2);
            entity.HitStun = hitbox.inflictHitStun + (affinity == AffinityData.Affinity.Weak ? 2 : 0);

            if(DamageDrawerInstance == null || DamageDrawerInstance.activeInHierarchy == false )
            {
                DamageDrawerInstance = ObjectPool.Instance.SpawnObject("DamageDrawer", transform.position, Quaternion.identity); 
            }

            DamageDrawerInstance.GetComponent<DamageDrawer>().Hit(transform.position, ((hitbox.inflictDamage + entity.BaseAttack) * AffinityDamageMult * CriticalMult - entity.BaseDefense) / (entity.Staggered ? 1 : 2), CriticalMult != 1, affinity == AffinityData.Affinity.Weak);

            float TotalKnockback = new Vector2(hitbox.inflictXKnockback, hitbox.inflictYKnockback).magnitude / (entity.Staggered ? 1 : 2);

            if (TotalKnockback - entity.BaseSturdiness > entity.KnockbackHurtstateThreshold || entity.Staggered)
            {
                entity.HurtState = hitbox.inflictHurtState + (affinity == AffinityData.Affinity.Weak ? 30 : 0);
            }

            entity.Velocity.x = Mathf.Clamp(hitbox.inflictXKnockback - entity.BaseSturdiness, 0, 12) * other.transform.parent.localScale.x / (entity.Staggered ? 1 : 4);

            if (hitbox.inflictYKnockback > 0 && entity.Staggered)
            {
                entity.Velocity.y = Mathf.Clamp(hitbox.inflictYKnockback - entity.BaseSturdiness, 0, 12);

                entity.GroundPoints.Clear();
                entity.Grounded = false;
            }

            CamVariables.Screenshake = hitbox.inflictXKnockback / 40 + hitbox.inflictYKnockback / 50;
            
            HitFXController hitfx = Instantiate(HitFX, transform.position, Quaternion.identity).GetComponent<HitFXController>();
            hitfx.type = hitbox.type;

            hitbox.AttackConnected(gameObject);
            entity.HurtResponse();

            Physics2D.IgnoreCollision(coll, other, true);
            ignores.Add(other);

            if(entity.MaxStagger == 0)
            {
                entity.Staggered = false;
            }
        }
    }
}
