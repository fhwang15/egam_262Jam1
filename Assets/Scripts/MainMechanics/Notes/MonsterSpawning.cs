using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class MonsterSpawning : MonoBehaviour
{
    public static MonsterSpawning Instance;
    //Spawning Related
    public GameObject monsterPrefab;
    public Transform[] spawnPoints;

    public float initialMonsterTime = 3f;
    public float minMonsterTime = 1f;
    public float difficultyIncreaseRate = 0.05f;

    private float currentMonsterTime;

    //Combo Relataed
    private List<char> comboChars;
    public Queue<string> comboQueue = new Queue<string>();
    //private List<char> availableFirstLetter;

    public List<char> usedFirstLetters = new List<char>();
    private List<Monster> activeMonsters = new List<Monster>();

    public string[] generatedComboList;


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
        comboChars = new List<char> { 'A', 'S', 'D', 'F', 'H', 'J', 'K', 'L' };
        generatedComboList = new string[spawnPoints.Length];

        currentMonsterTime = initialMonsterTime;
        StartCoroutine(SpawnMonster());
            
    }

    IEnumerator SpawnMonster()
    {
        while (true)
        {
            Debug.Log($"몬스터 소환 체크: activeMonsters.Count = {activeMonsters.Count}, spawnPoints.Length = {spawnPoints.Length}");
            
            if(activeMonsters.Count < spawnPoints.Length)
            {
                SpawnNote();
            }
            yield return new WaitForSeconds(1f);
            DecreaseMonsterTime();
        }
    }

    void SpawnNote()
    {
        if (spawnPoints.Length == 0)
        {
            return;
        }

        foreach (Transform spawnPoint in spawnPoints)
        {
            if (!spawnPointOccupied(spawnPoint))
            {

                GameObject note = Instantiate(monsterPrefab, spawnPoint.position, Quaternion.identity);
                Monster monster = note.GetComponent<Monster>();
                string combo = ComboGenerator();
                if (combo != null)
                {
                    comboQueue.Enqueue(combo);
                    monster.SetCombo(combo);
                    activeMonsters.Add(monster);
                }

                StartCoroutine(DestroyAfterTime(monster, currentMonsterTime));
                return;
            }
        }
    }

    IEnumerator DestroyAfterTime(Monster monster, float time)
    {
        yield return new WaitForSeconds(time);

        if (monster != null)
        {
            activeMonsters.Remove(monster);
            RemoveTheFirstLetter(monster.GetFirstLetter());
            Destroy(monster.gameObject);
        }

    }

    bool spawnPointOccupied(Transform spawnPoint)
    {
        foreach (Monster monster in activeMonsters)
        {
            if (monster.transform.position == spawnPoint.position)
            {
                return true;
            }
        }
        return false;
    }


    void DecreaseMonsterTime()
    {
        if (currentMonsterTime > minMonsterTime)
        {
            return;
            //currentMonsterTime -= difficultyIncreaseRate;
        }
    }

    public string ComboGenerator()
    {

       List<char> availableFirstLetters = comboChars.Except(usedFirstLetters).ToList();

        if(availableFirstLetters.Count == 0) 
        {
            Debug.LogWarning(" 사용할 수 있는 첫 글자가 부족해서 몬스터를 소환할 수 없음!");
            usedFirstLetters.Clear(); 
            availableFirstLetters = new List<char>(comboChars);

            return null; //All letters have been used = max monster spawned
        }

        string currentGeneratedCombo = "";
        int comboLength = Random.Range(2, comboChars.Count);//Length of code

        //Determine the first Letter seperately so that it doesn't overlap with others
        int index = Random.Range(0, availableFirstLetters.Count);
        char firstLetter = availableFirstLetters[index];
        currentGeneratedCombo += firstLetter;
        usedFirstLetters.Add(firstLetter);

        for (int i = 1; i < comboLength; i++)
        {
            char randomKey = comboChars[Random.Range(0, comboChars.Count)];
            currentGeneratedCombo += randomKey;
        }
        return currentGeneratedCombo;
    }

    public void RemoveTheFirstLetter(char letter)
    {
        if (usedFirstLetters.Contains(letter))
        {

            usedFirstLetters.Remove(letter);
        }
    }

    public void RemoveMonster(Monster monster)
    {
        if (monster != null)
        {
            RemoveTheFirstLetter(monster.GetFirstLetter());
            Destroy(monster.gameObject);


            if (activeMonsters.Contains(monster))
            {
                activeMonsters.Remove(monster);
            }



            if (GameManager.Instance != null && GameManager.Instance.GetLockedOnMonster() == monster)
            {
                GameManager.Instance.ClearLockOn(); 
            }
        }
    }


    public List<Monster> GetActiveMonsters()
    {
        return activeMonsters;
    }

}
