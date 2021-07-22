using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class HitFXController : MonoBehaviour
{
    // Start is called before the first frame update
    // Start is called before the first frame update
    public AnimationClip[] clips;
    public Animator animator;
    PlayableGraph playableGraph;

    public DamageType type;

    private void Start()
    {
        AnimationPlayableUtilities.PlayClip(animator, clips[(int)type], out playableGraph);
        transform.position += new Vector3(0, 0, -1);
    }

    public void AnimEnd()
    {
        playableGraph.Destroy();
        Destroy(gameObject);
    }

    
}
