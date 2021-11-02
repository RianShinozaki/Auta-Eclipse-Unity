using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimation : MonoBehaviour
{
    public enum AnimMode
    {
        LOOP,
        ONETIME_DEACTIVATE,
        ONETIME_DESTROY
    }
    public AnimMode mode;
    public float Speed;
    public Sprite[] Frames;
    float t;
    Image image;

    public bool CauseShake;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {
        if(CauseShake)
        {
            CamVariables.Screenshake = 0.4f;
        }
        image.sprite = Frames[Mathf.FloorToInt(t)];
        t += Speed * Time.deltaTime;
        if (t >= Frames.Length)
        {
            switch(mode)
            {
                case AnimMode.LOOP:
                    t -= Frames.Length;
                    break;
                case AnimMode.ONETIME_DEACTIVATE:
                    t = 0;
                    gameObject.SetActive(false);
                    break;
                case AnimMode.ONETIME_DESTROY:
                    Destroy(gameObject);
                    break;
            }
        }
    }
}
