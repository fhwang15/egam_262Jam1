using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    private MonsterSpawning monsterSpawning;
    private Monster lockedOnMonster = null;
    public int score = 0;

    public TextMeshProUGUI totalScore;


    void Awake()
    {
        // Singleton 패턴으로 Instance 할당
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
    }

    void Update()
    {

        if (Input.anyKeyDown)
        {
            string input = Input.inputString.ToUpper();
            if (string.IsNullOrEmpty(input)) return;

            char pressedKey = input[0];

            if (lockedOnMonster == null)
            {
                TryLockOnMonster(pressedKey);
            }
            else
            {
                ProcessComboInput(pressedKey);
            }
        }
        totalScore.text = score.ToString();
    }

    void TryLockOnMonster(char key)
    {
        foreach (Monster monster in monsterSpawning.GetActiveMonsters())
        {
            if (monster.GetFirstLetter() == key)
            {
                if(lockedOnMonster != null)
                {
                    lockedOnMonster.SetLockOn(false);
                }

                lockedOnMonster = monster;
                lockedOnMonster.SetLockOn(true);
                return;
            }
        }
    }

    void ProcessComboInput(char key)
    {
        if (lockedOnMonster != null)
        {
            int gainedScore = lockedOnMonster.ProcessInput(key);
            if (gainedScore > 0)
            {
                score += gainedScore;
            }
            else
            {
                lockedOnMonster.SetLockOn(false);
                lockedOnMonster = null;
            }

            if (lockedOnMonster != null && lockedOnMonster.IsComboComplete())
            {
                lockedOnMonster.SetLockOn(false);
                lockedOnMonster = null; 
            }
        }
    }

    public void ClearLockOn()
    {
        lockedOnMonster = null;
    }

    public Monster GetLockedOnMonster()
    {
        return lockedOnMonster;
    }


}
