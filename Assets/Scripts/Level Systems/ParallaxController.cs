using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    public GameObject cam;
    public float divide;
    public Vector3 offset;

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(cam.transform.position.x / divide, cam.transform.position.y / divide, 10-divide) + offset;
    }
}
