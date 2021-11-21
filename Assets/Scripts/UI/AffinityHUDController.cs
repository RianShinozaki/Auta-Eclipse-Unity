using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

public class AffinityHUDController : MonoBehaviour
{
    PhysicsEntity Entity;
    AffinityData Data;

    public SpriteRenderer Frame;
    public SpriteRenderer[] AffinityButtons;

    public TextMeshPro Name;
    public TextMeshPro Level;

    public Sprite Normal;
    public Sprite Strong;
    public Sprite Immune;
    public Sprite Weak;

    public float initY;

    public float alph;
    public float toalph;

    void Start()
    {
        Entity = GetComponentInParent<PhysicsEntity>();
        Data = Entity.Affinities;

        #region pain
        switch (Data.Slash)
        {
            case AffinityData.Affinity.Normal:
                AffinityButtons[0].sprite = Normal;
                break;
            case AffinityData.Affinity.Weak:
                AffinityButtons[0].sprite = Weak;
                break;
            case AffinityData.Affinity.Immune:
                AffinityButtons[0].sprite = Immune;
                break;
            case AffinityData.Affinity.Strong:
                AffinityButtons[0].sprite = Strong;
                break;
        }

        switch (Data.Bash)
        {
            case AffinityData.Affinity.Normal:
                AffinityButtons[1].sprite = Normal;
                break;
            case AffinityData.Affinity.Weak:
                AffinityButtons[1].sprite = Weak;
                break;
            case AffinityData.Affinity.Immune:
                AffinityButtons[1].sprite = Immune;
                break;
            case AffinityData.Affinity.Strong:
                AffinityButtons[1].sprite = Strong;
                break;
        }

        switch (Data.Pierce)
        {
            case AffinityData.Affinity.Normal:
                AffinityButtons[2].sprite = Normal;
                break;
            case AffinityData.Affinity.Weak:
                AffinityButtons[2].sprite = Weak;
                break;
            case AffinityData.Affinity.Immune:
                AffinityButtons[2].sprite = Immune;
                break;
            case AffinityData.Affinity.Strong:
                AffinityButtons[2].sprite = Strong;
                break;
        }

        switch (Data.Crystal)
        {
            case AffinityData.Affinity.Normal:
                AffinityButtons[3].sprite = Normal;
                break;
            case AffinityData.Affinity.Weak:
                AffinityButtons[3].sprite = Weak;
                break;
            case AffinityData.Affinity.Immune:
                AffinityButtons[3].sprite = Immune;
                break;
            case AffinityData.Affinity.Strong:
                AffinityButtons[3].sprite = Strong;
                break;
        }

        switch (Data.Solar)
        {
            case AffinityData.Affinity.Normal:
                AffinityButtons[4].sprite = Normal;
                break;
            case AffinityData.Affinity.Weak:
                AffinityButtons[4].sprite = Weak;
                break;
            case AffinityData.Affinity.Immune:
                AffinityButtons[4].sprite = Immune;
                break;
            case AffinityData.Affinity.Strong:
                AffinityButtons[4].sprite = Strong;
                break;
        }

        switch (Data.Sonic)
        {
            case AffinityData.Affinity.Normal:
                AffinityButtons[5].sprite = Normal;
                break;
            case AffinityData.Affinity.Weak:
                AffinityButtons[5].sprite = Weak;
                break;
            case AffinityData.Affinity.Immune:
                AffinityButtons[5].sprite = Immune;
                break;
            case AffinityData.Affinity.Strong:
                AffinityButtons[5].sprite = Strong;
                break;
        }
        #endregion

        Name.text = Entity.Name;
        Level.text = string.Concat("LV: ", (Entity.Level));

        GameManager.SetAnalysisState += OnAnalysisState;

        Frame.gameObject.SetActive(false);

        initY = transform.localPosition.y;

    }
    public void OnDestroy()
    {
        GameManager.SetAnalysisState -= OnAnalysisState;
    }

    public void OnAnalysisState(bool mode)
    {
        Frame.gameObject.SetActive(mode);
        StartCoroutine(HUDBounce());
        alph = 0;
    }

    public IEnumerator HUDBounce() 
    {
        transform.localPosition += new Vector3(0, -0.5f, 0);
        while(transform.localPosition.y > initY)
        {
            transform.localPosition += new Vector3(0, initY - transform.localPosition.y, 0) *0.05f;
            yield return null;
        }

        transform.localPosition = new Vector3(transform.localPosition.x, initY, transform.localPosition.z);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.root.GetComponent<PlayerController>())
        {
            toalph = 0.5f;
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.root.GetComponent<PlayerController>())
        {
            toalph = 1;
        }
    }

    void Update()
    {
        transform.localScale = new Vector3(Entity.transform.localScale.x, 1, 1);
        transform.localPosition = new Vector3(Mathf.Abs(transform.localPosition.x) * Mathf.Sign(Entity.transform.localScale.x), transform.localPosition.y, transform.localPosition.z);

        if(alph != toalph)
        {
            alph = Mathf.MoveTowards(alph, toalph, 0.1f);

            Frame.color = new Color(Frame.color.r, Frame.color.g, Frame.color.b, alph);
            foreach(SpriteRenderer button in AffinityButtons)
            {
                button.color = new Color(button.color.r, button.color.g, button.color.b, alph);
            }

            Name.color = new Color(Name.color.r, Name.color.g, Name.color.b, alph);
            Level.color = new Color(Level.color.r, Level.color.g, Level.color.b, alph);
        }
    }
}
