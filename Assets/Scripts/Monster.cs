using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    public List<Sprite> monsters; //Sprites (TBD)
    public string combo; 
    private int currentComboIndex = 0; 
    public int[] comboScores = new int[] { 50, 100, 250, 500, 1500, 130, 210, 340 };
    public int currentscore = 0;

    private MonsterSpawning monsterSpawning;

    private bool isLockedOn;
    private bool scoreAdd;

    //UI Element
    public GameObject keyUICircle;
    public Transform KeyTransform;

    void Start()
    {
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
        foreach (Transform child in KeyTransform)
        {
            Destroy(child.gameObject);
        }

        Vector3 monsterPosition = transform.position;
        KeyTransform.position = monsterPosition;

        int length = combo.Length;
        float spacing = 75f;
        float curveStrength = 0.005f;

        for (int i = 0; i < length; i++)
        {
            GameObject keyUI = Instantiate(keyUICircle, KeyTransform);

            TextMeshProUGUI keyUICombo = keyUI.GetComponentInChildren<TextMeshProUGUI>();
            Image keyUIImage = keyUI.GetComponentInChildren<Image>();

            keyUICombo.text = combo[i].ToString();

            RectTransform rect = keyUI.GetComponent<RectTransform>();

            float xPos = (i - (length - 1) / 2f) * spacing;
            float yPos = curveStrength * xPos * xPos + 20;

            rect.anchoredPosition = new Vector2(xPos, yPos);

        }
    }

    private void UpdateComboUI()
    {
        ArrangeCombo(combo);

        if (isLockedOn)
        {
            //pulls up the layer mask of current monster prefab. 
        }
        else
        {
            //returns it normal
        }
    }

    void OnPlayerInput(int index)
    {
        Image buttonImage = KeyTransform.GetChild(index).GetComponentInChildren<Image>();
        buttonImage.color = new Color(0.5f, 0.5f, 0.5f, 1f); // ¾îµÓ°Ô º¯°æ
    }


    public void provideScore()
    {
        currentscore = GetTotalScore();
        GameManager.Instance.score = GameManager.Instance.score + currentscore;
    }


}
