using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class GradientLightFlicker : MonoBehaviour
{
    public float TimeToFlicker;
    public float t;
    public int FlickerNum;
    public int state = 0;
    public float NormalFalloff;
    public float FlickerFalloffMult;

    public SpriteRenderer Sprite;
    public Light2D ThisLight;

    Color ogColor;
    float ogIntensity;

    public Vector2 NormalTimeToFlicker = new Vector2(3, 5);
    public Vector2Int NormalFlickerNum = new Vector2Int(3, 4);

    // Start is called before the first frame update
    void Start()
    {
        TimeToFlicker = Random.Range(NormalTimeToFlicker.x, NormalTimeToFlicker.y);
        FlickerNum = Random.Range(NormalFlickerNum.x, NormalFlickerNum.y);

        ogColor = Sprite.color;
        ogIntensity = ThisLight.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case 0:
                t += Time.deltaTime;

                Sprite.color = new Color(ogColor.r, ogColor.g, ogColor.b, Random.Range(ogColor.a - 0.05f, ogColor.a + 0.05f));

                if (t > TimeToFlicker)
                {
                    state = 1;
                    t = 0;
                }
                break;
            case 1:
                Sprite.color = Color.Lerp(ogColor, new Color(ogColor.r, ogColor.g, ogColor.b, 0), t);
                ThisLight.intensity = Mathf.Lerp(ogIntensity, 0, t);

                t += Time.deltaTime * NormalFalloff;
                if (t >= 1) {
                    t = 0;
                    state = 2;

                }
                break;
            case 2:
                Sprite.color = Color.Lerp(new Color(ogColor.r, ogColor.g, ogColor.b, ogColor.a/2), new Color(ogColor.r, ogColor.g, ogColor.b, 0), t);
                ThisLight.intensity = Mathf.Lerp(ogIntensity*0.5f, 0, t);

                t += Time.deltaTime * NormalFalloff * FlickerFalloffMult;
                if (t >= 1)
                {
                    if(FlickerNum == 0)
                    {
                        state = 3;
                        t = 0;
                    } else
                    {
                        FlickerNum--;
                        t = 0;
                    }
                }
                break;
            case 3:
                Sprite.color = Color.Lerp(new Color(ogColor.r, ogColor.g, ogColor.b, 0), ogColor, t);
                ThisLight.intensity = Mathf.Lerp(0, ogIntensity, t);

                t += Time.deltaTime * NormalFalloff;
                if (t >= 1)
                {
                    t = 0;
                    state = 0;

                    TimeToFlicker = Random.Range(3, 5);
                    FlickerNum = Random.Range(3, 4);

                    Sprite.color = ogColor;
                    ThisLight.intensity = ogIntensity;
                }
                break;
        }
    }
}
