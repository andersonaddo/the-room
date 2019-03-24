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

    public float baseAngularVelocity, angularVectorChangeRate, angularChangeSpeed;
    Vector3 angularVector, ranndomVector;
    float nextAngularChangeTime;

    bool canShoot, hasShot;
    public float shootWaitTime;
    float currentShootProg;
    Image progImage;

    public Transform target;


    void Awake()
    {
        angularVector = Random.onUnitSphere;
        ranndomVector = Random.onUnitSphere;
        nextAngularChangeTime = Time.time + angularVectorChangeRate;

        progImage = GetComponentInChildren<Image>();
        canShoot = true;
    }

    void Update()
    {
        rotate();

        if (canShoot && !hasShot)
        {
            charge();
            if (currentShootProg == shootWaitTime) shoot();
        }

    }

    public void initialize()
    {
        throw new System.NotImplementedException();
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
        currentShootProg = Mathf.MoveTowards(currentShootProg, shootWaitTime, Time.deltaTime);
        progImage.fillAmount = currentShootProg / shootWaitTime;
    }

    void shoot()
    {
        hasShot = true;
        Debug.DrawLine(transform.position, target.position, Color.cyan, 5f);
    }
}
