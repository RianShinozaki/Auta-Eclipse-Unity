using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK.Wwise;

public class MusicPlayer : MonoBehaviour
{
    //public static Queue<AK.Wwise.Event> MusicQueue;
    public AK.Wwise.Event OnStart;
    public AK.Wwise.Event OnEnd;
    //public static uint CurrentMusicInstance;
    //public static AK.Wwise.Event MusictoLoop;

    public AkGameObj akObj;

    // Start is called before the first frame update
    void Start()
    {
        OnStart.Post(gameObject);
    }

    private void OnDestroy()
    {
        OnEnd.Post(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
