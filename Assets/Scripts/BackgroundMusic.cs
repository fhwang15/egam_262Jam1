using UnityEngine;
using System.Collections;

public class BackgroundMusic : MonoBehaviour
{
    public AudioSource bgm;

    static bool isPlaying = false;

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(bgm);
        if (!isPlaying)
        {
            bgm.Play();
            isPlaying = true;
        }
    }


}
