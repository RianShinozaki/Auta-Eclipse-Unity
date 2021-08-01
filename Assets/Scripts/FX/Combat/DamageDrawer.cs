using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageDrawer : MonoBehaviour
{
    public Color normalDamageColor;
    public Color criticalDamageColor;
    public TextMeshPro Text;
    public float Damage = 0;

    float initY;
    float ySpeed;
    float timer;

    ObjectPoolObject opo;

    // Start is called before the first frame update
    void Start()
    {
        initY = transform.position.y;
        timer = 0;
        opo = GetComponent<ObjectPoolObject>();
        Text = GetComponent<TextMeshPro>();
    }

    private void OnEnable()
    {
        initY = transform.position.y;
        Damage = 0;
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {

        if (transform.position.y > initY - 0.1f)
        {
            ySpeed -= Time.deltaTime * 10;
            transform.position += Vector3.up * ySpeed * Time.deltaTime;
        }
        else
        {
            ySpeed = 0;
            timer += Time.deltaTime;

            if(timer > 2)
            {
                opo.RePool();
            }
        }
    }

    public void NormalHit(Vector3 position, float value)
    {
        Text.color = normalDamageColor;
        transform.position = position;
        initY = position.y;
        timer = 0;
        Damage += value;
        Text.text = Damage.ToString();
        ySpeed = 5;
    }
    public void CriticalHit(Vector3 position, float value)
    {
        Text.color = criticalDamageColor;
        transform.position = position;
        initY = position.y;
        timer = 0;
        Damage += value;
        Text.text = Damage.ToString();
        ySpeed = 5;
    }
}
