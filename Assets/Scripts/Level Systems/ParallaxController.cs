using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxController : MonoBehaviour {
    public Transform cam;
    public float divide;
    public Vector3 offset;

    void Update() {
        transform.position = new Vector3(cam.position.x / divide, cam.position.y / divide, 10 - divide) + offset;
    }
}