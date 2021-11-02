using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class EnemyBullet : MonoBehaviour
{
    ObjectPoolObject opo;
    public float Speed;
    public HitBox hb;
    public AnimationClip clip;
    Animator anim;
    PlayableGraph playable;
    public float AliveTime;

    void Awake()
    {
        opo = GetComponent<ObjectPoolObject>();
        anim = GetComponent<Animator>();

        AnimationPlayableUtilities.PlayClip(GetComponent<Animator>(), clip, out playable);
        
    }
    public void OnEnable()
    {
        hb.collisions = 1;
        AliveTime = 3;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        opo.RePool();
    }
    void Update()
    {
        Debug.Log(transform.rotation.z);
        transform.position += new Vector3(Speed * transform.localScale.x * Mathf.Cos(transform.rotation.z), Speed * transform.localScale.y * Mathf.Sin(transform.rotation.z), 0) * Time.deltaTime;
        if(hb.collisions <= 0)
        {
            opo.RePool();
        }

        AliveTime -= Time.deltaTime;
        if(AliveTime <= 0)
        {
            opo.RePool();
        }
    }

    void OnDestroy()
    {
        playable.Destroy();
    }
}
