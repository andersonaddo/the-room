using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class transitioner : MonoBehaviour
{
    //RIght now the only thing this thing does is hold some information needed when transitioneing between levels
    public Vector3 positionFirstRoom, positionSecondRoom;
    [ColorUsage(false, true)]public Color skyColorSecondRoom;
    public float skyColorIntensityFirstRoom, skyColorIntensitySecondRoom;

    public enum gameStage
    {
        firstRoom,
        escapeCorridor,
        bossBattle
    }

    public gameStage currentStage;
}
