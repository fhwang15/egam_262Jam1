using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class MonsterSpawning : MonoBehaviour
{
    public static MonsterSpawning Instance;
    private ComboGenerator comboGenerator;
    
    //Spawning Related
    public GameObject monsterPrefab; //Prefab of Monster
    public Transform[] spawnPoints; //Spawning Point

    public float initialMonsterTime = 3f;
    public float minMonsterTime = 1f;
    public float difficultyIncreaseRate = 0.05f; //Will be used later
    public float monsterSpawnTime;

    private float currentMonsterTime; //

    //Monster
    private List<Monster> activeMonsters = new List<Monster>();

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
        comboGenerator = GetComponent<ComboGenerator>(); //Calls the ComboGenerator Script (Script that creates the code)
        currentMonsterTime = initialMonsterTime;
        StartCoroutine(SpawnMonster());
    }

    IEnumerator SpawnMonster()
    {
        while (true)
        {
            if(activeMonsters.Count < spawnPoints.Length) //If all spawnpoints are not taken,
            {
                SpawnNote(); //Spawn something
            }
            yield return new WaitForSeconds(monsterSpawnTime); //With a term in between.
            DecreaseMonsterTime();
        }
    }

    void SpawnNote()
    {
        if (spawnPoints.Length == 0)
        {
            return;
        }

        while (true)
        {
            Transform currentSpawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
            if (!spawnPointOccupied(currentSpawn))
            {

                GameObject note = Instantiate(monsterPrefab, currentSpawn.position, Quaternion.identity);
                Monster monster = note.GetComponent<Monster>();
                string combo = comboGenerator.generateCombo();

                
                if (combo != null)
                {
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
            comboGenerator.RemoveTheFirstLetter(monster.GetFirstLetter());
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

    public void RemoveMonster(Monster monster)
    {
        if (monster != null)
        {
            comboGenerator.RemoveTheFirstLetter(monster.GetFirstLetter());
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
