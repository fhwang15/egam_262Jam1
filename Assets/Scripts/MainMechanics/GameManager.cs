using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{

    //Player Input and Overall Scoring
    public static GameManager Instance;

    private MonsterSpawning monsterSpawning;
    private Monster lockedOnMonster = null;
    
    public int score = 0;

    public TextMeshProUGUI totalScore;

    //UI/Visual Feedback
    public GameObject lockOnBackground;


    private bool isFeverTime = false;
    private float feverGauge = 0f;
    private float maxFeverGauge = 100f;
    private float feverDuration = 10f;
    private float feverTimer = 0f;
    private int feverMultiplier = 2;
    private int feverComboThreshold = 10; 

    public Image feverGaugeImage;
    public GameObject floatingScorePrefab;
    public Canvas worldCanvas; // 월드 공간에 두는 캔버스

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        monsterSpawning = GetComponent<MonsterSpawning>();
        lockOnBackground.SetActive(false);
        
    }

    void Update()
    {

        //Player Input
        if (Input.anyKeyDown)
        {
            string input = Input.inputString.ToUpper(); //Change every key into Uppercase
            if (string.IsNullOrEmpty(input)) return; //if input is empty, just return.

            char pressedKey = input[0]; //Get the first letter

            if (lockedOnMonster == null)
            {
                //if there's no locked on monster, try locking it on.
                TryLockOnMonster(pressedKey);
                if (lockedOnMonster != null) 
                {
                    ProcessComboInput(pressedKey);
                }

            }
            else
            {
                //if not, see if it's matching the next key.
                ProcessComboInput(pressedKey);
            }
        }
        totalScore.text = score.ToString();

        if (isFeverTime)
        {
            feverTimer -= Time.deltaTime;
            if (feverTimer <= 0)
            {
                EndFeverTime();
            }
        }

        // Fever Gauge UI 업데이트
        feverGaugeImage.fillAmount = feverGauge / maxFeverGauge;

    }

    void TryLockOnMonster(char key)
    {
        //Look for the already spawned monster with matching first letter
        foreach (Monster monster in monsterSpawning.GetActiveMonsters())
        {
            //if it matches
            if (monster.GetFirstLetter() == key)
            {
                //if already locked monster exists,
                if(lockedOnMonster != null)
                {
                    //unlock the already set on lock (cuz it means that it is wrong)
                    lockedOnMonster.SetLockOn(false);
                }

                //Lock on the monster if nothing is locked.
                lockedOnMonster = monster;
                lockedOnMonster.SetLockOn(true);

                return;
            }

        }
    }


    void ProcessComboInput(char key)
    {
        if (lockedOnMonster == null)
        {
            TryLockOnMonster(key);

            if (lockedOnMonster != null)
            {
                HandleComboInput(key);
            }
            return;
        }

        HandleComboInput(key);
    }

    void HandleComboInput(char key)
    {
        if (lockedOnMonster == null) return;
        int gainedScore = lockedOnMonster.ProcessInput(key);

        if (gainedScore == 0)
        {
            ClearLockOn();
        }

        if (lockedOnMonster != null && lockedOnMonster.IsComboComplete())
        {
            ClearLockOn();
        }
    }


    public void ClearLockOn()
    {
        if (lockedOnMonster != null)
        {
            lockedOnMonster.SetLockOn(false);
            lockedOnMonster = null;

        }

        lockOnBackground.SetActive(false);

    }

    public Monster GetLockedOnMonster()
    {
        return lockedOnMonster;
    }

    //Fever Gauge
    public void AddCombo()
    {
        feverGauge += 10f; // 콤보 한 번에 게이지 20% 상승
        feverGauge = Mathf.Clamp(feverGauge, 0, maxFeverGauge);

        // 피버타임 발동 조건 체크
        if (feverGauge >= maxFeverGauge && !isFeverTime)
        {
            StartFeverTime();
        }
    }

    void StartFeverTime()
    {
        isFeverTime = true;
        feverTimer = feverDuration;
        feverGauge = maxFeverGauge;
        feverMultiplier = 3;
        Debug.Log("Fever Time Start!");

        // 피버타임 시각적 효과
        if (feverGaugeImage != null)
        {
            feverGaugeImage.color = new Color(1f, 0.2f, 0.2f); // 피버타임 중 붉은색
        }
    }

    void EndFeverTime()
    {
        isFeverTime = false;
        feverGauge = 0;
        feverMultiplier = 1;
        Debug.Log("Fever Time End!");

        // 기본 색상으로 복귀
        if (feverGaugeImage != null)
        {
            feverGaugeImage.color = Color.white;
        }
    }

    public int GetScoreMultiplier()
    {
        Debug.Log(isFeverTime ? feverMultiplier : 1);
        return isFeverTime ? feverMultiplier : 1;
    }

    public void ShowFloatingScore(Vector3 position, int score, Color color)
    {
        if (floatingScorePrefab == null || worldCanvas == null) return;

        GameObject floatText = Instantiate(floatingScorePrefab, position, Quaternion.identity, worldCanvas.transform);
        FloatingScore floatingScore = floatText.GetComponent<FloatingScore>();

        if (floatingScore != null)
        {
            floatingScore.GetText("+" + score.ToString(), color);
        }
    }

    public void AddFeverGauge(int amount)
    {
        feverGauge += amount;
        feverGauge = Mathf.Clamp(feverGauge, 0, maxFeverGauge);

        if (feverGauge >= maxFeverGauge && !isFeverTime)
        {
            StartFeverTime();
        }
    }

    void UpdateFeverGaugeUI()
    {
        if (feverGaugeImage != null)
        {
            feverGaugeImage.fillAmount = feverGauge / maxFeverGauge;
        }
    }
    public void UpdateScoreUI()
    {
        if (totalScore != null)
        {
            totalScore.text = score.ToString();
        }
    }

    public void AddScore(int amount)
    {

        if (amount <= 0)
        {
            Debug.Log("AddScore called with non-positive amount: " + amount);
            return;
        }

        score += amount;
        Debug.Log("Score Added: " + amount + ", Total Score: " + score);
        UpdateScoreUI();
    }


    public IEnumerator HandleScore(int totalScore)
    {
        Debug.Log("HandleScore called in GameManager. Total score to add: " + totalScore);
        yield return new WaitForSeconds(0.5f);

        int finalScore = totalScore * GetScoreMultiplier();
        Debug.Log("Final Score to Add: " + finalScore);

        AddScore(finalScore);
        Debug.Log("Final Score Added to GameManager: " + finalScore);

        AddFeverGauge(10);

        yield return new WaitForSeconds(1.5f);

        // 몬스터 삭제는 MonsterSpawning에서 수행하도록 유지
        if (lockedOnMonster != null)
        {
            monsterSpawning.RemoveMonster(lockedOnMonster);
        }
    }

}
