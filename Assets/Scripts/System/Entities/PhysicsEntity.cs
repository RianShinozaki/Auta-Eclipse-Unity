using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using AK.Wwise;

public class PhysicsEntity : MonoBehaviour
{

    #region Physics System

    public AffinityData Affinities;

    [FoldoutGroup("Physics")] public Vector2 Velocity;
    [FoldoutGroup("Physics")] public float Gravity = 18;
    [FoldoutGroup("Physics")] public float GravityCancelTime;
    [FoldoutGroup("Physics")] public bool Grounded;
    [FoldoutGroup("Physics")] public bool Groundray;
    [FoldoutGroup("Physics")] public float UngroundedTimer;
    [FoldoutGroup("Physics")] public bool Bounce;
    [FoldoutGroup("Physics")] public float BounceMult = 0.9f;
    [FoldoutGroup("Physics")] public float maxFallSpeed = -10;
    [FoldoutGroup("Physics")] public float TimeScale = 1;

    [FoldoutGroup("Manual Setup")] public Rigidbody2D rb;
    [FoldoutGroup("Manual Setup")] public Collider2D coll;
    [FoldoutGroup("Manual Setup")] public LayerMask EnvironmentMask;
    [FoldoutGroup("Manual Setup")] public float UnderwaterDragScale = 0.7f;

    [FoldoutGroup("Resources")] public GameObject HitFX;
    [FoldoutGroup("Resources")] public GameObject SplashFX;
    

    [FoldoutGroup("Setup")] public float AfterIMGTimer = 0;
    [FoldoutGroup("Setup")] public bool CreateAfterImg = false;
    [FoldoutGroup("Setup")] public GameObject AfterIMG;
    [FoldoutGroup("Setup")] public Animator Anim;
    [FoldoutGroup("Setup")] public bool Underwater;

    #endregion Physics

    [FoldoutGroup("Combat")] public bool isCombatEntity;
    [FoldoutGroup("Combat")] public float HP;
    [FoldoutGroup("Combat")] public int MaxHP;
    [FoldoutGroup("Combat")] public float HitStun;
    [FoldoutGroup("Combat")] public float HurtState;

    [FoldoutGroup("Combat")] public float BaseAttack;
    [FoldoutGroup("Combat")] public float BaseDefense;
    [FoldoutGroup("Combat")] public float BaseSturdiness;
    [FoldoutGroup("Combat")] public float KnockbackHurtstateThreshold;
    [FoldoutGroup("Combat")] public float BaseTechnical;
    [FoldoutGroup("Combat")] public float MaxStagger;
    [FoldoutGroup("Combat")] public float Stagger;
    [FoldoutGroup("Combat")] public bool Staggered;
    [FoldoutGroup("Combat")] public float StaggerRecoverySpd;

    [FoldoutGroup("Identity")] public string Name = "";
    [FoldoutGroup("Identity")] public float Level = 1;

    [FoldoutGroup("Collisions")] public List<ContactPoint2D> GroundPoints = new List<ContactPoint2D>();
    [FoldoutGroup("Collisions")] public List<ContactPoint2D> WallPoints = new List<ContactPoint2D>();
    [FoldoutGroup("Collisions")] public List<ContactPoint2D> CeilingPoints = new List<ContactPoint2D>();

    [FoldoutGroup("States")] public int Mode;
    [FoldoutGroup("States")] public int SubMode;

    [FoldoutGroup("Collisions")] public bool Landed = false;
    
    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (Bounce)
        {
            Velocity = Vector2.Reflect(Velocity, collision.contacts[0].normal) * BounceMult;
            rb.velocity = new Vector3(Velocity.x, Velocity.y, 0);

            Debug.Log("Bounce");
            OnBounce();
        }
    }
    public virtual void OnCollisionStay2D(Collision2D collision)
    {
        if (!Bounce)
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                float angle = Vector2.SignedAngle(Vector2.up, contact.normal);
                if (angle > 135 || angle < -135)
                {
                    CeilingPoints.Add(contact);
                }
                else if ((angle > 45 && angle <= 135) || (angle >= -135 && angle < -45))
                {
                    WallPoints.Add(contact);
                }

            }
        }
        else
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
        HitFX = Resources.Load<GameObject>("FX/pre_HitFX");
        SplashFX = Resources.Load<GameObject>("FX/pre_WaterSplash");

        Anim = GetComponent<Animator>();
        if (Anim == null)
        {
            Anim = GetComponentInChildren<Animator>();
        }

        //StartCoroutine(CreateAfterImgEnum());
    }

    public virtual void OnLand() { }
    public virtual void OnBounce() { }


    // Update is called once per frame
    public virtual void FixedUpdate()
    {
        



        if (rb == null)
        {
            rb = gameObject.GetComponent<Rigidbody2D>();
        }

        if (!Bounce)
        {
            int wallPointsNum = 0;
            float avgAngle2 = 0;


            foreach (ContactPoint2D contact in WallPoints)
            {

                wallPointsNum++;

                avgAngle2 += Vector2.SignedAngle(Vector2.up, contact.normal);
            }
            if (wallPointsNum > 0)
            {
                avgAngle2 /= wallPointsNum;
                if (avgAngle2 > 0)
                {
                    float toSpdx = Mathf.Cos(avgAngle2 * Mathf.Deg2Rad) * Velocity.y;
                    if (Velocity.x > toSpdx)
                    {
                        Velocity.x = toSpdx;
                    }
                }

                if (avgAngle2 < 0)
                {
                    float toSpdx = Mathf.Cos(avgAngle2 * Mathf.Deg2Rad) * Velocity.y;
                    if (Velocity.x < toSpdx)
                    {
                        Velocity.x = toSpdx;
                    }
                }

            }

            int ceilPointsNum = 0;
            avgAngle2 = 0;

            foreach (ContactPoint2D contact in CeilingPoints)
            {

                ceilPointsNum++;

                avgAngle2 += Vector2.SignedAngle(Vector2.up, contact.normal);
            }
            if (ceilPointsNum > 0)
            {
                avgAngle2 /= ceilPointsNum;

                if (Velocity.y > Mathf.Sin(avgAngle2 * Mathf.Deg2Rad) * Velocity.y)
                {
                    Velocity.y = Mathf.Sin(avgAngle2 * Mathf.Deg2Rad) * Velocity.y;
                }

            }

            int groundPointsNum = 0;
            Vector2 avgAngle = new Vector3(0, 0);
            float avgPos = 0;

            for(int i = 0; i < 3; i++)
            {
                float xpos = coll.bounds.center.x - coll.bounds.extents.x + (coll.bounds.extents.x) * i;
                float ypos = coll.bounds.center.y - coll.bounds.extents.y;
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(xpos, ypos), Vector2.down, (Grounded ? 0.5f : 0.2f), layerMask: EnvironmentMask);
                Debug.DrawLine(new Vector3(xpos, ypos, 0), new Vector3(xpos, ypos - 0.1f, 0));
                if (hit.collider != null)
                {
                    if (avgPos < hit.point.y) {
                        avgPos = hit.point.y;
                    }
                    avgAngle += hit.normal;
                    Debug.Log("Normal " + i.ToString() + ": " + hit.normal.ToString());
                    groundPointsNum++;

                    if (hit.rigidbody != null)
                    {
                        transform.position += new Vector3(hit.rigidbody.velocity.x, hit.rigidbody.velocity.y, 0) * Time.deltaTime * 0.5f;
                        transform.position += new Vector3(-hit.rigidbody.angularVelocity * Vector2.Distance(hit.point, hit.collider.transform.position) * Mathf.Cos(Mathf.Deg2Rad * Vector2.SignedAngle(hit.point, hit.collider.transform.position)) * Time.deltaTime, hit.rigidbody.angularVelocity * Vector2.Distance(hit.point, hit.collider.transform.position) * Mathf.Cos(Mathf.Deg2Rad * Vector2.SignedAngle(hit.point, hit.collider.transform.position)) * Time.deltaTime, 0) * Time.deltaTime * 0.5f;
                    }
                    Debug.DrawRay(hit.point, hit.normal);
                }
                
            }

            if(groundPointsNum > 0)
            {
                if(Velocity.y <= 1) {
                    transform.position = new Vector2(transform.position.x, avgPos + (transform.position.y - coll.bounds.center.y) + coll.bounds.extents.y + 0.15f);

                    if (Grounded == false)
                    {
                        OnLand();
                        rb.velocity = new Vector3(rb.velocity.x, 0, 0);
                    }

                    Grounded = true;
                    UngroundedTimer = 0;

                    Debug.Log("AvgAngle before: " + avgAngle.ToString());

                    avgAngle /= groundPointsNum;

                    Debug.Log("AvgAngle after: " + avgAngle.ToString());
                    Velocity.y = 0;
                    //Velocity.y = avgAngle.x * Velocity.x;
                    //Velocity.x = avgAngle.y * Velocity.x;
                }
            } else
            {
                Grounded = false;
                /*UngroundedTimer += Time.deltaTime;
                if (UngroundedTimer >= 0.0f)
                {
                    Grounded = false;
                }

                if (Velocity.y <= 0) //If falling, look for collision as to land on it perfectly
                {

                    RaycastHit2D hit;
                    hit = Physics2D.Raycast(new Vector2(transform.position.x, coll.bounds.center.y - coll.bounds.extents.y), Vector2.down, 0.4f, EnvironmentMask);

                    if (hit.collider != null) //Check for a collision
                    {
                        transform.position = hit.point + new Vector2(0, coll.bounds.extents.y - coll.offset.y - 0.1f); //Move to collision point
                        Debug.Log("ground1");

                        if (Velocity.y < 0)
                        {
                            OnLand();
                        }
                        Velocity.y = 0;
                        Grounded = true;
                        UngroundedTimer = 0;
                    }
                }*/
            }
        } 
        

        if (rb.IsAwake())
        {
            GroundPoints.Clear();
            WallPoints.Clear();
            CeilingPoints.Clear();
        }

    }
    public virtual void Update()
    {
        if (HitStun > 0)
        {
            HitStun = Mathf.MoveTowards(HitStun, 0, Time.deltaTime * TimeScale * 60);
            return;
        }

        if(Staggered)
        {
            Stagger += StaggerRecoverySpd * Time.deltaTime * MaxStagger * (Grounded ? 1 : 0.6f) / 15;
            StaggerRecoverySpd += 2 * Time.deltaTime;
            if(Stagger >= MaxStagger)
            {
                Staggered = false;
                Stagger = MaxStagger;
            } else
            {
                HurtState = 30;
            }
        }

        if(!Grounded)
        {
            if (GravityCancelTime == 0)
            {
                Velocity.y = Mathf.MoveTowards(Velocity.y, Bounce ? -100 : maxFallSpeed, Gravity * Time.deltaTime * TimeScale);
            }
            else
            {
                GravityCancelTime = Mathf.MoveTowards(GravityCancelTime, 0, Time.deltaTime * TimeScale);
            }
        }

        rb.velocity = new Vector3(Velocity.x, Velocity.y, 0) * TimeScale;
    }

    public virtual void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            Underwater = true;
        }
    }

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("Water"))
        {
            Debug.Log("Test1");
            Instantiate(SplashFX, other.ClosestPoint(transform.position), Quaternion.identity);
        }
    }

    public virtual void OnTriggerExit2D(Collider2D other)
    {
        
        if (other.CompareTag("Water"))
        {
            Debug.Log("Test2");
            Underwater = false;
            Instantiate(SplashFX, other.ClosestPoint(transform.position), Quaternion.identity);
        }
    }

    public virtual void HitResponse(GameObject attacker, GameObject Defender) { }

    public virtual void HurtResponse(float damage = 0, float knockbackx = 0, float knockbacky = 0) { }

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

[System.Serializable]
public struct AffinityData
{
    public enum Affinity
    {
        Normal,
        Weak,
        Strong,
        Immune
    }

    public Affinity Slash;
    public Affinity Bash;
    public Affinity Pierce;

    public Affinity Solar;
    public Affinity Crystal;
    public Affinity Sonic;

    public enum ElementalClassTypes
    {
        None,
        Energy,
        Earthen,
        Mechanical,
    }

    public ElementalClassTypes ElementalClass;

}
