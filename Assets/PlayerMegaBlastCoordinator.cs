﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class PlayerMegaBlastCoordinator : MonoBehaviour
{
    [SerializeField] bool _canShoot = true; //To allow for it to stay encapsulated but still changeable in the inspector
    public bool canShoot{ get{return _canShoot;} }
    bool isShooting;
    [SerializeField] string meteorLayerName;
    bool raycastOnMeteor;
    meteorMovementManager currentMeteor;

    public PlayerEnergyBlast rightLaser, leftLaser;
    Vector3 targetPoint;

    //When meteors are at that stage where the players need to tap a lot to make 
    //Metoer explode, then the lazers shouldn't go off immediately when the player lets
    //go of the trigger
    public bool isOnDelayMode {get; private set;}
    [SerializeField] float delayModeTimeDelay;

    void Awake()
    {
        updateLaserTargetPoints();

        PlayerCardboardPointer.pointerClickDown += signalHit;
        PlayerCardboardPointer.pointerClickUp += signalHitEnd;
        PlayerCardboardPointer.pointerEnter += onPointerEnter;
        PlayerCardboardPointer.pointerExit += onPointerExit;

        rightLaser.initialize();
        leftLaser.initialize();
    }

    void Update()
    {
        if (_canShoot) updateLaserTargetPoints();
    }

    public void reset(){
        isOnDelayMode = false;
        currentMeteor = null;
        isShooting = false;
        stopLasers();
        CancelInvoke();
    }

    void updateLaserTargetPoints()
    {
        if (PlayerCardboardPointer.isOnObject) targetPoint = PlayerCardboardPointer.raycast.worldPosition;
        else targetPoint = PlayerCardboardPointer.position + PlayerCardboardPointer.forwardDirection * PlayerCardboardPointer.maxRaycastingDistance;

        rightLaser.setTargetPoint(targetPoint);
        rightLaser.transform.LookAt(targetPoint); //Looks like the lazers can only shoot forward. I have to rotate them to actually look at the target.
        leftLaser.setTargetPoint(targetPoint);
        leftLaser.transform.LookAt(targetPoint);
    }

    void signalHit()
    {
        if (!_canShoot) return;
        if (!isShooting) startShooting();
        CancelInvoke("stopLasers");
        if (currentMeteor != null) currentMeteor.GetComponent<meteorDamager>().signalHit();
    }

    void startShooting(){
        isShooting = true;
        rightLaser.startShooting();
        leftLaser.startShooting();
    }

    void signalHitEnd()
    {
        if (currentMeteor != null) currentMeteor.signalHitEnd();
        if (isOnDelayMode) {
            Invoke("stopLasers", delayModeTimeDelay);
        }else{
            stopLasers();
        }
    }

    void stopLasers(){
        isShooting = false;
        rightLaser.stopShooting();
        leftLaser.stopShooting();
    }

    void onPointerEnter(RaycastResult result)
    {
        raycastOnMeteor = result.gameObject.layer == LayerMask.NameToLayer(meteorLayerName);

        if (raycastOnMeteor && isShooting)
        {
            currentMeteor = result.gameObject.GetComponentInParent<meteorMovementManager>();
            currentMeteor.signalHitStart();
        }
    }

    void onPointerExit(GameObject go)
    {
        if (currentMeteor == null) return;

        if (go == currentMeteor.gameObject)
        {
            currentMeteor.signalHitEnd();
            currentMeteor = null;
        }
    }

    public void setToDelayMode(){
        isOnDelayMode = true;
    }

    public void enableShooting(){
        _canShoot = true;
    }

    public void disableShooting(){
        _canShoot = false;
    }
}
