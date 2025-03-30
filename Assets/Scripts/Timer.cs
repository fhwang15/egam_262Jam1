using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{

    public EntryEnter entryEnter;

    public float timeRemaining = 180f;
    public float inMin;
    public float elapsedTime;
    public TextMeshProUGUI timerText;

    //Replay Button
    public GameObject Replay;

    void Start()
    {
        Replay.SetActive(false);

    }

    void Update()
    {

        if (timeRemaining >= 0)
        {

            timeRemaining -= Time.deltaTime;
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        else
        {
            Time.timeScale = 0f;

            Replay.SetActive(true);
            entryEnter.ShowEntryUI();

        }
    }

}
