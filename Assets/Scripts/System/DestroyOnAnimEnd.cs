using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnAnimEnd : MonoBehaviour
{
    // Start is called before the first frame update
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
