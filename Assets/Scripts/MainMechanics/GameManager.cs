using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
        lockOnBackground.SetActive(false);
        monsterSpawning = GetComponent<MonsterSpawning>();

        
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
            }
            else
            {
                //if not, see if it's matching the next key.
                ProcessComboInput(pressedKey);
            }
        }
        totalScore.text = score.ToString();
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
                    lockOnBackground.SetActive(false);
                }

                //Lock on the monster if nothing is locked.
                lockedOnMonster = monster;
                lockedOnMonster.SetLockOn(true);

                lockOnBackground.SetActive(true);
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
                lockOnBackground.SetActive(false);
                lockedOnMonster = null;
            }

            if (lockedOnMonster != null && lockedOnMonster.IsComboComplete())
            {
                lockedOnMonster.SetLockOn(false);
                lockOnBackground.SetActive(false);
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
