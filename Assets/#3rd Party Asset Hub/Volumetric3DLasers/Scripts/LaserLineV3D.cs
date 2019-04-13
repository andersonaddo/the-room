using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The script for a conventional volumetric laser
/// </summary>
public class LaserLineV3D : MonoBehaviour
{
    public float maxLength = 1.0f;
    Vector3 targetPoint;

    [SerializeField] AnimationCurve shaderProgressCurve;
    [SerializeField] AnimationCurve shaderImpactCurve;

    [SerializeField] float explosionDistanceFromHit = 0.5f;
    [SerializeField] int particleMeshLength = 1;

    [SerializeField] bool scaleWithTransform = true;
    float scale = 1f;
    float scaleLastFrame;

    private float HitLength;
    private Vector3 endPoint;
    private ParticleSystem ps;

    private ParticleSystemRenderer psr;
    private Vector3 positionForExplosion;
    private Vector3[] particleSpawnPositions;
    private float globalProgress;
    private float globalimpactProgress;
    private ParticleSystem.Particle[] particles;
    private int positionArrayLength;

    private bool tempFix = false;


    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        psr = GetComponent<ParticleSystemRenderer>();
        HitLength = 0;
        FindEndPoint();
        LaserControl();
        UpdateLaserParts();
        tempFix = true;
    }

    void Update()
    {
        if (scaleWithTransform)
        {
            scale = gameObject.transform.lossyScale.x;
        }

        FindEndPoint();
        LaserControl();

        if (positionArrayLength != particles.Length || scaleLastFrame != scale)
        {
            UpdateLaserParts();
        }

        scaleLastFrame = scale;
    }

    void OnEnable()
    {
        if (tempFix == true)
        {
            UpdateLaserParts();
        }
    }


    // Updating and Fading
    void LaserControl() 
    {
        float progress = shaderProgressCurve.Evaluate(globalProgress);
        psr.material.SetFloat("_Progress", progress);
        float impactProgress = shaderImpactCurve.Evaluate(globalimpactProgress);
        psr.material.SetFloat("_ImpactProgress", impactProgress);
        psr.material.SetVector("_StartPosition", transform.position);
        psr.material.SetVector("_EndPosition", endPoint);
        psr.material.SetFloat("_Distance", HitLength);
        psr.material.SetFloat("_MaxDist", HitLength);
        psr.material.SetFloat("_FinalSize", scale);
    }


    void FindEndPoint()
    {
        float distanceToTarget = Vector3.Distance(transform.position, targetPoint);
        if (distanceToTarget <= maxLength)
        {
            HitLength = distanceToTarget;

            positionForExplosion = Vector3.MoveTowards(targetPoint, transform.position, explosionDistanceFromHit);
            positionArrayLength = Mathf.RoundToInt(distanceToTarget / (particleMeshLength * scale));

            if (positionArrayLength < distanceToTarget)
            {
                positionArrayLength += 1;
            }

            particleSpawnPositions = new Vector3[positionArrayLength];
            endPoint = targetPoint;
        }
        else
        {
            HitLength = maxLength;
            positionArrayLength = Mathf.RoundToInt(maxLength / (particleMeshLength * scale));
            if (positionArrayLength < maxLength)
            {
                positionArrayLength += 1;
            }
            particleSpawnPositions = new Vector3[positionArrayLength];

            endPoint = Vector3.MoveTowards(transform.position, transform.forward * 1000f, maxLength);
            positionForExplosion = endPoint;
        }
    }


    // Updating Laser parts positions and length
    void UpdateLaserParts()
    {
        particles = new ParticleSystem.Particle[positionArrayLength];

        for (int i = 0; i < positionArrayLength; i++)
        {
            particleSpawnPositions[i] = new Vector3(0f, 0f, i*particleMeshLength * scale);
            particles[i].position = particleSpawnPositions[i];
            particles[i].startSize = scale;
            particles[i].startColor = new Color(1f, 1f, 1f);
        }

        ps.SetParticles(particles, particles.Length);

    }

    // Recieving global progress from control script
    public void SetGlobalProgress(float gp)
    {
        globalProgress = gp;
    }

    // Recieving global impact progress from control script
    public void SetGlobalImpactProgress(float gp)
    {
        globalimpactProgress = gp;
    }

    // Giving end position to endPointEffectController
    public Vector3 GetEndPointPosition()
    {
        return positionForExplosion;
    }

    public void setTargetPoint(Vector3 point)
    {
        targetPoint = point; 
    }

}
