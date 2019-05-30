using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class firstRoomProgressionTracker : MonoBehaviour
{

    Image image;
    public GameObject scoreText, scoreTitleText, errorText;
    bool hasSwitched;

    void Start()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {
        image.fillAmount = gameManager.Instance.progressionPercentage;
        if (!hasSwitched && gameManager.Instance.progressionPercentage == 1)
        {
            hasSwitched = true;
            scoreText.SetActive(false);
            scoreTitleText.SetActive(false);
            errorText.SetActive(true);
        }
    }
}
