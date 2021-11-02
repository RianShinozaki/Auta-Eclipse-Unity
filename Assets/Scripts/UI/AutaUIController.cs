using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutaUIController : MonoBehaviour
{
    public static AutaUIController Instance;
    public PlayerController Player;

    public RectTransform Frame;
    public RectTransform HealthBar;
    public RectTransform MPBar;

    Vector2 FrameInitPos;
    Vector2 HealthBarInitPos;
    Vector2 MPBarInitPos;
    float HealthBarInitWidth;
    float MPBarInitWidth;

    public GameObject[] ScreenFX;

    // Update is called once per frame
    private void Start()
    {
        Instance = this;
        FrameInitPos = Frame.position;
        HealthBarInitPos = HealthBar.position;
        HealthBarInitWidth = HealthBar.sizeDelta.x;
        MPBarInitPos = MPBar.position;
        MPBarInitWidth = MPBar.sizeDelta.x;
    }
    void Update()
    {
        if(CamVariables.Screenshake > 0)
        {
            float shakeX = Random.Range(-CamVariables.Screenshake, CamVariables.Screenshake) * 10;
            float shakeY = Random.Range(-CamVariables.Screenshake, CamVariables.Screenshake) * 10;

            Frame.position = FrameInitPos + new Vector2(shakeX, shakeY);
            HealthBar.position = HealthBarInitPos + new Vector2(shakeX, shakeY);
            MPBar.position = MPBarInitPos + new Vector2(shakeX, shakeY);
        } else
        {
            Frame.position = FrameInitPos;
            HealthBar.position = HealthBarInitPos;
            MPBar.position = MPBarInitPos;
        }

        if(Player == null)
        {
            return;
        }

        HealthBar.sizeDelta = new Vector2(HealthBarInitWidth * (Player.HP / Player.MaxHP), HealthBar.sizeDelta.y);
        MPBar.sizeDelta = new Vector2(MPBarInitWidth * (Player.MP / Player.MaxMP), MPBar.sizeDelta.y);
    }
    public void ActivateScreenFX(int FX)
    {
        ScreenFX[FX].SetActive(true);
    }
}
