using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cartMovement : MonoBehaviour
{
    public float movementSpeed;
    public string pathName;

    // Start is called before the first frame update
    void Start()
    {
        startMovement();   
    }

    // Update is called once per frame
    void startMovement()
    {
        iTween.MoveTo(gameObject, iTween.Hash("path", iTweenPath.GetPath(pathName),
                                   "speed", movementSpeed,
                                   "onComplete", "startMovement",
                                   "oncompletetarget", gameObject,
                                   "easetype", iTween.EaseType.linear));
    }
}
