using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsEntity : MonoBehaviour
{

    #region Physics System

    public Vector2 Velocity;
    public float Gravity;
    public bool Grounded;

    public Rigidbody2D rb;
    public Collider2D coll;
    public LayerMask EnvironmentMask;

    #endregion Physics

    public List<ContactPoint2D> GroundPoints = new List<ContactPoint2D>();
    public List<ContactPoint2D> WallPoints = new List<ContactPoint2D>();
    public virtual void OnCollisionStay2D(Collision2D collision)
    {
        foreach(ContactPoint2D contact in collision.contacts)
        {
            float angle = Vector2.SignedAngle(Vector2.up, contact.normal);
            Debug.Log(angle);
            if(angle >= -45 && angle <= 45)
            {
                GroundPoints.Add(contact);
            } else
            {
                WallPoints.Add(contact);
            }
            
        }
    }

    public virtual void Awake()
    {

    }


    // Update is called once per frame
    public virtual void Update()
    {
        Vector3 vel = new Vector3(0, 0, 0);

        int wallPointsNum = 0;
        float avgAngle = 0;

        foreach (ContactPoint2D contact in WallPoints)
        {
            Debug.Log(Vector2.SignedAngle(Vector2.up, contact.normal));

            wallPointsNum++;

            avgAngle += Vector2.SignedAngle(Vector2.up, contact.normal);
        }
        if(wallPointsNum > 0)
        {
            avgAngle /= wallPointsNum;
            if(avgAngle > 0)
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

        int groundPointsNum = 0;
        avgAngle = 0;


        foreach (ContactPoint2D contact in GroundPoints)
        {
            Debug.Log(Vector2.SignedAngle(Vector2.up, contact.normal));

            groundPointsNum++;

            avgAngle += Vector2.SignedAngle(Vector2.up, contact.normal);
        }

        if(groundPointsNum == 0 )
        {
            
            Velocity.y -= Gravity * Time.deltaTime;
            vel.x = Velocity.x;

            if (Velocity.y < 0) //If falling, look for collision as to land on it perfectly
            {
                Grounded = false;
                

                RaycastHit2D hit;
                hit = Physics2D.Raycast(new Vector2(transform.position.x, coll.bounds.center.y - coll.bounds.extents.y), Vector2.down, Velocity.y * Time.deltaTime, EnvironmentMask);
                Debug.DrawRay(new Vector2(transform.position.x, coll.bounds.center.y - coll.bounds.extents.y), Vector2.down * Velocity.y * Time.deltaTime);

                if (hit.collider != null) //Check for a collision
                {
                    transform.position = hit.point + new Vector2(0, coll.bounds.extents.y); //Move to collision point
                    transform.position -= new Vector3(0, Velocity.y * Time.deltaTime, 0);
                    Velocity.y = 0;
                }
            }
        } else
        {
            Grounded = true;
            avgAngle /= groundPointsNum;
            Velocity.y = Mathf.Sin(avgAngle * Mathf.Deg2Rad) * Velocity.x;
            vel.x = Mathf.Cos(avgAngle * Mathf.Deg2Rad) * Velocity.x;
        }

        

        vel.y = Velocity.y;
        transform.position += vel * Time.deltaTime;

        if (rb.IsAwake())
        {
            GroundPoints.Clear();
            WallPoints.Clear();
        }

        Debug.Log("Hooray");
    }
    public virtual void FixedUpdate()
    {
        
    }
}
