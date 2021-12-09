using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public GameObject Enemy;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y) + Vector2.up * 2, Vector2.down, 2);
        if (hit.collider != null)
        {
            transform.position = hit.point;
        }

    }
    public void Spawn()
    {
        Instantiate(Enemy, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
