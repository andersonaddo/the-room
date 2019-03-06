using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class roomVisualsChanger : MonoBehaviour
{
    public float completionTime; 
    float stopTime;

    public List<roomVisualsHolder> roomThemes = new List<roomVisualsHolder>();
    int currentIndex;
    List<LightChangeDeltas> lightDeltas = new List<LightChangeDeltas>();
    List<MaterialChangeDeltas> matDeltas = new List<MaterialChangeDeltas>();

    void Awake()
    {
        currentIndex = Random.Range(0, roomThemes.Count);
        transitionTo(roomThemes[currentIndex]);
    }

    void Update()
    {
        if (stopTime >= Time.time)
        {
            foreach (LightChangeDeltas lightDelta in lightDeltas)
            {
                foreach (LightClass lightClass in FindObjectsOfType<LightClass>().Where(light => light.classification == lightDelta.lightClassification))
                {
                    lightDelta.applyChange(lightClass.light, completionTime);
                }
            }

            foreach (MaterialChangeDeltas matDelta in matDeltas)
                matDelta.applyChange(completionTime);
        }
    }

    public void chooseNextTheme()
    {
        currentIndex++;
        if (currentIndex == roomThemes.Count) currentIndex = 0;
        transitionTo(roomThemes[currentIndex]);
    }

    //Should be called in update for timing to be correct
    void transitionTo(roomVisualsHolder newHolder)
    {
        gameManager.Instance.updateTargetType(newHolder.targetType);
        lightDeltas.Clear();
        matDeltas.Clear();
        foreach(roomLightConfiguration lightConfig in newHolder.lightConfigurations)
        {
            LightClass sampleLightClass = FindObjectsOfType<LightClass>().Where(light => light.classification == lightConfig.targetClassification).FirstOrDefault();
            if (sampleLightClass != null) lightDeltas.Add(new LightChangeDeltas(lightConfig, sampleLightClass.light));
        }

        foreach (roomMaterialConfiguration matConfig in newHolder.materialConfigurations)
        {
           matDeltas.Add(new MaterialChangeDeltas(matConfig));
        }

        //Changing will start on next update()
        stopTime = Time.time + completionTime;
    }
}

public struct LightChangeDeltas
{
    public LightClass.lightClassifications lightClassification;
    float deltaRange, deltaIntensity;
    float deltaR, deltaG, deltaB;
    roomLightConfiguration newConfig;

    public LightChangeDeltas(roomLightConfiguration newConfig, Light sampleLight)
    {
        this.newConfig = newConfig;
        lightClassification = newConfig.targetClassification;
        deltaIntensity = Mathf.Abs(newConfig.intensity - sampleLight.intensity);
        deltaRange = Mathf.Abs(newConfig.range - sampleLight.range);
        deltaR = Mathf.Abs(newConfig.realTimeLightColor.r - sampleLight.color.r);
        deltaG = Mathf.Abs(newConfig.realTimeLightColor.g - sampleLight.color.g);
        deltaB = Mathf.Abs(newConfig.realTimeLightColor.b - sampleLight.color.b);
    }

    public void applyChange(Light light, float completionTime)
    {
        light.intensity = Mathf.MoveTowards(light.intensity, newConfig.intensity, deltaIntensity * Time.deltaTime / completionTime);
        light.range = Mathf.MoveTowards(light.range, newConfig.range, deltaRange * Time.deltaTime / completionTime);
        float r = Mathf.MoveTowards(light.color.r, newConfig.realTimeLightColor.r, deltaR * Time.deltaTime / completionTime);
        float g = Mathf.MoveTowards(light.color.g, newConfig.realTimeLightColor.g, deltaG * Time.deltaTime / completionTime);
        float b = Mathf.MoveTowards(light.color.b, newConfig.realTimeLightColor.b, deltaB * Time.deltaTime / completionTime);
        light.color = new Color(r, g, b);
    }

}


public struct MaterialChangeDeltas
{
    float deltaEmR, deltaEmG, deltaEmB;
    roomMaterialConfiguration newConfig;

    public MaterialChangeDeltas(roomMaterialConfiguration newConfig)
    {
        this.newConfig = newConfig;
        deltaEmR = Mathf.Abs(newConfig.emissMatColor.r - newConfig.mat.GetColor("_EmissionColor").r);
        deltaEmG = Mathf.Abs(newConfig.emissMatColor.g - newConfig.mat.GetColor("_EmissionColor").g);
        deltaEmB = Mathf.Abs(newConfig.emissMatColor.b - newConfig.mat.GetColor("_EmissionColor").b);
    }

    public void applyChange(float completionTime)
    {
        Color emColor = newConfig.mat.GetColor("_EmissionColor");
        float r = Mathf.MoveTowards(emColor.r, newConfig.emissMatColor.r, deltaEmR * Time.deltaTime / completionTime);
        float g = Mathf.MoveTowards(emColor.g, newConfig.emissMatColor.g, deltaEmG * Time.deltaTime / completionTime);
        float b = Mathf.MoveTowards(emColor.b, newConfig.emissMatColor.b, deltaEmB * Time.deltaTime / completionTime);
        newConfig.mat.SetColor("_EmissionColor", new Color(r, g, b));
    }
}
