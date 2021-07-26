using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxController : MonoBehaviour {
    public Transform cam;
    public float divide;
    public Vector3 offset;

    public bool XLock;
    public bool YLock;

    void Update() {
        transform.position = new Vector3(  cam.position.x / (XLock ? 1 :divide), cam.position.y / (YLock ? 1 : divide), 10 - divide) + offset;
    }
}