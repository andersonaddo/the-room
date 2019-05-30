using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scoreDisplayer : MonoBehaviour
{
    int score;
    int currentlyDisplayedScore;
    int difference; //the difference between the two above variables
    float changePerSecond;
    Text text;

    public float countingDuration;

    // Use this for initialization
    void Start()
    {
        text = GetComponent<Text>();
        gameManager.Instance.scoreChanged += startCountingUp;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentlyDisplayedScore != score)
        {
            currentlyDisplayedScore = Mathf.RoundToInt(Mathf.MoveTowards(currentlyDisplayedScore, score, Mathf.Abs(changePerSecond * Time.deltaTime)));
            text.text = currentlyDisplayedScore.ToString();
        }
    }

    void startCountingUp(int newScore)
    {
        score = newScore;
        difference = score - currentlyDisplayedScore;
        changePerSecond = difference / countingDuration;
    }
}
