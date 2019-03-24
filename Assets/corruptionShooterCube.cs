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

    public float baseAngularVelocity, angularVectorChangeRate, angularChangeSpeed;
    Vector3 angularVector, ranndomVector;
    float nextAngularChangeTime;

    bool canShoot, hasShot;
    public float shootChargeTime;
    float currentShootProg;
    Image progImage;

    Transform target;
    LineRenderer laser;


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
            if (currentShootProg == shootChargeTime) shoot();
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

    void shoot()
    {
        laser = GetComponentInChildren<LineRenderer>();
        hasShot = true;
        laser.SetPosition(1, (target.transform.position - laser.transform.position) / transform.localScale.x); //Assuming that the scale is uniform
        laser.enabled = true;
    }

    public void initialize() { /**Meh nothing here*/ }
}
