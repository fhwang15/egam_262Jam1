using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CircleController : MonoBehaviour
{
    public GameObject[] circleParts;
    private int currentCombo = 0;
    private Vector3[] rotationSpeeds;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rotationSpeeds = new Vector3[circleParts.Length];
        for (int i = 0; i < circleParts.Length; i++)
        {
            circleParts[i].SetActive(false);
            float randomSpeed = Random.Range(-150f, 150f);
            rotationSpeeds[i] = new Vector3(0, 0, randomSpeed);
        }

    }

    public void OnComboInput()
    {
        if (currentCombo < circleParts.Length)
        {
            StartCoroutine(ActivateCirclePart(currentCombo));
            currentCombo++;
        }

    }

    public void ResetCircleCombo()
    {
        currentCombo = 0;
        for (int i = 0; i < circleParts.Length; i++)
        {
            circleParts[i].SetActive(false);
        }
    }

    private void Update()
    {
        for (int i = 0; i < circleParts.Length; i++)
        {
            if (circleParts[i].activeSelf)
            {
                circleParts[i].transform.Rotate(rotationSpeeds[i] * Time.deltaTime);
            }
        }
    }

    void RandomizeColors()
    {
        for (int i = 0; i < circleParts.Length; i++)
        {
            SpriteRenderer sprite = circleParts[i].GetComponent<SpriteRenderer>();
            if (sprite != null)
            {
                sprite.color = new Color(Random.value, Random.value, Random.value, 1f);
            }
        }
    }


    private IEnumerator ActivateCirclePart(int index)
    {
        yield return new WaitForSeconds(0.1f);
        circleParts[index].SetActive(true); if (index < circleParts.Length)
        {
            circleParts[index].SetActive(true);


            SpriteRenderer sprite = circleParts[index].GetComponent<SpriteRenderer>();
            if (sprite != null)
            {
                sprite.color = new Color(Random.value, Random.value, Random.value, 1f);
            }


            float randomSpeed = Random.Range(-150f, 150f);
            rotationSpeeds[index] = new Vector3(0, 0, randomSpeed);
        }

    }
}
