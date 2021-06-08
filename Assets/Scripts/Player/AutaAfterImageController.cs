using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutaAfterImageController : MonoBehaviour
{
    // Start is called before the first frame update
    Material material;
    float Timer = 0;
    void Start()
    {
        material = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime*3.5f;
        material.SetFloat("_Timer", Timer);
        if (Timer > 1)
        {
            Destroy(gameObject);
        }
    }
}
