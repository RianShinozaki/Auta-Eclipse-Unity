using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutaDisintegrateController : MonoBehaviour
{
    // Start is called before the first frame update
    Material material;
    SpriteRenderer rend;
    float Timer = 0;
    void Start()
    {
        material = GetComponent<Renderer>().material;
        rend = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Timer += (1 - Timer) * Time.deltaTime * 6;

        material.SetFloat("_GlitchAmount", Timer * 0.015f);
        if (Timer > 1)
        {
            Destroy(gameObject);
        }
        rend.color = new Color(1, 1, 1, 1 - Timer);
    }
}
