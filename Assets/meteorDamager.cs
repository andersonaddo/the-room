using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meteorDamager : MonoBehaviour
{

    bool canBeDamaged, isDestoryed;
    [SerializeField] int maxNumberOfClicks;
    int currentNumberOfClicks;

    [SerializeField] float postStopAliveTime;


    [SerializeField] float startIntensity;
    [SerializeField] float maxIntensity;
    float targetIntensity;
    float currentIntensity;
    [SerializeField] [ColorUsage(false, true)] Color baseColor;
    [SerializeField] [Range(0, 1)] float colorLerpSpeed;

    [SerializeField] Renderer rockRenderer;
    Material crackMaterial;
    [SerializeField] GameObject destroyedRock;
    [SerializeField] float explosionForce, explosionRadius;


    void Awake()
    {
        crackMaterial = rockRenderer.materials[0];
        crackMaterial.color = new Color(0, 0, 0, 0);
        currentIntensity = startIntensity;
    }

    void Update()
    {
        currentIntensity = Mathf.Lerp(currentIntensity, targetIntensity, colorLerpSpeed * Time.deltaTime);
        crackMaterial.SetColor("_EmissionColor", baseColor * currentIntensity);
    }

    public void enableDamage()
    {
        canBeDamaged = true;
        crackMaterial.color = Color.black;
    }

    public void signalHit()
    {
        if (!canBeDamaged || isDestoryed) return;
        currentNumberOfClicks++;
        if (maxNumberOfClicks == currentNumberOfClicks)
        {
            isDestoryed = true;
            destroyedRock.SetActive(true);
            rockRenderer.gameObject.SetActive(false);

            foreach (Rigidbody rb in destroyedRock.GetComponentsInChildren<Rigidbody>())
                rb.AddExplosionForce(explosionForce, destroyedRock.transform.position, explosionRadius);
        }
        else
        {
            targetIntensity = ((maxIntensity - startIntensity) / maxNumberOfClicks) * currentNumberOfClicks;
            targetIntensity += startIntensity;
        }
    }
}
