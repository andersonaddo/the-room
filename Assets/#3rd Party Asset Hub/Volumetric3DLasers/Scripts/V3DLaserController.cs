using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The main script that facilitates the laser effect
/// </summary>
public class V3DLaserController : MonoBehaviour {

    [Tooltip("Affect all the child lazers' max lengths")]
    [SerializeField] bool changeAllMaxLength = true; 
    [SerializeField] float maxLength = 32f;

    bool isShooting = true;
    [HideInInspector] public bool justStartedShooting;
    Vector3 targetPoint;

    [SerializeField] float globalProgressSpeed = 1f;
    [SerializeField] float globalImpactProgressSpeed = 1f; 
    float globalProgress; //Used to monitor the fading out and shrinking effect when you stop shooting
    float globalImpactProgress; //Used to monitor the impact effect of the lazer when you first start shooting 

    [Tooltip("Affect all the child lazers' and effects' colors")]
    [SerializeField] bool colorizeAll = true;
    [SerializeField] Color color;

    [Range(0.2f, 1.0f)]
    [SerializeField]
    float gammaLinear = 1f; //Not sure what this does

    //For affecting the color of the drone shooting the lazer
    [SerializeField] Renderer droneRenderer;
    [SerializeField] float droneRendererColorMultiplier = 3f;

    public StartPointEffectControllerV3D startPointEffect;
    public EndPointEffectControllerV3D endPointEffect;
    public SmartWaveParticlesControllerV3D smartWaveParticles;
    public SFXControllerV3D sfxcontroller;

    LaserLineV3D[] laserLines;
    LightLineV3D[] lightLines;
    Renderer[] childRenderers;

   void Start()
    {
        globalProgress = 1f;
        globalImpactProgress = 1f;
        laserLines = GetComponentsInChildren<LaserLineV3D>(true);
        lightLines = GetComponentsInChildren<LightLineV3D>(true);
        childRenderers = GetComponentsInChildren<Renderer>(true);
    }

    public void ChangeColor(Color color)
    {
        this.color = color;
    }

    void Update()
    {
        targetPoint = transform.parent.position + transform.parent.forward * 6;
        isShooting = Input.GetMouseButton(0);
        justStartedShooting = Input.GetMouseButtonDown(0);

        UpdateColorOfChildren();
        UpdateProgressVariables();
        UpdateEffectManagers();
        UpdateLaserChildren();
        sfxcontroller.SetGlobalProgress(1f - globalProgress);


        // Control Gamma and Linear modes
        foreach (Renderer rend in childRenderers)
        {
            rend.material.SetFloat("_GammaLinear", gammaLinear);
        }

        //Updating Drone Material
        if (droneRenderer != null)
        {
            droneRenderer.material.SetColor("_EmissionColor", color * droneRendererColorMultiplier);
        }
    }

    private void UpdateProgressVariables()
    {
        if (!isShooting)
            globalProgress = Mathf.Clamp01(globalProgress + Time.deltaTime * globalProgressSpeed);

        if (justStartedShooting)
            globalImpactProgress = 0f;
        else
            globalImpactProgress = Mathf.Clamp01(globalImpactProgress + Time.deltaTime * globalImpactProgressSpeed);

        if (isShooting)
        {
            globalProgress = 0f;
            endPointEffect.shouldEmitParticleSystems = true;
        }
        else
        {
            endPointEffect.shouldEmitParticleSystems = false;
        }

    }

    private void UpdateLaserChildren()
    {
        foreach (LaserLineV3D ll in laserLines)
        {
            ll.SetGlobalProgress(globalProgress);
            ll.SetGlobalImpactProgress(globalImpactProgress);
            ll.setTargetPoint(targetPoint);

            if (changeAllMaxLength) ll.maxLength = maxLength;
        }

        foreach (LightLineV3D lil in lightLines)
        {
            lil.SetGlobalProgress(globalProgress);
            lil.SetGlobalImpactProgress(globalImpactProgress);
            lil.setTargetPoint(targetPoint);

            if (changeAllMaxLength == true) lil.maxLength = maxLength;
        }
    }

    private void UpdateColorOfChildren()
    {
        if (colorizeAll)
        {
            foreach (LightLineV3D lil in lightLines)
            {
                lil.SetFinalColor(color);
            }

            startPointEffect.SetFinalColor(color);
            endPointEffect.SetFinalColor(color);

            foreach (Renderer rend in childRenderers)
            {
                rend.material.SetColor("_FinalColor", color);
            }
        }
    }

    private void UpdateEffectManagers()
    {
        // Sending global progress value to other scripts
        startPointEffect.SetGlobalProgress(globalProgress);
        startPointEffect.SetGlobalImpactProgress(globalImpactProgress);
        endPointEffect.SetGlobalProgress(globalProgress);
        endPointEffect.SetGlobalImpactProgress(globalImpactProgress);
        smartWaveParticles.SetGlobalProgress(globalProgress);
    }
}
