using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int finalScore;
    public int currentScore;

    public float SpawnTimeInterval;
    public float Timer;

    bool gameStart;
    private bool isLockedOn = false;  // �Ͽ� ���� ����
    private Monster lockedOnMonster;  // �Ͽµ� ����
    private int totalScore = 0;

    // �޺� �Է� ó��
    public void InputCombo(char input)
    {
        if (isLockedOn && lockedOnMonster != null)
        {
            int score = lockedOnMonster.ProcessInput(input);
            if (score > 0)
            {
                totalScore += score;
            }
            else
            {
                EndLockedOnCombo();
            }

            if (lockedOnMonster.IsComboComplete())
            {
                EndLockedOnCombo();
            }
        }
        else
        {
            Monster monster = GetMonsterForInput(input);
            if (monster != null && !isLockedOn)
            {
                StartLockedOn(monster);
            }
        }
    }

    // Ư�� �Է¿� �´� ���͸� ã�� �Լ�
    private Monster GetMonsterForInput(char input)
    {
        // �� �κ��� ȭ�鿡 �ִ� ���͵鿡�� ù ���ڰ� �´� ���͸� ã�ƾ� �Ѵ�.
        // ���� ����Ʈ���� ù ���ڿ� input�� ��ġ�ϴ� ���͸� ã�� ������� ����
        return FindObjectOfType<Monster>();  // ���÷� �� ���͸� ã�� �ڵ�, ������ ����Ʈ���� ã�ƾ� ��
    }

    // �Ͽ� ����
    private void StartLockedOn(Monster monster)
    {
        isLockedOn = true;
        lockedOnMonster = monster;
        lockedOnMonster.SetCombo(lockedOnMonster.GetFirstLetter().ToString());  // �ش� ������ �޺��� ����
    }

    // �Ͽ� ����
    private void EndLockedOnCombo()
    {
        isLockedOn = false;
        lockedOnMonster = null;
    }

}
