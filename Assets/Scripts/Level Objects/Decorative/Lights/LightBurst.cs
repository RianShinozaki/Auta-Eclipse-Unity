using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LightBurst : MonoBehaviour
{
    // Start is called before the first frame update
    UnityEngine.Rendering.Universal.Light2D thisLight;
    public int Preset;

    ObjectPoolObject opo;

    public void Awake()
    {
        thisLight = GetComponent<UnityEngine.Rendering.Universal.Light2D>();
        opo = GetComponent<ObjectPoolObject>();
    }
    public void OnEnable()
    {
        thisLight.intensity = 1;
    }
    public void Start()
    {
        thisLight.intensity = 1;
    }

    // Update is called once per frame
    void Update()
    {
        thisLight.intensity -= Time.deltaTime;
        if(thisLight.intensity <= 0)
        {
            opo.RePool();
            thisLight.intensity = 1;
        }
    }
}
