using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using JetBrains.Annotations;

public class ComboGenerator : MonoBehaviour
{ 
    private List<List<char>> codeList;
    private List<char> usedFirstLetters;

    private void Start()
    {
        codeList = new List<List<char>> { new List<char> { 'A', 'S', 'D', 'F' }, new List<char> { 'Q', 'W', 'E', 'R' } };
        usedFirstLetters = new List<char>();
    }

    //Will generate Combo once called
    public string generateCombo()
    {
        //List that will be used for this time's code generator
        List<char> chosenCodelist = codeList[ Random.Range(0, codeList.Count)];

        //Letters that will be used for next
        List<char> availableFirstLetter = chosenCodelist.Except(usedFirstLetters).ToList();

        if(availableFirstLetter.Count == 0)
        {
            usedFirstLetters.Clear();
            availableFirstLetter = new List<char>(chosenCodelist);
            return ""; //Letters 
        }


        string currentGeneratedCombo = ""; //Variables that will save the generated combo
        
        int comboLength = Random.Range(2, chosenCodelist.Count);
        
        int index = Random.Range(0, availableFirstLetter.Count);
        char firstLetter = availableFirstLetter[index];
        currentGeneratedCombo += firstLetter; //Adds the first letter into the generatedCombo
        usedFirstLetters.Add(firstLetter); //Adds the first letter in the used first letter list.

        for (int i = 1; i < comboLength; i++)
        { 
            char randomKey = chosenCodelist[Random.Range(0, chosenCodelist.Count)];
            currentGeneratedCombo += randomKey;
        }

        return currentGeneratedCombo; //Give the generated code
    }

    public void RemoveTheFirstLetter(char letter)
    {
        if (usedFirstLetters.Contains(letter))
        {

            usedFirstLetters.Remove(letter);
        }
    }
}
