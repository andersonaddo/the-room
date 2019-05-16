using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The class in charge of the effects at the start of the lasers
/// </summary>
public class StartPointEffectControllerV3D : MonoBehaviour
{

    [SerializeField] Light pointLight;
    [SerializeField] float pointLightRange = 10f;
    [SerializeField] float pointLightIntensity = 1f;

    [Tooltip("Particle systems whose scale is affected by progress")]
    [SerializeField] ParticleSystem[] scalingParticleSystems;
    private Vector3[] initialLocalScales;

    [Tooltip("Particle systems whose emittance is affected by progress")]
    [SerializeField] ParticleSystem[] emittingParticleSystems;

    bool emitStartEffectParticles = false;

    [SerializeField] AnimationCurve progressCurve;
    [SerializeField] AnimationCurve impactCurve;
    [SerializeField] Color color;

    private float globalProgress;
    private float globalResultProgress;
    private float globalImpactProgress;
    private float globalImpactResultProgress;
    private float resultProgress;

    private void Start()
    {
        initializeLocalScales();
    }

    void Update()
    {
        UpdateProgressVariables();
        UpdateEmittingParticles();
        UpdateScalingParticles();
        UpdatePointLight();
    }

    private void UpdatePointLight()
    {
        // Start Point Light Control
        if (pointLight == null) return;
        pointLight.color = color;
        pointLight.range = transform.lossyScale.x * pointLightRange;
        pointLight.intensity = resultProgress * pointLightIntensity;
    }

    private void UpdateScalingParticles()
    {
        // Scaling Particle Systems Control
        for (int i = 0; i < scalingParticleSystems.Length; i++)
        {
            scalingParticleSystems[i].transform.localScale = initialLocalScales[i] * resultProgress;
            if (resultProgress < 0.01f)
            {
                scalingParticleSystems[i].gameObject.SetActive(false);
            }
            else
            {
                scalingParticleSystems[i].gameObject.SetActive(true);
            }
        }
    }

    private void UpdateEmittingParticles()
    {
        emitStartEffectParticles = resultProgress > 0.9;
        if (emitStartEffectParticles)
        {
            foreach (ParticleSystem ps in emittingParticleSystems)
            {
                var em = ps.emission;
                em.enabled = true;
            }
        }
        else
        {
            foreach (ParticleSystem ps in emittingParticleSystems)
            {
                var em = ps.emission;
                em.enabled = false;
            }
        }

    }

    private void UpdateProgressVariables()
    {
        // Result Control
        globalImpactResultProgress = impactCurve.Evaluate(globalImpactProgress);
        if (globalImpactResultProgress == 0f)
        {
            globalImpactResultProgress = 0.001f;
        }

        globalResultProgress = progressCurve.Evaluate(globalProgress);
        resultProgress = globalImpactResultProgress + globalResultProgress;
    }

    private void initializeLocalScales()
    {
        initialLocalScales = new Vector3[scalingParticleSystems.Length];
        for (int i = 0; i < scalingParticleSystems.Length; i++)
        {
            initialLocalScales[i] = scalingParticleSystems[i].transform.localScale;
        }
    }

    public void SetGlobalProgress(float gp)
    {
        globalProgress = gp;
    }

    public void SetFinalColor(Color col)
    {
        color = col;
    }

    public void SetGlobalImpactProgress(float gp)
    {
        globalImpactProgress = gp;
    }
}
