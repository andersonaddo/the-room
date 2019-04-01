using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class corruptionShooterCube : MonoBehaviour, IShootableCube
{

    public cubeTypes _type;
    public cubeTypes type
    {
        get { return _type; }
    }

    public iTween.EaseType launchEaseType;

    [SerializeField] float baseAngularVelocity, angularVectorChangeRate, angularChangeSpeed;
    Vector3 angularVector, ranndomVector;
    float nextAngularChangeTime;

    bool canShoot, hasShot;
    [SerializeField] float shootChargeTime;
    float currentShootProg;
    Image progImage;

    Transform target;
    LineRenderer laser;
    [SerializeField] float laserSpeed, aliveTimeAfterHit;
    [SerializeField] int damageOnHit;

    void Awake()
    {
        angularVector = Random.onUnitSphere;
        ranndomVector = Random.onUnitSphere;
        nextAngularChangeTime = Time.time + angularVectorChangeRate;

        progImage = GetComponentInChildren<Image>();
    }

    void Update()
    {
        rotate();

        if (canShoot && !hasShot)
        {
            charge();
            if (currentShootProg == shootChargeTime) StartCoroutine("shoot");
        }

    }

    public void launch(Vector3 restPostion, Transform target, float launchSpeed)
    {
        this.target = target;
        iTween.MoveTo(gameObject, iTween.Hash("position", restPostion,
                                              "easeType", launchEaseType,
                                              "onComplete", "enableShooting",
                                              "onCompleteTarget", gameObject,
                                              "speed", launchSpeed));
    }

    void enableShooting()
    {
        canShoot = true;
    }

    public void onShot(Vector3 shotPosition, damageEffectors damageEffector)
    {
        throw new System.NotImplementedException();
    }


    void rotate()
    {
        transform.Rotate(angularVector * baseAngularVelocity * Time.deltaTime);
        angularVector = Vector3.MoveTowards(angularVector, ranndomVector, angularChangeSpeed * Time.deltaTime).normalized;

        if (Time.time >= nextAngularChangeTime)
        {
            ranndomVector = Random.onUnitSphere;
            nextAngularChangeTime = Time.time + angularVectorChangeRate;
        }
    }

    void charge()
    {
        currentShootProg = Mathf.MoveTowards(currentShootProg, shootChargeTime, Time.deltaTime);
        progImage.fillAmount = currentShootProg / shootChargeTime;
    }

    IEnumerator shoot()
    {
        laser = GetComponentInChildren<LineRenderer>();
        hasShot = true;
        laser.enabled = true;

        Vector3 laserTip = Vector3.zero; //Local Space
        Vector3 targetPosition = (target.transform.position - laser.transform.position) / transform.localScale.x;

        while (laserTip != targetPosition)
        {
            laserTip = Vector3.MoveTowards(laserTip, targetPosition, laserSpeed * Time.deltaTime);
            laser.SetPosition(1, laserTip); //Assuming that the scale is uniform
            yield return null;
        }

        //Laser has reached player now...
        FindObjectOfType<playerDamager>().inflictDamage(damageOnHit);

        Destroy(gameObject, aliveTimeAfterHit);

    }

    public void initialize() { /**Meh nothing here*/ }
}
