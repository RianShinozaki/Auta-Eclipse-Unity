using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Auta/Character Dialogue")]
public class Dialogue : ScriptableObject {
    [HideLabel]
    public CharacterText[] Lines;
}