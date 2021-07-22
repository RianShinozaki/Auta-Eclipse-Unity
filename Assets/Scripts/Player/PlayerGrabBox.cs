using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrabBox : MonoBehaviour
{
    public PlayerController player;
    // Start is called before the first frame update
    public void OnTriggerEnter2D(Collider2D collision)
    {
        BaseEnemy enemy = collision.transform.root.GetComponent<BaseEnemy>();
        if (enemy != null)
        {
            if (enemy.stunned)
            {
                player.EnemyGrabbed(enemy);
            }
        }
    }

    public void Update()
    {
        if(player.stateMachine.CurrentState != player.State_GrabEnemy)
        {
            gameObject.SetActive(false);
        }
    }
}
