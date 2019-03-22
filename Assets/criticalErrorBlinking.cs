using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class criticalErrorBlinking : MonoBehaviour
{

    Text text;
    public float interval;

    void OnEnable()
    {
        text = GetComponent<Text>();
        InvokeRepeating("FlashLabel", 0, interval);
    }

    void FlashLabel()
    {
        if (text.enabled)
            text.enabled = false;
        else
            text.enabled = true;
    }


}
