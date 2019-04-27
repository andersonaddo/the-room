using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class corruptionShooterDrone : MonoBehaviour, IShootableCube
{

    public cubeTypes _type;
    public cubeTypes type
    {
        get { return _type; }
    }

    public Ease launchEaseType;

    [SerializeField] float droneSpinningSpeed;
    [SerializeField] Transform droneTransform;
    [SerializeField] Collider hitBox;


    bool canShoot, hasShot, hasBeenShot;
    [SerializeField] float shootChargeTime;

    float currentShootProgress;
    Image progressDisplay;
    GameObject progressCanvas;

    Vector3 originalCanvasScale, originalDroneScale, originalDissapearPSScale;

    Transform target;
    MyV3DLaserController laser;

    [SerializeField] float laserSpeed, laserAliveTime, aliveTimeAfterHit, shrinkTime;
    [SerializeField] GameObject dissappearPS, explosionPS;
    [SerializeField] int damageOnHit;
    [Tooltip("X = shake magnitude, Y = shake roughness")][SerializeField] Vector2 shake;

    void Awake()
    {
        laser = GetComponentInChildren<MyV3DLaserController>();
        progressDisplay = GetComponentInChildren<Image>();
        progressCanvas = GetComponentInChildren<Canvas>().gameObject;

        originalCanvasScale = progressCanvas.transform.localScale;
        originalDroneScale = droneTransform.localScale;
        originalDissapearPSScale = dissappearPS.transform.localScale;
    }

    public void initialize(Transform target) //Called the right before launch
    {
        this.target = target;
        laser.initialize(target);
    }

    public void launch(Vector3 restPostion, float launchSpeed)
    {
        transform.DOMove(restPostion, launchSpeed)
            .SetSpeedBased()
            .SetEase(launchEaseType)
            .OnComplete(enableShooting);
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

        Vector3 laserTip = laser.transform.position; 
        laser.startShooting();

        while (laserTip != target.position)
        {
            laserTip = Vector3.MoveTowards(laserTip, target.position, laserSpeed * Time.deltaTime);
            laser.setTargetPoint(laserTip); //Assuming that the scale is uniform
            yield return null;
        }

        //Laser has reached player now...
        FindObjectOfType<playerDamager>().inflictDamage(damageOnHit, shake);

        yield return new WaitForSeconds(laserAliveTime);
        laser.stopShooting();

        //Done with shooting, now about to disappear
        yield return new WaitForSeconds(aliveTimeAfterHit - laserAliveTime);
        hitBox.enabled = false; //Cannot be hit anymore;
        displayDissapearEffect();
        shrinkWithTween();
        yield return new WaitForSeconds(shrinkTime);
        resetForPooling();
    }

    void displayDissapearEffect()
    {
        dissappearPS.SetActive(true);
    }

    private void shrinkWithTween()
    {
        droneTransform.DOScale(Vector3.zero, shrinkTime);
        progressCanvas.transform.DOScale(Vector3.zero, shrinkTime);
        dissappearPS.transform.DOScale(Vector3.zero, shrinkTime);
    }

    IEnumerator Explode()
    {
        hitBox.enabled = false; //Cannot be hit anymore
        progressDisplay.enabled = false;
        displayExplosion();
        laser.stopShooting();
        droneTransform.gameObject.SetActive(false);
        yield return new WaitForSeconds(shrinkTime + 0.5f); //0.5f added to give tweens time to kill themselves
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
        progressDisplay.enabled = true;
        explosionPS.SetActive(false);
        dissappearPS.SetActive(false);
        droneTransform.gameObject.SetActive(true);

        canShoot = false;
        hasShot = false;
        hasBeenShot = false;

        transform.rotation = Quaternion.identity;
        currentShootProgress = 0;
        progressDisplay.fillAmount = 0;

        droneTransform.localScale = originalDroneScale;
        dissappearPS.transform.localScale = originalDissapearPSScale;
        progressCanvas.transform.localScale = originalCanvasScale;

        objectPooler.Instance.returnObject("shooterDrone", gameObject);
    }

}
