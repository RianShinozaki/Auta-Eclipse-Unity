using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float LifeTime;
    public float FadeSpd;
    SpriteRenderer rend;

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        LifeTime -= Time.deltaTime;
        if(LifeTime <= 0)
        {
            rend.color -= new Color(0, 0, 0, FadeSpd * Time.deltaTime);
            if(rend.color.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
