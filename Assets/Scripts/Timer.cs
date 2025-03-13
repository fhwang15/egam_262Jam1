using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{

    public EntryEnter entryEnter;

    public float timeRemaining = 180f;
    public float inMin;
    public TextMeshProUGUI timerText;

    public GameObject Replay;

    void Start()
    {
        Replay.SetActive(false);
        StartCoroutine(StartTimer());
    }

    IEnumerator StartTimer()
    {
        while (timeRemaining > 0)
        {
            inMin = timeRemaining / 60;
            timerText.text = $"Time: {inMin:F2}"; 
            yield return new WaitForSeconds(1f);
            timeRemaining--;
        }

        timerText.text = "Time's Up!";


        Time.timeScale = 0f;

        Replay.SetActive(true);

        entryEnter.ShowEntryUI();

    }
}
