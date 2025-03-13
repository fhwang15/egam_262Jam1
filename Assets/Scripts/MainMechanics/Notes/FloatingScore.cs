using UnityEngine;
using TMPro;

public class FloatingScore : MonoBehaviour
{

    public float moveSpeed = 5f;
    public float duration = 1f;

    public Vector3 moveDirection = Vector3.up; //direction of the text

    private TextMeshProUGUI score;
    private float timer;


    private void Awake()
    {
        score = GetComponentInChildren<TextMeshProUGUI>();
    }


    public void GetText(string scoreText, Color color)
    {
        score.text = scoreText;
        score.color = color;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        Color color = score.color;
        color.a = Mathf.Lerp(1f, 0f, timer/duration);
        score.color = color;

        if (timer > duration)
        {
            Destroy(gameObject);
        }

    }
}
