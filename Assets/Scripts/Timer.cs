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
    private bool isRunning = true; 

    void Start()
    {
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
        isRunning = false;

        Time.timeScale = 0f;

        entryEnter.ShowEntryUI();

    }
}
