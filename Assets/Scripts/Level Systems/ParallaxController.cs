using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteAlways]

public class ParallaxController : MonoBehaviour {
    public Transform cam;
    public float divide;
    public Vector3 offset;

    public bool XInfluence;
    public bool YInfluence;

    public bool XLock;
    public bool YLock;

    void Update() {
        transform.position = new Vector3(  XInfluence ? cam.position.x / (XLock ? 1 :divide) : transform.position.x, YInfluence ? cam.position.y / (YLock ? 1 : divide) + offset.y: transform.position.y, transform.position.z);
    }
}