using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meteorDamager : MonoBehaviour
{

    public bool canBeDamaged {get; private set;}
    bool isDestroyed;
    [SerializeField] int maxNumberOfClicks;
    int currentNumberOfClicks;

    [SerializeField] float postStopAliveTime, postExplosionAliveTime;

    [SerializeField] float startIntensity;
    [SerializeField] float maxIntensity;
    float targetIntensity;
    [SerializeField] [ColorUsage(false, true)] Color baseColor;

    [SerializeField] Renderer rockRenderer;
    Material crackMaterial;
    [SerializeField] GameObject destroyedRock;
    [SerializeField] float explosionForce, explosionRadius;


    void Awake()
    {
        crackMaterial = rockRenderer.materials[0];
        crackMaterial.color = new Color(0, 0, 0, 0);
    }

    public void enableDamage()
    {
        canBeDamaged = true;
        crackMaterial.color = Color.black;
        Invoke("destroyMeteorUnsuccessful", postStopAliveTime);
    }

    public void signalHit()
    {
        if (!canBeDamaged || isDestroyed) return;
        currentNumberOfClicks++;
        if (maxNumberOfClicks == currentNumberOfClicks)
        {
            explode();
        }
        else
        {
            updateCrackColor();
        }
    }

    private void updateCrackColor()
    {
        targetIntensity = ((maxIntensity - startIntensity) / maxNumberOfClicks) * currentNumberOfClicks;
        targetIntensity += startIntensity;
        crackMaterial.SetColor("_EmissionColor", baseColor * targetIntensity);
    }

    void explode()
    {
        isDestroyed = true;
        destroyedRock.SetActive(true);
        rockRenderer.gameObject.SetActive(false);
        canBeDamaged = false;

        CancelInvoke(); 

        foreach (Rigidbody rb in destroyedRock.GetComponentsInChildren<Rigidbody>())
            rb.AddExplosionForce(explosionForce, destroyedRock.transform.position, explosionRadius);

        Invoke("destroyMeteorSuccessful", postExplosionAliveTime);
    }

    void destroyMeteorUnsuccessful(){
        GetComponent<meteorDestroyer>().destroyMeteor(meteorDestroyer.metoerDestructionModes.stoppedButUnsuccessful);
    }

    void destroyMeteorSuccessful(){
        GetComponent<meteorDestroyer>().destroyMeteor(meteorDestroyer.metoerDestructionModes.successful);
    }
}
