using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class corruptionShooterDrone : MonoBehaviour, IShootableCube
{

    public cubeTypes _type;
    public cubeTypes type
    {
        get { return _type; }
    }

    public iTween.EaseType launchEaseType;

    [SerializeField] float droneSpinningSpeed;
    [SerializeField] Transform droneTransform;
    [SerializeField] Collider hitBox;


    bool canShoot, hasShot, hasBeenShot;
    [SerializeField] float shootChargeTime;
    float currentShootProgress;
    Image progressDisplay;
    GameObject countingCanvas;

    Transform target;
    MyV3DLaserController laser;

    [SerializeField] float laserSpeed, aliveTimeAfterHit, shrinkTime, aliveTimeAfterExplosion;
    [SerializeField] GameObject dissappearPS, explosionPS;
    [SerializeField] int damageOnHit;
    [Tooltip("X = shake magnitude, Y = shake roughness")][SerializeField] Vector2 shake;

    void Awake()
    {
        laser = GetComponentInChildren<MyV3DLaserController>();
        progressDisplay = GetComponentInChildren<Image>();
        countingCanvas = GetComponentInChildren<Canvas>().gameObject;
    }

    public void initialize(Transform target) //Called the right before launch
    {
        this.target = target;
        laser.initialize(target);
    }

    void Update()
    {
        rotateDrone();
        lookAtTarget();

        if (canShoot && !hasShot && !hasBeenShot)
        {
            charge();
            if (currentShootProgress == shootChargeTime) StartCoroutine("shootAndDisappear");
        }

    }

    void lookAtTarget()
    {
        transform.LookAt(target);
    }

    void rotateDrone()
    {
        droneTransform.Rotate(Vector3.forward * droneSpinningSpeed * Time.deltaTime, Space.Self);
    }

    public void launch(Vector3 restPostion, float launchSpeed)
    {
        iTween.MoveTo(gameObject, iTween.Hash("position", restPostion,
                                              "easeType", launchEaseType,
                                              "onComplete", "enableShooting",
                                              "onCompleteTarget", gameObject,
                                              "speed", launchSpeed));
    }

    public void onShot(Vector3 shotPosition, damageEffectors damageEffector)
    {
        hasBeenShot = true;
        StopAllCoroutines(); //Incase the cube was already about to reset itself since it was finished shooting...
        StartCoroutine("Explode");
    }

    void enableShooting()
    {
        canShoot = true;
    }

    void charge()
    {
        currentShootProgress = Mathf.MoveTowards(currentShootProgress, shootChargeTime, Time.deltaTime);
        progressDisplay.fillAmount = currentShootProgress / shootChargeTime;
    }

    IEnumerator shootAndDisappear()
    {
        hasShot = true;
        laser.enabled = true;

        Vector3 laserTip = laser.transform.position; //Local Space
        laser.startShooting();

        while (laserTip != target.position)
        {
            laserTip = Vector3.MoveTowards(laserTip, target.position, laserSpeed * Time.deltaTime);
            laser.setTargetPoint(laserTip); //Assuming that the scale is uniform
            yield return null;
        }

        //Laser has reached player now...
        FindObjectOfType<playerDamager>().inflictDamage(damageOnHit, shake);

        //Done with shooting, now about to disappear
        yield return new WaitForSeconds(aliveTimeAfterHit);
        hitBox.enabled = false; //Cannot be hit anymore;
        displayDissapearEffect();
        laser.stopShooting();
        shrinkWithItween();
        yield return new WaitForSeconds(shrinkTime);
        resetForPooling();
    }

    void displayDissapearEffect()
    {
        dissappearPS.SetActive(true);
    }

    private void shrinkWithItween()
    {
        iTween.ScaleTo(droneTransform.gameObject, Vector3.zero, shrinkTime);
        iTween.ScaleTo(countingCanvas, Vector3.zero, shrinkTime);
        iTween.ScaleTo(dissappearPS, Vector3.zero, shrinkTime);
    }

    IEnumerator Explode()
    {
        hitBox.enabled = false; //Cannot be hit anymore
        displayExplosion();
        laser.stopShooting();
        droneTransform.gameObject.SetActive(false);
        yield return new WaitForSeconds(shrinkTime + 0.5f);
        resetForPooling();
    }

    void displayExplosion()
    {
        //Explosion is non-looping, needs to be reset
        explosionPS.SetActive(true);
        ParticleSystem ps = explosionPS.GetComponent<ParticleSystem>();
        ps.time = 0;
        ps.Play(true);
    }

    void resetForPooling()
    {
        StopAllCoroutines();
        hitBox.enabled = true;
        explosionPS.SetActive(false);
        dissappearPS.SetActive(false);
        droneTransform.gameObject.SetActive(true);

        canShoot = false;
        hasShot = false;
        hasBeenShot = false;

        transform.rotation = Quaternion.identity;
        currentShootProgress = 0;
        progressDisplay.fillAmount = 0;

        droneTransform.localScale = Vector3.one;
        dissappearPS.transform.localScale = Vector3.one;
        countingCanvas.transform.localScale = Vector3.one;

        objectPooler.Instance.returnObject("shooterDrone", gameObject);
    }

}
