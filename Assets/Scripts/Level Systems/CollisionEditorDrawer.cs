using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionEditorDrawer : MonoBehaviour
{
    // Start is called before the first frame update

    public SpriteRenderer rend;
    void Start()
    {
        rend.color = new Color(1, 1, 1, 0);
    }

}
