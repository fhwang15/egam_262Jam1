using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    //public List<Sprite> monsters; //Sprites (TBD)
    public string combo; 
    private int currentComboIndex = 0; 
    public int[] comboScores = new int[] { 50, 100, 250, 500, 1500, 130, 210, 340 };
    public int currentscore = 0;

    private MonsterSpawning monsterSpawning;

    public TextMeshProUGUI comboIndicator;

    private bool isLockedOn;
    private bool scoreAdd;

    //UI Element
    public GameObject keyUICircle;
    public Transform KeyTransform;

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
        isLockedOn = state; //Locked on.
        UpdateComboUI();
    }
    public void SetCombo(string newCombo)
    {
        combo = newCombo;
        currentComboIndex = 1;
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
            Debug.Log($"Score Added: {comboScores[i]}");
        }
        scoreAdd = true;

        return totalScore;
    }


    void ArrangeCombo(string combo)
    {
        int length = combo.Length;
        float radius = 50f;

        for (int i = 0; i < length; i++)
        {
            GameObject keyUI = Instantiate(keyUICircle, KeyTransform);
            keyUI.GetComponentInChildren<TMP_Text>.text = combo[i].ToString();

            RectTransform rect = keyUI.GetComponent<RectTransform>();
            if (length == 2)
            {
                rect.anchoredPosition = new Vector2(i * 100, 0);
            }
            else if (length == 3)
            {
                Vector2[] positions = { new Vector2(-50, -50), new Vector2(50, -50), new Vector2(0, 50) };
                rect.anchoredPosition = positions[i];
            }
            else if(length == 4)
            {
                float angle = i * (360f / length);
                rect.anchoredPosition = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad) * radius, Mathf.Sin(angle * Mathf.Deg2Rad) *radius);
            }

        }
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
