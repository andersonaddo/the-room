using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The script for a laser made of only realtime point lights.
/// It's pretty expensive because it deals with a lot of RTLs
/// </summary>
public class LightLineV3D : MonoBehaviour
{
    public float maxLength = 1.0f;
    Vector3 targetPoint;

    [SerializeField] AnimationCurve shaderProgressCurve;
    [SerializeField] AnimationCurve shaderImpactCurve;

    [SerializeField] int distanceBetweenLights = 1;
    [SerializeField] bool scaleWithTransform = true;
    [SerializeField] float scale = 1f;

    [SerializeField] Light lightPrefab;
    [SerializeField] float lightRange = 5f;
    [SerializeField] float lightIntensity = 1f;

    [SerializeField] Color targetColor;
    Color currentColor;

    private Vector3[] pointLightSpawnPositions;
    private int positionArrayLength;
    private Light[] lights;
    private int roundedMaxLength;

    private float globalProgress;
    private float globalimpactProgress;
    private float progress;
    private float impactProgress;
    private float resultProgress;

    void Start()
    {
        roundedMaxLength = Mathf.RoundToInt(maxLength);
        CreateLights();
        CalculateNumberOfLights();
        ActivateLights();
        LaserControl();
        UpdateLaserParts();
    }


    void Update()
    {
        if (scaleWithTransform)
        {
            scale = gameObject.transform.lossyScale.x;
        }

        CalculateNumberOfLights();
        LaserControl();

        if (positionArrayLength != lights.Length || currentColor != targetColor)
        {
            ActivateLights();
        }

        UpdateLaserParts();
        currentColor = targetColor;
    }

    // Updating and Fading
    void LaserControl()
    {
        progress = shaderProgressCurve.Evaluate(globalProgress);
        impactProgress = shaderImpactCurve.Evaluate(globalimpactProgress);
        resultProgress = progress + impactProgress;
    }

    // Function for recieving color value from Progress Control script
    public void SetFinalColor(Color col)
    {
        targetColor = col;
    }

    // Initialize Laser Line
    void CalculateNumberOfLights()
    {
        float distanceToTarget = Vector3.Distance(transform.position, targetPoint);

        if (distanceToTarget <= maxLength)
        {
            positionArrayLength = Mathf.RoundToInt(distanceToTarget / distanceBetweenLights);
            pointLightSpawnPositions = new Vector3[positionArrayLength];
        }
        else
        {
            positionArrayLength = Mathf.RoundToInt(maxLength / distanceBetweenLights);
            pointLightSpawnPositions = new Vector3[positionArrayLength];
        }
    }

    // Instantiating Light Prefabs
    private void CreateLights()
    {
        lights = new Light[roundedMaxLength];

        for (int i = 0; i < roundedMaxLength; i++)
        {
            lights[i] = (Light)Instantiate(lightPrefab);
            lights[i].transform.parent = transform;
            lights[i].gameObject.SetActive(false);
            lights[i].color = targetColor;
        }
    }

    // Turn Lights On and Off depending on distance
    private void ActivateLights()
    {
        for (int i = 0; i < positionArrayLength; i++)
        {
            lights[i].color = targetColor;
            lights[i].gameObject.SetActive(true);
        }

        for (int i = positionArrayLength + 1; i < roundedMaxLength; i++)
        {
            lights[i].gameObject.SetActive(false);
        }
    }

    // Updating Lights Intensity and Range
    void UpdateLaserParts()
    {
        for (int i = 0; i < positionArrayLength; i++)
        {
            pointLightSpawnPositions[i] = new Vector3(0f, 0f, 0f) + new Vector3(0f, 0f, i * distanceBetweenLights * (1 / scale));
            lights[i].transform.localPosition = pointLightSpawnPositions[i];
            lights[i].intensity = resultProgress * lightIntensity;
            lights[i].range = lightRange * scale;
        }
    }

    // Function for recieving progress value from Progress Control script
    public void SetGlobalProgress(float gp)
    {
        globalProgress = gp;
    }

    // Function for recieving impact progress value from Progress Control script
    public void SetGlobalImpactProgress(float gp)
    {
        globalimpactProgress = gp;
    }

    public void setTargetPoint(Vector3 point)
    {
        targetPoint = point;
    }
}
