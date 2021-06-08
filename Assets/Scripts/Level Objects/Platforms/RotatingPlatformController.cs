using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPlatformController : MonoBehaviour
{
    float degrees = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        degrees += 2 * Time.deltaTime;
        transform.eulerAngles = Vector3.forward * degrees;
    }
}
