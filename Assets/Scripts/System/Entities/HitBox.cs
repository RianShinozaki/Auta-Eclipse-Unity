using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectType
{
    Slice,
    EnemyDeath,
    Grab,
    Bash,
    Pierce,
    Burn,
    Zap
}

public enum InflictType
{
    Slash,
    Bash,
    Pierce,

    Solar,
    Crystal,
    Sonic
}
public class HitBox : MonoBehaviour
{
    public float inflictDamage;
    public float inflictHitStun;
    public float inflictHurtState;
    public float inflictXKnockback;
    public float inflictYKnockback;
    public float TechnicalAdd;

    public int collisions = -1; //-1 means it will never deactivate
    public int StoppedState = 0;
    public float ActiveTime = -1;
    public PhysicsEntity Entity;

    public bool InflictRecoil = true;

    public EffectType type;
    public InflictType Affinity;

    public Collider2D coll;

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<Collider2D>();
        if (Entity == null)
        {
            Entity = transform.root.GetComponent<PhysicsEntity>();
        }
    }

    private void OnEnable()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(collisions == 0)
        {
            Destroy(transform.root.gameObject);
        }
        if(ActiveTime > 0)
        {
            coll.enabled = true;
            ActiveTime = Mathf.MoveTowards(ActiveTime, 0, 60* Time.deltaTime);
            if(ActiveTime == 0)
            {
                coll.enabled = false;
            }
        }
    }

    public void AttackConnected(GameObject defender)
    {
        if(Entity != null && InflictRecoil)
        {
            Entity.HitResponse(gameObject, defender);
        }

    }
}
