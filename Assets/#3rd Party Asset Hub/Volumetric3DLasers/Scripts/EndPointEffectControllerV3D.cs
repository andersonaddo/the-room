using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The class in charge of the effects at the end of the lasers
/// </summary>
public class EndPointEffectControllerV3D : MonoBehaviour {

    [SerializeField] Light pointLight;
    [SerializeField] float pointLightRange = 10f;
    [SerializeField] float pointLightIntensity = 1f;
    [SerializeField] Color color;

    [SerializeField] AnimationCurve progressCurve;
    [SerializeField] AnimationCurve impactCurve;

    [SerializeField] LaserLineV3D endPointReferenceLaser;

    [Tooltip("Particle systems whose emittance is affected by progress")]
    [SerializeField]
    ParticleSystem[] emittingParticleSystems;
    [HideInInspector] public bool shouldEmitParticleSystems = false;
    private bool isEmittingParticles = false;


    [Tooltip("GameObjects whose emittance is affected by progress")]
    [SerializeField]
    GameObject[] scalingComponents;
    Vector3[] initialLocalScales;

    private Vector3 endPointPosition;
    private float globalProgress;
    private float globalResultProgress;
    private float globalImpactProgress;
    private float globalImpactResultProgress;
    private float resultProgress;

    void Start () {
        updateEmissionParticles();
        endPointPosition = endPointReferenceLaser.GetEndPointPosition();

        initialLocalScales = new Vector3[scalingComponents.Length];
        for (int i = 0; i < scalingComponents.Length; i++)
        {
            initialLocalScales[i] = scalingComponents[i].transform.localScale;
        }
    }

    void Update()
    {

        // Positioning End Point effect
        endPointPosition = endPointReferenceLaser.GetEndPointPosition();
        gameObject.transform.position = endPointPosition;

        if (isEmittingParticles != shouldEmitParticleSystems)
        {
            updateEmissionParticles();
            isEmittingParticles = shouldEmitParticleSystems;
        }

        UpdateProgressVariables();

        UpdatScalingGameObjects();

        UpdatePointLight();

    }

    // Recieving color from control script
    public void SetFinalColor(Color col)
    {
        color = col;
    }

    void updateEmissionParticles()
    {
        foreach (ParticleSystem ps in emittingParticleSystems)
        {
            var em = ps.emission;
            em.enabled = shouldEmitParticleSystems;
        }
    }

    // Recieving global progress from control script
    public void SetGlobalProgress(float gp)
    {
        globalProgress = gp;
    }

    // Recieving global impact progress from control script
    public void SetGlobalImpactProgress(float gp)
    {
        globalImpactProgress = gp;
    }

    private void UpdatePointLight()
    {
        // End Point Light Control
        if (pointLight != null)
        {
            pointLight.color = color;
            pointLight.range = transform.lossyScale.x * pointLightRange;
            pointLight.intensity = resultProgress * pointLightIntensity;
        }
    }

    private void UpdatScalingGameObjects()
    {
        // Scaling Particle Systems Control
        for (int i = 0; i < scalingComponents.Length; i++)
        {
            scalingComponents[i].transform.localScale = initialLocalScales[i] * resultProgress;
            if (resultProgress < 0.01f)
            {
                scalingComponents[i].gameObject.SetActive(false);
            }
            else
            {
                scalingComponents[i].gameObject.SetActive(true);
            }
        }
    }

    private void UpdateProgressVariables()
    {
        // Result Control
        globalImpactResultProgress = impactCurve.Evaluate(globalImpactProgress);
        if (globalImpactResultProgress == 0f) globalImpactResultProgress = 0.001f;

        globalResultProgress = progressCurve.Evaluate(globalProgress);
        resultProgress = globalImpactResultProgress + globalResultProgress;
    }
}
