using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidBeing : MonoBehaviour
{
    public Vector2 PatrolRadius;
    public Vector2 MoveTimeRange;
    public Vector2 MoveTo;
    Vector3 InitPos;
    float MoveTime = 0;
    float time = 0;
    bool Activated = false;
    public GameObject spawn;
    public FightBarrier Barrier;

    void Start()
    {
        MoveTime = Random.Range(MoveTimeRange.x, MoveTimeRange.y);
        InitPos = transform.position;
        MoveTo = new Vector2(transform.position.x, transform.position.y);
    }
    void Update()
    {
        time += Time.deltaTime;
        if(time > MoveTime)
        {
            MoveTo = new Vector2(InitPos.x + Random.Range(-PatrolRadius.x, PatrolRadius.x), InitPos.y + Random.Range(-PatrolRadius.y, PatrolRadius.y));
            time = 0;
            MoveTime = Random.Range(MoveTimeRange.x, MoveTimeRange.y);
        }

        transform.position += new Vector3(MoveTo.x - transform.position.x, MoveTo.y - transform.position.y, 0) * Time.deltaTime;

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.transform.parent.GetComponent<PlayerController>();
        if (player != null)
        {
            if(!Activated)
            {
                Activated = true;
                
                AutaUIController.Instance.ActivateScreenFX(0);

                player.Velocity.y = 3;
                player.Velocity.x = 5 * (transform.position.x > player.transform.position.x ? 1 : -1);

                Barrier.AddEnemy(Instantiate(spawn, transform.position, Quaternion.identity));
                Barrier.ActivateBarrier();
                

                Destroy(gameObject);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(PatrolRadius.x, PatrolRadius.y, 0) * 2);
    }
}
