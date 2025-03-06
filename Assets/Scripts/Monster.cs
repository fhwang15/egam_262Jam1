using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using TMPro;

public class Monster : MonoBehaviour
{
    public List<Sprite> monsters; //Sprites (TBD)
    public string combo; 
    private int currentComboIndex = 0; 
    public int[] comboScores = new int[] { 10, 20, 30, 50, 80, 130, 210, 340 };
    public int currentscore = 0;

    private MonsterSpawning monsterSpawning;

    public TextMeshProUGUI comboIndicator;

    private bool isLockedOn;
    private bool scoreAdd;

    void Start()
    {

        if (comboIndicator == null)
        {
            comboIndicator = GetComponentInChildren<TextMeshProUGUI>();
        }

        if(monsterSpawning == null)
        {
            monsterSpawning = MonsterSpawning.Instance;
        }

        isLockedOn = false;
        scoreAdd = false;

        UpdateComboUI();
    }

    private void Update()
    {
       if (scoreAdd)
        {
            provideScore();
            scoreAdd = false;
        }
        else
        {
            return;
        }
    }

    public void SetLockOn(bool state)
    {
        isLockedOn = state;
        UpdateComboUI();
    }
    public void SetCombo(string newCombo)
    {
        combo = newCombo;
        currentComboIndex = 0;
        UpdateComboUI();
    }

    public char GetFirstLetter()
    {
        return combo[0];
    }

    public int ProcessInput(char input)
    {
        if (currentComboIndex < combo.Length && input == combo[currentComboIndex])
        {
            int score = comboScores[currentComboIndex]; 
            currentComboIndex++;

            if (IsComboComplete())
            {
                monsterSpawning.RemoveMonster(this);
            }

            return score;
        }
        else
        {
            monsterSpawning.RemoveMonster(this); 
            return 0;
        }
    }

    public bool IsComboComplete()
    {
        return currentComboIndex >= combo.Length;
    }

    public int GetTotalScore()
    {
        int totalScore = 0;
        for (int i = 0; i < currentComboIndex; i++)
        {
            totalScore += comboScores[i];
        }
        scoreAdd = true;

        return totalScore;
    }


    private void UpdateComboUI()
    {
        if (comboIndicator != null)
        {
            comboIndicator.text = combo;

            if (isLockedOn)
            {
                comboIndicator.color = Color.red;
            }
            else
            {
                comboIndicator.color = Color.white;
            }


        }
    }


    public void HideComboUI()
    {
        if (comboIndicator != null)
        {
            comboIndicator.text = "";
        }
    }


    public void provideScore()
    {
        currentscore = GetTotalScore();
        GameManager.Instance.score = GameManager.Instance.score + currentscore;
    }


}
