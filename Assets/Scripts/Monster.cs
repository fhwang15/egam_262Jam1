using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class Monster : MonoBehaviour
{

    public List<Sprite> monsters;

    private string combo; 
    private int currentComboIndex = 0; 
    public int[] comboScores = new int[] { 10, 20, 30, 50, 80, 130, 210, 340 };

    public void SetCombo(string newCombo)
    {
        combo = newCombo;
        currentComboIndex = 0;
    }

    // 첫 글자 반환
    public char GetFirstLetter()
    {
        return combo[0];
    }

    // 콤보 입력 처리
    public int ProcessInput(char input)
    {
        if (currentComboIndex < combo.Length && input == combo[currentComboIndex])
        {
            currentComboIndex++;
            return comboScores[currentComboIndex - 1];
        }
        else
        {
            return 0; 
        }
    }
    public bool IsComboComplete()
    {
        return currentComboIndex >= combo.Length;
    }
}
