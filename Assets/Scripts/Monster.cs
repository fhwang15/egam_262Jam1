using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class Monster : MonoBehaviour
{
    public List<Sprite> monsters;
    public SpriteRenderer sprite;

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
    public Canvas monsterCanvas;
    private int originalSortingOrder;

    public GameObject floatingScorePrefab;


    void Start()
    {
        if(monsterSpawning == null)
        {
            monsterSpawning = MonsterSpawning.Instance;
        }

        sprite = GetComponentInChildren<SpriteRenderer>();
        sprite.sprite = monsters[Random.Range(0, monsters.Count)];
        monsterCanvas = GetComponentInChildren<Canvas>();
        originalSortingOrder = monsterCanvas.sortingOrder;


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

        if (state)
        {
            GameManager.Instance.lockOnBackground.SetActive(true);
            monsterCanvas.sortingOrder = 200;
            sprite.sortingOrder = 200;
            GameManager.Instance.lockOnBackground.GetComponentInParent<Canvas>().sortingOrder = 150;
        }
        else
        {
            GameManager.Instance.lockOnBackground.SetActive(false);
            monsterCanvas.sortingOrder = originalSortingOrder;
            sprite.sortingOrder = originalSortingOrder;
        }
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
            Debug.Log(score);
            OnPlayerInput(currentComboIndex);
            currentComboIndex++;

            if (IsComboComplete())
            {
                ShowFinalFloatingText();
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


    void ArrangeCombo(string combo)
    {

        for (int i = KeyTransform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(KeyTransform.GetChild(i).gameObject); //Frame issue so needed to use this
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
    }

    void OnPlayerInput(int index)
    {
        if (index < KeyTransform.childCount)
        {

            Transform keyChild = KeyTransform.GetChild(index);
            Image buttonImage = keyChild.Find("Image").GetComponent<Image>();
            buttonImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            ShowFloatingText(index);
        }
    }

    void ShowFloatingText(int index)
    {
        Vector3 spawnPosition = KeyTransform.GetChild(index).position; // 누른 키 위치
        GameObject floatText = Instantiate(floatingScorePrefab, spawnPosition, Quaternion.identity, monsterCanvas.transform); 

        int score = comboScores[index];
        string scoreText = score.ToString();

        FloatingScore floatScore = floatText.GetComponent<FloatingScore>();
        floatScore.GetText(scoreText, Color.yellow);
    }

    void ShowFinalFloatingText()
    {
        int totalScore = GetTotalScore();

        Vector3 spawnPosition = transform.position;
        GameObject floatText = Instantiate(floatingScorePrefab, spawnPosition, Quaternion.identity);
       
        StartCoroutine(HandleScore(totalScore));

        FloatingScore floatingScore = floatText.GetComponentInChildren<FloatingScore>();
        floatingScore.GetText("+" + totalScore, Color.green);


    }

    IEnumerator HandleScore(int totalScore)
    {
        yield return new WaitForSeconds(1.5f);

        GameManager.Instance.score += totalScore;
        monsterSpawning.RemoveMonster(this); 
    }

    public void provideScore()
    {
        currentscore = GetTotalScore();
        GameManager.Instance.score = GameManager.Instance.score + currentscore;
    }


}
