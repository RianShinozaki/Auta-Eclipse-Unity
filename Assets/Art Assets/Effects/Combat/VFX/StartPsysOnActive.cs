using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class StartPsysOnActive : MonoBehaviour
{
    public ParticleSystem psys;

    private void OnEnable()
    {
        psys.Play();
    }
}
