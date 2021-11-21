using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public delegate void AnalysisState(bool mode);
    public static event AnalysisState SetAnalysisState;
    public static bool AnalysisActive = false;
    public static bool CanSwapAnalysis = true;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void OnDestroy()
    {
    }

    public void OnAnalysisState(bool mode)
    {
        SetAnalysisState?.Invoke(mode);
        StartCoroutine(AnalysisTransition(mode));
        AnalysisActive = mode;
    }

    public IEnumerator AnalysisTransition(bool mode)
    {
        CanSwapAnalysis = false;
        if(mode)
        {
            while(Time.timeScale > 0.01f && mode == true)
            {
                Time.timeScale += (0 - Time.timeScale) * 0.2f;
                yield return null;
            }
            Time.timeScale = 0;

        } else
        {
            while (Time.timeScale < 0.99f && mode == false)
            {
                Time.timeScale += (1 - Time.timeScale) * 0.2f;
                yield return null;
            }
            Time.timeScale = 1;
        }
        CanSwapAnalysis = true;
    }
}