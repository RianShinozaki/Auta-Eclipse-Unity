using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LumpyAnim : MonoBehaviour
{
    Lumpy Entity;
    private void Start()
    {
        Entity = GetComponentInParent<Lumpy>();
    }
    public void Strike()
    {
        Entity.Strike();
    }
}
