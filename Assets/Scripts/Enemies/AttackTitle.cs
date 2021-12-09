using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AttackTitle : MonoBehaviour
{
    // Start is called before the first frame update

    TextMeshPro Text;
    public string text;
    float ySpeed = 0;
    float initY;
    float TimeAlive = 0;

    void Awake()
    {
        Text = GetComponentInChildren<TextMeshPro>();
        initY = transform.localPosition.y;

    }

    public void OnAwaken()
    {
        ySpeed = 1;
        Text.text = text;
        TimeAlive = 0;
    }

    private void FixedUpdate()
    {
        Text.text = text;
        if(transform.parent.transform.localScale.x == -1)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        } else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        if (transform.localPosition.y > initY || ySpeed != 0)
        {
            transform.position += new Vector3(0, ySpeed, 0) * 0.05f;
            ySpeed -= Time.fixedDeltaTime * 18;
            if(transform.localPosition.y <= initY)
            {
                ySpeed = 0;
                transform.localPosition = new Vector3(transform.localPosition.x, initY, transform.localPosition.z);
            }
        }
    }
}
