using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using AK.Wwise;

public class MoneyPickup : MonoBehaviour
{
    // Start is called before the first frame update

    public int Value;
    public int Type;

    public AnimationClip[] Clips;
    Animator anim;

    ObjectPoolObject opo;

    PlayableGraph playableGraph;
    bool setGraph;

    public AK.Wwise.Event PickupSound;

    Rigidbody2D rb;

    void OnEnable()
    {
        anim = GetComponent<Animator>();
        AnimationPlayableUtilities.PlayClip(GetComponent<Animator>(), Clips[Type], out playableGraph);
        setGraph = true;

        opo = GetComponent<ObjectPoolObject>();

        rb = GetComponent<Rigidbody2D>();

        float Direction = Random.Range(Mathf.PI / 4, Mathf.PI * 3 / 4);
        rb.velocity = new Vector2(3.5f * Mathf.Cos(Direction), 3.5f * Mathf.Sin(Direction));
    }

    // Update is called once per frame
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.root.GetComponent<PlayerController>())
        {
            //GameObject soundHelper = new GameObject("Sound Helper");
            //soundHelper.transform.position = transform.position;
            PickupSound.Post(gameObject);
            ObjectPool.Instance.SpawnObject("LightBurst", transform.position, Quaternion.identity);
            opo.RePool();
        }
    }

    void OnDestroy()
    {
        if(setGraph)
            playableGraph.Destroy();
    }
}
