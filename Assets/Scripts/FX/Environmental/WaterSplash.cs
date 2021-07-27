using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class WaterSplash : MonoBehaviour
{
    public Animator anim;
    public AnimationClip Clip;

    PlayableGraph playableGraph;

    public AK.Wwise.Event Sound_Splash;
    // Update is called once per frame

    private void Start()
    {
        anim = GetComponent<Animator>();
        AnimationPlayableUtilities.PlayClip(GetComponent<Animator>(), Clip, out playableGraph);
        Sound_Splash.Post(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        transform.position = new Vector3(collision.ClosestPoint(transform.position).x, collision.ClosestPoint(transform.position).y, 0);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
        playableGraph.Destroy();
    }
}
