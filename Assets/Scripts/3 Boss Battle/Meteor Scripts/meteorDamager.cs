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

    [SerializeField] GameObject meteorRestarter;
    GameObject createdRestarter;


    void Awake()
    {
        crackMaterial = rockRenderer.materials[0];
        crackMaterial.color = new Color(0, 0, 0, 0);
    }

    public void enableDamage()
    {
        canBeDamaged = true;
        crackMaterial.color = Color.black;

        float restarterTravelTime = meteorRestarter.GetComponent<pathFollower>().travelTime;
        float restarterLaunchWaitTime = postStopAliveTime - restarterTravelTime;
        if (restarterLaunchWaitTime <= 0) Debug.LogError("restarterLaunchWaitTime not positive");
        Invoke("launchMeteorReset", restarterLaunchWaitTime);
    }

    public void disableDamage()
    {
        canBeDamaged = false;
        CancelInvoke();
        if (createdRestarter != null) Destroy(createdRestarter);
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

        if (createdRestarter != null) Destroy(createdRestarter);
        CancelInvoke(); 

        foreach (Rigidbody rb in destroyedRock.GetComponentsInChildren<Rigidbody>())
            rb.AddExplosionForce(explosionForce, destroyedRock.transform.position, explosionRadius);

        Invoke("destroyMeteorSuccessful", postExplosionAliveTime);
    }

    void destroyMeteorSuccessful(){
        GetComponent<meteorDestroyer>().destroyMeteor(meteorDestroyer.metoerDestructionModes.successful);
    }

    void launchMeteorReset(){
        createdRestarter = Instantiate(meteorRestarter, Vector3.zero, Quaternion.identity);
        createdRestarter.GetComponent<pathFollower>().setPath(GetComponent<meteorMovementManager>().follower.path);
    }
}
