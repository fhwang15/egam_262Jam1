using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int finalScore;
    public int currentScore;

    public float SpawnTimeInterval;
    public float Timer;

    bool gameStart;
    private bool isLockedOn = false;  // 록온 상태 추적
    private Monster lockedOnMonster;  // 록온된 몬스터
    private int totalScore = 0;

    // 콤보 입력 처리
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

    // 특정 입력에 맞는 몬스터를 찾는 함수
    private Monster GetMonsterForInput(char input)
    {
        // 이 부분은 화면에 있는 몬스터들에서 첫 글자가 맞는 몬스터를 찾아야 한다.
        // 몬스터 리스트에서 첫 글자와 input이 일치하는 몬스터를 찾는 방식으로 구현
        return FindObjectOfType<Monster>();  // 예시로 한 몬스터만 찾는 코드, 실제론 리스트에서 찾아야 함
    }

    // 록온 시작
    private void StartLockedOn(Monster monster)
    {
        isLockedOn = true;
        lockedOnMonster = monster;
        lockedOnMonster.SetCombo(lockedOnMonster.GetFirstLetter().ToString());  // 해당 몬스터의 콤보로 설정
    }

    // 록온 종료
    private void EndLockedOnCombo()
    {
        isLockedOn = false;
        lockedOnMonster = null;
    }

}
