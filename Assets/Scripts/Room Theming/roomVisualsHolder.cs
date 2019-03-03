using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[CreateAssetMenu(menuName = "MyAssets/Room Theme")]
public class roomVisualsHolder : ScriptableObject {

    public cubeTypes targetType;
    public List<roomLightConfiguration> lightConfigurations = new List<roomLightConfiguration>();
    public List<roomMaterialConfiguration> materialConfigurations = new List<roomMaterialConfiguration>();
    public PostProcessProfile roomProfile;   
}


[System.Serializable]
public class roomLightConfiguration
{
    public LightClass.lightClassifications targetClassification = LightClass.lightClassifications.firstRoomBasic;
    public Color realTimeLightColor;
    public float intensity, range;
}


[System.Serializable]
public class roomMaterialConfiguration
{
    public Material mat;
    [ColorUsage(false, true)]
    public Color emissMatColor;

}
