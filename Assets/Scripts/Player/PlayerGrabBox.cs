using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrabBox : MonoBehaviour
{
    public PlayerController player;
    // Start is called before the first frame update
    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("grab1");
        BaseEnemy enemy = collision.transform.parent.GetComponent<BaseEnemy>();
        if (enemy != null)
        {
            Debug.Log("grab2");
            if (enemy.stunned)
            {
                Debug.Log("grab3");
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
