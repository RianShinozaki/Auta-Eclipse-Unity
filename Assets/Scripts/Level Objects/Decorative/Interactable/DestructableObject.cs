using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObject : MonoBehaviour
{
    public Sprite[] NormalState;
    public int SpriteIndex;
    public Sprite[] DestroyedState;
    public Sprite[] BurnedState;
    public Sprite[] fragmentsSprite;
    public int fragNum;
    public Vector2 FragXSpdRange;
    public Vector2 FragYSpdRange;

    int State = NORMALSTATE;
    const int NORMALSTATE = 0;
    const int DESTROYEDSTATE = 1;
    const int BURNEDSTATE = 2;

    public SpriteRenderer Rend;

    public GameObject Shard;
    
    void Start()
    {
        
    }


    [ExecuteAlways]
    void Update()
    {
        if (Rend == null)
            return;

        switch (State)
        {
            case NORMALSTATE:
                Rend.sprite = NormalState[SpriteIndex];
                break;
            case DESTROYEDSTATE:
                Rend.sprite = DestroyedState[SpriteIndex];
                break;
            case BURNEDSTATE:
                Rend.sprite = BurnedState[SpriteIndex];
                break;

        }

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {

        if(State != NORMALSTATE)
        {
            return;
        }

        if (collision.GetComponent<HitBox>())
        {
            CamVariables.Screenshake = 0.2f;

            HitBox hb = collision.GetComponent<HitBox>();
            if(hb.inflictDamage > 0 || hb.inflictXKnockback > 0)
            {

                State = DESTROYEDSTATE;

                for(int i = 0; i < fragNum; i++)
                {
                    float mult = hb.transform.position.x > transform.position.x ? -1 : 1;

                    Rigidbody2D rb = Instantiate(Shard, transform.position + transform.up * 0.5f, Quaternion.identity).GetComponent<Rigidbody2D>();
                    rb.velocity = new Vector2(Random.Range(FragXSpdRange.x, FragXSpdRange.y), Random.Range(FragYSpdRange.x, FragYSpdRange.y)) + 0.7f * new Vector2(hb.inflictXKnockback * mult, hb.inflictYKnockback);
                    rb.angularVelocity = -rb.velocity.x*60;
                    rb.GetComponent<SpriteRenderer>().sprite = fragmentsSprite[Random.Range(0, fragmentsSprite.Length)];
                }
            }
        }
    }
}
