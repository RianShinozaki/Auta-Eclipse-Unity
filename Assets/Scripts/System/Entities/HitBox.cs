using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType
{
    Slice,
    EnemyDeath,
    Grab,
    Bash,
    Burn,
    Zap
}
public class HitBox : MonoBehaviour
{
    public float inflictDamage;
    public float inflictHitStun;
    public float inflictHurtState;
    public float inflictXKnockback;
    public float inflictYKnockback;
    public int collisions = -1; //-1 means it will never deactivate
    public int StoppedState = 0;
    public int ActiveTime = -1;
    public PhysicsEntity entityResponder;
    public DamageType type;

    public Collider2D coll;

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<Collider2D>();
        entityResponder = transform.root.GetComponent<PhysicsEntity>();
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
            ActiveTime--;
            if(ActiveTime == 0)
            {
                coll.enabled = false;
            }
        }
    }

    public void AttackConnected(GameObject defender)
    {
        if(entityResponder != null)
        {
            entityResponder.HitResponse(gameObject, defender);
        }

    }
}
