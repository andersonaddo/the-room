using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class iTweenStartScaler : MonoBehaviour
{

    public iTween.EaseType easeType;
    public float time;

    // Start is called before the first frame update
    public void scale()
    {
        iTween.ScaleFrom(gameObject, iTween.Hash("scale", Vector3.zero,
                                                  "easetype", easeType,
                                                  "time", time));
    }
}
