using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AutaUnity/Data/PowerChip Master")]
public class PCContainer : ScriptableObject
{
    public PowerChip[] Data;
}

[System.Serializable]
public struct PowerChip
{
    public string Name;
    [Multiline(10)] public string Description;
    public float MPCost;
    public GameObject CodeRunner;
}

