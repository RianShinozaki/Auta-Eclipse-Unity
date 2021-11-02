using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightBarrier : MonoBehaviour
{
    public List<GameObject> Barriers = new List<GameObject>();
    public List<GameObject> Enemies = new List<GameObject>();

    public void AddEnemy(GameObject enemy)
    {
        Enemies.Add(enemy);
    }
    public void ActivateBarrier()
    {
        foreach (GameObject barrier in Barriers)
        {
            barrier.SetActive(true);
            barrier.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        }
        StartCoroutine(ActivateBarrierEnum());
    }

    public void DeactivateBarrier()
    {
        foreach (GameObject barrier in Barriers)
        {
            barrier.SetActive(false);
        }
    }

    public void Update()
    {
        foreach(GameObject Enemy in Enemies)
        {
            if(Enemy == null)
            {
                Enemies.Remove(Enemy);
            }
        }
        if(Enemies.Count == 0)
        {
            DeactivateBarrier();
        }
    }

    IEnumerator ActivateBarrierEnum()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime;
            foreach (GameObject barrier in Barriers)
            {
                barrier.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, t);
                yield return null;
            }
        }
    }
}
