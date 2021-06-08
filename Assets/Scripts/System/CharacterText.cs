using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class CharacterText {
    public Sprite FaceImage;

    [Multiline(10)]
    [HideLabel]
    public string Text;
}