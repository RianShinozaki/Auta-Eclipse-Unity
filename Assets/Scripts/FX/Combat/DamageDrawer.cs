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
    public GameObject CritImg;

    float initY;
    float ySpeed;
    float timer;
    float baseSize;

    ObjectPoolObject opo;

    // Start is called before the first frame update
    void Start()
    {
        initY = transform.position.y;
        timer = 0;
        opo = GetComponent<ObjectPoolObject>();
        baseSize = Text.fontSize;
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

            if(timer > 1)
            {
                opo.RePool();
            }
        }

        if(CritImg.activeInHierarchy)
        {
            Text.transform.localPosition = new Vector3(Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f), 1) + new Vector3(0, 0.2f, 0);
            CritImg.transform.localPosition = new Vector3(Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f), 1) + new Vector3(-0.03f, 1.15f, 0);
        } else
        {
            Text.transform.localPosition = new Vector3(0, 0.2f, 0);
            CritImg.transform.localPosition = new Vector3(0, 0, 0) + new Vector3(-0.03f, 1.15f, 0);
        }
    }

    public void Hit(Vector3 position, float value, bool Critical, bool Weakness)
    {
        Text.color = Weakness ? criticalDamageColor : normalDamageColor;
        transform.position = position;
        initY = position.y;
        timer = 0;
        Damage += value;
        Text.text = (Mathf.FloorToInt(Damage)).ToString();
        ySpeed = 5;
        CritImg.SetActive(Critical);
        Text.fontSize = 0.5f * (Critical ? 1.5f : 1);
    }
}
