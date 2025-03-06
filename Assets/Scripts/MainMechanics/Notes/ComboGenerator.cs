using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ComboGenerator : MonoBehaviour
{

    private List<char> comboChars;
    private List<char> nextFirstLetter;

    public string currentGeneratedCombo;

    public string[] generatedComboList;

    void Start()
    {
        comboChars = new List<char> { 'A', 'S', 'D', 'F', 'H', 'J', 'K', 'L' };

        nextFirstLetter = new List<char>(comboChars);
        

        generatedComboList = new string[5];

        for (int i = 0; i < 5; i++)
        {

            int comboLength = Random.Range(2, comboChars.Count);//Length of code



            char randomKey = comboChars[Random.Range(0, comboChars.Count)];
            currentGeneratedCombo += randomKey;
        }

        Debug.Log("Generated Combo is " + currentGeneratedCombo);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
