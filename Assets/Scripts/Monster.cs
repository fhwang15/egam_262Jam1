using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.SocialPlatforms.Impl;

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

    public Image circleTimer;
    public float timerDuration;
    private float timeRemaining;

    private CircleController magicCircle;


    void Start()
    {
        if(monsterSpawning == null)
        {
            monsterSpawning = MonsterSpawning.Instance;
        }

        magicCircle = GetComponent<CircleController>();

        sprite = GetComponentInChildren<SpriteRenderer>();
        sprite.sprite = monsters[Random.Range(0, monsters.Count)];
        monsterCanvas = GetComponentInChildren<Canvas>();
        originalSortingOrder = monsterCanvas.sortingOrder;

        timerDuration = monsterSpawning.currentMonsterTime;
        timeRemaining = timerDuration;
        circleTimer.fillAmount = 1f;

        isLockedOn = false;

        UpdateComboUI();
    }

    private void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            float fillAmount = timeRemaining / timerDuration;

            if (circleTimer != null) // 안전성 검사 추가
            {
                // 바깥에서 안쪽으로 줄어드는 효과
                circleTimer.fillAmount = fillAmount;

                // 오수! 스타일: 입력할수록 원이 안쪽으로 줄어듬
                float scaleFactor = Mathf.Lerp(1.2f, 1f, fillAmount);
                circleTimer.rectTransform.localScale = new Vector3(scaleFactor, scaleFactor, 1);
            }
        }
        else
        {
            if (circleTimer != null) // 안전성 검사 추가
            {
                Destroy(circleTimer.gameObject);
            }
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
            int finalScore = score * GameManager.Instance.GetScoreMultiplier();
            Debug.Log("ProcessInput called. Base score: " + score + ", Multiplied score: " + finalScore);

            OnPlayerInput(currentComboIndex);
            currentComboIndex++;

            if (IsComboComplete())
            {
                ShowFinalFloatingText(finalScore);
            }

            return finalScore;
        }
        else
        {
            Debug.Log("Wrong input or combo not complete. Returning 0.");
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
            magicCircle.OnComboInput();

            // 오수! 스타일: 입력 시 순간적으로 커졌다 작아지는 효과
            StartCoroutine(ButtonPulseEffect(buttonImage));

            buttonImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            ShowFloatingText(index);

            GameManager.Instance.AddCombo();
        }
    }

    void ShowFloatingText(int index)
    {
        int score = comboScores[index];
        Vector3 spawnPosition = KeyTransform.GetChild(index).position;

        // GameManager를 통해 FloatingScore 생성
        GameManager.Instance.ShowFloatingScore(spawnPosition, score, Color.yellow);
    }


    void ShowFinalFloatingText(int finalScore)
    {
        Debug.Log("ShowFinalFloatingText called. Final score: " + finalScore);

        int totalScore = GetTotalScore();
        int calculatedScore = totalScore * GameManager.Instance.GetScoreMultiplier();
        Debug.Log("Calculated Final Score: " + calculatedScore);

        // 점수 표시
        GameManager.Instance.ShowFloatingScore(transform.position, calculatedScore, Color.green);

        // 몬스터 처치 시 피버 게이지 증가
        GameManager.Instance.AddFeverGauge(10);

        // UI와 마법진 제거
        ClearMonsterUI();

        // 코루틴 호출을 GameManager를 통해 수행
        Debug.Log("Calling HandleScore Coroutine via GameManager.");
        GameManager.Instance.StartCoroutine(GameManager.Instance.HandleScore(calculatedScore));
    }

    void ClearMonsterUI()
    {
        foreach (Transform child in KeyTransform)
        {
            Destroy(child.gameObject);
        }

        if (magicCircle != null)
        {
            magicCircle.gameObject.SetActive(false);
        }

        sprite.enabled = false;
    }

    IEnumerator HandleScore(int totalScore)
    {
        Debug.Log("HandleScore called. Total score to add: " + totalScore);
        yield return new WaitForSeconds(0.5f);

        int finalScore = totalScore * GameManager.Instance.GetScoreMultiplier();
        GameManager.Instance.AddScore(finalScore); // 누적 점수 추가
        Debug.Log("Final Score Added: " + finalScore);

        yield return new WaitForSeconds(1.5f);

        monsterSpawning.RemoveMonster(this);
    }

    IEnumerator HideCanvasWithDelay()
    {
        yield return new WaitForSeconds(0.5f);
        monsterCanvas.enabled = false;
    }


    IEnumerator ButtonPulseEffect(Image buttonImage)
    {
        float duration = 0.1f;
        float elapsed = 0f;

        Vector3 originalScale = buttonImage.transform.localScale;
        Vector3 enlargedScale = originalScale * 1.2f;

        while (elapsed < duration)
        {
            buttonImage.transform.localScale = Vector3.Lerp(originalScale, enlargedScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        buttonImage.transform.localScale = originalScale;
    }

}
