using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

public class PlayerHitboxController : MonoBehaviour
{
    // Start is called before the first frame update
    public AnimationClip clip;
    public Animator animator;
    PlayableGraph playableGraph;

    public GameObject[] HitBoxes;
    public int StoppedState = 0;
    GameObject parent;
    public GameObject currentHitBox;
    public float offsetX;

    private void Awake()
    {
        offsetX = Mathf.Abs(transform.localPosition.x);
    }
    void OnEnable()
    {
        StoppedState = 0;

        parent = transform.root.gameObject;
        AnimationPlayableUtilities.PlayClip(animator, clip, out playableGraph);

        transform.localScale = parent.transform.GetChild(0).localScale;
        transform.localPosition = new Vector3(offsetX * Mathf.Sign(transform.localScale.x), transform.localPosition.y, transform.localPosition.z);
    }

    private void FixedUpdate()
    {
        if(parent.GetComponent<StateMachine>().CurrentState != parent.GetComponent<PlayerController>().State_Attack)
        {
            gameObject.SetActive(false);
        }
    }

    void OnDisable()
    {
        playableGraph.Destroy();
        parent.GetComponent<PlayerController>().currentHitBox = null;
    }

    void CancelLag()
    {
        parent.GetComponent<PlayerController>().AttackLag = false;
    }

    public void CallHitBox(int index)
    {
        HitBoxes[index].SetActive(true);
        currentHitBox = HitBoxes[index];
    }
    public void DeactivateHitBox(int index)
    {
        HitBoxes[index].SetActive(false);
    }
}