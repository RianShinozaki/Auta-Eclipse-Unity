using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using AK.Wwise;
using Sirenix.OdinInspector;

public class HitFXController : MonoBehaviour
{
    // Start is called before the first frame update
    // Start is called before the first frame update
    public AnimationClip[] clips;
    public Animator animator;
    PlayableGraph playableGraph;

    public EffectType type;

    [FoldoutGroup("Sounds")] public AK.Wwise.Event SlashImpact;

    private void Start()
    {
        AnimationPlayableUtilities.PlayClip(animator, clips[(int)type], out playableGraph);
        transform.position += new Vector3(0, 0, -1);
        SlashImpact.Post(gameObject);
    }

    public void AnimEnd()
    {
        playableGraph.Destroy();
        Destroy(gameObject);
    }

    
}
