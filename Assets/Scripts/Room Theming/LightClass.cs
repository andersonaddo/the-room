using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightClass : MonoBehaviour
{
    public lightClassifications classification;

    public enum lightClassifications
    {
        firstRoomBasic,
        firstRoomColumn,
        firstRoomSecondary
    }

    public Light light { get { return GetComponent<Light>(); } }
}
