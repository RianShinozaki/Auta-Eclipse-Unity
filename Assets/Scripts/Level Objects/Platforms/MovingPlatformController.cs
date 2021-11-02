using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformController : MonoBehaviour
{
    public float t = 0;
    public Vector2 moveVec;
    public Vector3 initPos;
    public float speed;
    public float dir = 1;
    // Start is called before the first frame update
    void Start()
    {
        initPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        t += speed * Time.deltaTime * dir;
        if(t > 1 || t < 0)
        {
            dir*=-1;
        }
        transform.position = Vector3.Lerp(initPos, initPos + new Vector3(moveVec.x, moveVec.y, 0), t);
    }

    private void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, transform.position + new Vector3(moveVec.x, moveVec.y, 0));
        Gizmos.DrawWireSphere(Vector3.Lerp(transform.position, transform.position + new Vector3(moveVec.x, moveVec.y, 0), t), 0.3f);
    }
}
