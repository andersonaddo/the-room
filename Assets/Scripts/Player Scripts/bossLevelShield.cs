using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossLevelShield : MonoBehaviour
{
    [SerializeField] Transform topPoint, midPoint, bottomPoint;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            print(string.Format("Top Point {0}     Middle Point {1}     Bottom Point {2}", 
                topPoint.position, 
                midPoint.position,
                bottomPoint.position));
        }
    }
}
