using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEditor.VFX;
using UnityEditor.Experimental.Rendering;
using UnityEditor.VFX.UI;

public class SolarBurst : MonoBehaviour
{
    public UnityEngine.Rendering.Universal.Light2D myLight;
    public UnityEngine.VFX.VisualEffect visualEffect;

    // Start is called before the first frame update
    void Start()
    {
        visualEffect.Play();
        CamVariables.Screenshake = 0.2f;
        StartCoroutine(burst());
    }

    public IEnumerator burst()
    {
        float time = 0;
        float bursts = 0;
        while (bursts < 3)
        {
            time = 0;
            while (time < 0.1f)
            {
                time += Time.deltaTime;
                yield return null;
            }
            visualEffect.Play();
            bursts += 1;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(visualEffect.aliveParticleCount == 0)
        {
           // Destroy(gameObject);
        }
    }
}
