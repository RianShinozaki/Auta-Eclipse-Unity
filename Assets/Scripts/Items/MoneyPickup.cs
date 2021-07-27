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

    public AK.Wwise.Event PickupSound;

    Rigidbody2D rb;

    void Start()
    {
        anim = GetComponent<Animator>();
        AnimationPlayableUtilities.PlayClip(GetComponent<Animator>(), Clips[Type], out playableGraph);

        opo = GetComponent<ObjectPoolObject>();

        rb = GetComponent<Rigidbody2D>();

        float Direction = Random.Range(Mathf.PI / 4, Mathf.PI * 3 / 4);
        rb.velocity = new Vector2(3 * Mathf.Sin(Direction), 3 * Mathf.Cos(Direction));
    }

    // Update is called once per frame
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.root.GetComponent<PlayerController>())
        {
            //GameObject soundHelper = new GameObject("Sound Helper");
            //soundHelper.transform.position = transform.position;
            PickupSound.Post(gameObject);

            opo.RePool();
        }
    }

    void OnDestroy()
    {
        playableGraph.Destroy();
    }
}
