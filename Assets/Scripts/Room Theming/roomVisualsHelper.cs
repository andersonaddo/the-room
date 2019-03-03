using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class roomVisualsHelper : MonoBehaviour
{
    public List<roomVisualsMaterialChanger> materialChangers = new List<roomVisualsMaterialChanger>();
    public List<roomVisualsLightsChanger> changers = new List<roomVisualsLightsChanger>();


    public void testMaterialEmission(roomVisualsMaterialChanger matChanger)
    {
        if (matChanger.mat == null) return;
        matChanger.mat.SetColor("_EmissionColor", matChanger.testMainEmissMatColor);
    }

    public void resetToEmissionDefault(roomVisualsMaterialChanger matChanger)
    {
        if (matChanger.mat == null) return;
        matChanger.mat.SetColor("_EmissionColor", matChanger.defaultMainEmissMatColor);
    }


    public void testRealtimeLights(roomVisualsLightsChanger lightsChanger)
    {
        List<LightClass> testableLights = FindObjectsOfType<LightClass>()
           .Where((light) => light.classification == lightsChanger.targetClassification)
           .ToList<LightClass>();

        foreach (LightClass testLight in testableLights)
        {
            testLight.light.color = lightsChanger.testRealTimeLightColor;
            testLight.light.intensity = lightsChanger.testIntensity;
            testLight.light.range = lightsChanger.testRange;
        }
    }


    public void resetRealtimeLights(roomVisualsLightsChanger lightsChanger)
    {
        List<LightClass> testableLights = FindObjectsOfType<LightClass>()
            .Where((light) => light.classification == lightsChanger.targetClassification)
            .ToList<LightClass>();

        foreach (LightClass testLight in testableLights)
        {
            testLight.light.color = lightsChanger.defaultRealTimeLightColor;
            testLight.light.intensity = lightsChanger.defaultIntensity;
            testLight.light.range = lightsChanger.defaultRange;
        }
    }
}

/// <summary>
/// Holds information about defualt and currently-being-tested values for lights
/// Can be manipulated in Editor for live and en-mass light editing
/// </summary>
[System.Serializable]
public class roomVisualsLightsChanger
{
    public LightClass.lightClassifications targetClassification = LightClass.lightClassifications.firstRoomBasic;
    public Color testRealTimeLightColor, defaultRealTimeLightColor;
    public float testIntensity, defaultIntensity, defaultRange, testRange;

    public bool isExpanded, autoUpdate; //Just for inspector
}


/// <summary>
/// Holds information about defualt and currently-being-tested values for materials
/// Can be manipulated in Editor for live material editing
/// </summary>
[System.Serializable]
public class roomVisualsMaterialChanger
{
    public Material mat;

    [ColorUsage(false, true)]
    public Color defaultMainEmissMatColor;
    [ColorUsage(false, true)]
    public Color testMainEmissMatColor;

    public bool isExpanded, autoUpdate; //Just for inspector
}
