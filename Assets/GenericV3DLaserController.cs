using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericV3DLaserController : MonoBehaviour
{
    [Tooltip("Affect all the child lazers' max lengths")]
    [SerializeField]
    bool changeAllMaxLength = true;
    [SerializeField] float maxLength = 32f;

    [HideInInspector] public bool isShooting = false;
    Vector3 targetPoint = Vector3.zero;

    [SerializeField] float globalProgressSpeed = 1f;
    [SerializeField] float globalImpactProgressSpeed = 1f;
    float globalProgress; //Used to monitor the fading out and shrinking effect when you stop shooting
    float globalImpactProgress; //Used to monitor the impact effect of the lazer when you first start shooting 

    [Tooltip("Affect all the child lazers' and effects' colors")]
    [SerializeField]
    protected bool colorizeAll = true;
    [SerializeField] Gradient colorRange;
    protected Color color;

    [Range(0.2f, 1.0f)]
    [SerializeField]
    protected float gammaLinear = 1f; //Not sure what this does

    public StartPointEffectControllerV3D startPointEffect;
    public EndPointEffectControllerV3D endPointEffect;
    public SmartWaveParticlesControllerV3D smartWaveParticles;

    LaserLineV3D[] laserLines;
    LightLineV3D[] lightLines;
    Renderer[] childRenderers;

    void Awake()
    {
        laserLines = GetComponentsInChildren<LaserLineV3D>(true);
        lightLines = GetComponentsInChildren<LightLineV3D>(true);
        childRenderers = GetComponentsInChildren<Renderer>(true);

        UpdateMaxLengths();
    }

    //This is called when the parent is initialized after being gotten from it's object pool, 
    //so it's essentially an initialization that resets the whole script
    public void initialize(Transform target)
    {
        globalProgress = 1f;
        globalImpactProgress = 1f;

        color = colorRange.Evaluate(Random.Range(0f, 1f));

        UpdateEffectManagers();
        UpdateLaserChildren();
        UpdateColorOfChildren();
    }

    virtual public void startShooting()
    {
        globalImpactProgress = 0f;
        GetComponentInChildren<SmartWaveParticlesControllerV3D>().SpawnWave();
        isShooting = true;
    }


    public void stopShooting()
    {
        isShooting = false;
    }

    public void setTargetPoint(Vector3 point)
    {
        targetPoint = point;
    }


    protected virtual void Update()
    {
        UpdateColorOfChildren();
        UpdateProgressVariables();
        UpdateEffectManagers();
        UpdateLaserChildren();

        // Control Gamma and Linear modes
        foreach (Renderer rend in childRenderers)
        {
            rend.material.SetFloat("_GammaLinear", gammaLinear);
        }

    }

    private void UpdateProgressVariables()
    {
        if (!isShooting)
            globalProgress = Mathf.Clamp01(globalProgress + Time.deltaTime * globalProgressSpeed);

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


    void UpdateMaxLengths()
    {
        if (!changeAllMaxLength) return;

        foreach (LaserLineV3D ll in laserLines)
            ll.maxLength = maxLength;

        foreach (LightLineV3D lil in lightLines)
            lil.maxLength = maxLength;
    }

    private void UpdateLaserChildren()
    {
        foreach (LaserLineV3D ll in laserLines)
        {
            ll.SetGlobalProgress(globalProgress);
            ll.SetGlobalImpactProgress(globalImpactProgress);
            ll.setTargetPoint(targetPoint);
        }

        foreach (LightLineV3D lil in lightLines)
        {
            lil.SetGlobalProgress(globalProgress);
            lil.SetGlobalImpactProgress(globalImpactProgress);
            lil.setTargetPoint(targetPoint);
        }
    }

    protected virtual void UpdateColorOfChildren()
    {
        if (colorizeAll)
        {
            startPointEffect.SetFinalColor(color);
            endPointEffect.SetFinalColor(color);

            foreach (LightLineV3D lil in lightLines)
            {
                lil.SetFinalColor(color);
            }

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
