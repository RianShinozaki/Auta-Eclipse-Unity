using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PhysicsEntity : MonoBehaviour
{

    #region Physics System

    [FoldoutGroup("Physics")] public Vector2 Velocity;
    [FoldoutGroup("Physics")] public float Gravity = 18;
    [FoldoutGroup("Physics")] public float GravityCancelTime;
    [FoldoutGroup("Physics")] public bool Grounded;
    [FoldoutGroup("Physics")] public bool Bounce;
    [FoldoutGroup("Physics")] public float BounceMult = 0.9f;
    [FoldoutGroup("Physics")] public float maxFallSpeed = -10;
    [FoldoutGroup("Physics")] public float TimeScale = 1;

    [FoldoutGroup("Manual Setup")] public Rigidbody2D rb;
    [FoldoutGroup("Manual Setup")] public Collider2D coll;
    [FoldoutGroup("Manual Setup")] public LayerMask EnvironmentMask;

    [FoldoutGroup("Setup")] public GameObject HitFX;
    [FoldoutGroup("Setup")] public float AfterIMGTimer = 0;
    [FoldoutGroup("Setup")] public bool CreateAfterImg = false;
    [FoldoutGroup("Setup")] public GameObject AfterIMG;
    [FoldoutGroup("Setup")] public Animator Anim;

    #endregion Physics

    [FoldoutGroup("Combat")] public bool isCombatEntity;
    [FoldoutGroup("Combat")] public float HP;
    [FoldoutGroup("Combat")] public int MaxHP;
    [FoldoutGroup("Combat")] public float HitStun;
    [FoldoutGroup("Combat")] public float HurtState;

    [FoldoutGroup("Collisions")] public List<ContactPoint2D> GroundPoints = new List<ContactPoint2D>();
    [FoldoutGroup("Collisions")] public List<ContactPoint2D> WallPoints = new List<ContactPoint2D>();
    [FoldoutGroup("Collisions")] public List<ContactPoint2D> CeilingPoints = new List<ContactPoint2D>();

    public int Mode;
    public int SubMode;

    [FoldoutGroup("Collisions")] public bool Landed = false;
    
    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (Bounce)
        {
            Velocity = Vector2.Reflect(Velocity, collision.contacts[0].normal) * BounceMult;

        }
    }
    public virtual void OnCollisionStay2D(Collision2D collision)
    {
        if (!Bounce)
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                float angle = Vector2.SignedAngle(Vector2.up, contact.normal);
                if (angle >= -45 && angle <= 45 && (Velocity.y <= 0 || Grounded))
                {
                    GroundPoints.Add(contact);
                }
                else if (angle > 135 || angle < -135)
                {
                    CeilingPoints.Add(contact);
                } 
                else if( (angle > 45  && angle <= 135) || (angle >= -135 && angle < -45) )
                {
                    WallPoints.Add(contact);
                }

            }
        } else
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                float angle = Vector2.SignedAngle(Vector2.up, contact.normal);
                if (angle >= -45 && angle <= 45)
                {
                    Landed = true;
                }
            }
        }
    }

    public virtual void Awake() 
    {
        HitFX = Resources.Load<GameObject>("FX/HitFX");

        Anim = GetComponent<Animator>();
        if (Anim == null)
        {
            Anim = GetComponentInChildren<Animator>();
        }

        //StartCoroutine(CreateAfterImgEnum());
    }

    public virtual void OnLand()
    {

    }


    // Update is called once per frame
    public virtual void Update()
    {
        if(HitStun > 0)
        {
            HitStun = Mathf.MoveTowards(HitStun, 0, Time.deltaTime * TimeScale * 60);
            return;
        }

        if (rb == null)
        {
            rb = gameObject.GetComponent<Rigidbody2D>();
        }
        Vector3 vel = new Vector3(0, 0, 0);

        if (!Bounce)
        {
            int wallPointsNum = 0;
            float avgAngle = 0;


            foreach (ContactPoint2D contact in WallPoints)
            {

                wallPointsNum++;

                avgAngle += Vector2.SignedAngle(Vector2.up, contact.normal);
            }
            if (wallPointsNum > 0)
            {
                avgAngle /= wallPointsNum;
                if (avgAngle > 0)
                {
                    float toSpdx = Mathf.Cos(avgAngle * Mathf.Deg2Rad) * Velocity.y;
                    if (Velocity.x > toSpdx)
                    {
                        Velocity.x = toSpdx;
                    }
                }

                if (avgAngle < 0)
                {
                    float toSpdx = Mathf.Cos(avgAngle * Mathf.Deg2Rad) * Velocity.y;
                    if (Velocity.x < toSpdx)
                    {
                        Velocity.x = toSpdx;
                    }
                }

            }

            int ceilPointsNum = 0;
            avgAngle = 0;

            foreach (ContactPoint2D contact in CeilingPoints)
            {

                ceilPointsNum++;

                avgAngle += Vector2.SignedAngle(Vector2.up, contact.normal);
            }
            if (ceilPointsNum > 0)
            {
                avgAngle /= ceilPointsNum;

                if (Velocity.y > Mathf.Sin(avgAngle * Mathf.Deg2Rad) * Velocity.y)
                {
                    Velocity.y = Mathf.Sin(avgAngle * Mathf.Deg2Rad) * Velocity.y;
                }

            }

            int groundPointsNum = 0;
            avgAngle = 0;


            foreach (ContactPoint2D contact in GroundPoints)
            {

                groundPointsNum++;

                avgAngle += Vector2.SignedAngle(Vector2.up, contact.normal);
            }

            if (groundPointsNum == 0)
            {

                if (GravityCancelTime == 0)
                {
                    Velocity.y = Mathf.MoveTowards(Velocity.y, maxFallSpeed, Gravity * Time.deltaTime * TimeScale);
                }
                else
                {
                    GravityCancelTime = Mathf.MoveTowards(GravityCancelTime, 0, Time.deltaTime * TimeScale);
                }
                vel.x = Velocity.x;

                if (Velocity.y < 0) //If falling, look for collision as to land on it perfectly
                {
                    Grounded = false;


                    RaycastHit2D hit;
                    hit = Physics2D.Raycast(new Vector2(transform.position.x, coll.bounds.center.y - coll.bounds.extents.y), Vector2.down, Velocity.y * Time.deltaTime * TimeScale, EnvironmentMask);

                    if (hit.collider != null) //Check for a collision
                    {
                        transform.position = hit.point + new Vector2(0, coll.bounds.extents.y) * 0.95f; //Move to collision point
                        transform.position -= new Vector3(0, Velocity.y * Time.deltaTime * TimeScale, 0);

                        if(Velocity.y < 0)
                        {
                            OnLand();
                        }
                        Velocity.y = 0;
                    }
                }
            }
            else
            {
                if(Grounded == false)
                {
                    OnLand();
                }
                Grounded = true;
                avgAngle /= groundPointsNum;
                Velocity.y = Mathf.Sin(avgAngle * Mathf.Deg2Rad) * Velocity.x;
                vel.x = Mathf.Cos(avgAngle * Mathf.Deg2Rad) * Velocity.x;
            }
        } else
        {
            Velocity.y -= Gravity * Time.deltaTime * TimeScale;
            vel = new Vector3(Velocity.x, Velocity.y, 0);
        }
        

        vel.y = Velocity.y;
        rb.velocity = vel * TimeScale;
        //transform.position += 

        if (rb.IsAwake())
        {
            GroundPoints.Clear();
            WallPoints.Clear();
            CeilingPoints.Clear();
        }

    }
    public virtual void FixedUpdate()
    {
        
    }

    public virtual void HitResponse(GameObject attacker, GameObject Defender)
    {

    }

    public virtual void StartCreatingAfterImgs()
    {
        StartCoroutine(CreateAfterImgEnum());
    }
    public IEnumerator CreateAfterImgEnum()
    {
        CreateAfterImg = true;
        while (CreateAfterImg)
        {
            AfterIMGTimer += Time.deltaTime;

            if (AfterIMGTimer > 0.07f)
            {
                GameObject img = Instantiate(AfterIMG, transform.position, Quaternion.identity);
                img.GetComponent<SpriteRenderer>().sprite = Anim.GetComponent<SpriteRenderer>().sprite;
                img.transform.localScale = Anim.transform.localScale;
                AfterIMGTimer = 0;
            }
            yield return null;
        }

    }
}
