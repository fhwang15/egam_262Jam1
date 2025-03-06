using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class MonsterSpawning : MonoBehaviour
{
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

    public string[] generatedComboList;
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
            SpawnNote();
            yield return new WaitForSeconds(1f); // 노트 생성 간격
            DecreaseMonsterTime();
        }
    }

    void SpawnNote()
    {

        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("Spawn points are empty!"); // 디버깅용
            return; // spawnPoints가 비어 있으면 아무것도 생성하지 않음
        }

        Debug.Log("Am I called?");
        if (spawnPoints.Length == 0) return;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject note = Instantiate(monsterPrefab, spawnPoint.position, Quaternion.identity);

        string combo = ComboGenerator();
        if(combo != null)
        {
            comboQueue.Enqueue(combo);
            note.GetComponent<Monster>().SetCombo(combo);
        }


        StartCoroutine(DestroyAfterTime(note, currentMonsterTime));
    }

    IEnumerator DestroyAfterTime(GameObject note, float time)
    {
        yield return new WaitForSeconds(time);
        if (note != null) Destroy(note);

        if (comboQueue.Count > 0)
        {
            string removedCombo = comboQueue.Dequeue();
            Debug.Log("콤보 제거: " + removedCombo);
        }

        char firstLetter = note.GetComponent<Monster>().GetFirstLetter(); 
        RemoveTheFirstLetter(firstLetter);

    }

    void DecreaseMonsterTime()
    {
        if (currentMonsterTime > minMonsterTime)
        {
            currentMonsterTime -= difficultyIncreaseRate;
        }
    }

    public string ComboGenerator()
    {

       List<char> availableFirstLetters = comboChars.Except(usedFirstLetters).ToList();

        if(availableFirstLetters.Count == 0) 
        {
            return null; //All letters have been used = max monster spawned
        }

        string currentGeneratedCombo = "";
        int comboLength = Random.Range(2, comboChars.Count);//Length of code

        //Determine the first Letter seperately so that it doesn't overlap with others
        int index = Random.Range(0, availableFirstLetters.Count);
        char firstLetter = availableFirstLetters[index];
        currentGeneratedCombo += firstLetter;
        usedFirstLetters.Add(firstLetter);

        for (int i = 0; i < comboLength; i++)
        {
            char randomKey = comboChars[Random.Range(0, comboChars.Count)];
            currentGeneratedCombo += randomKey;
        }

        Debug.Log("Generated Combo is " + currentGeneratedCombo);
        return currentGeneratedCombo;
    }

    public void RemoveTheFirstLetter(char letter)
    {
        if (usedFirstLetters.Contains(letter))
        {

            usedFirstLetters.Remove(letter);
        }
    }

}
