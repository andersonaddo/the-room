using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class PlayerMegaBlastCoordinator : MonoBehaviour
{
    public bool canShoot = true;
    bool isShooting;
    [SerializeField] string meteorLayerName;
    bool raycastOnMeteor;
    meteor currentMeteor;

    public PlayerEnergyBlast rightLaser, leftLaser;
    Vector3 targetPoint;


    void Awake()
    {
        updateLaserTargetPoints();

        PlayerCardboardPointer.pointerClickDown += startShooting;
        PlayerCardboardPointer.pointerClickUp += stopShooting;
        PlayerCardboardPointer.pointerEnter += onPointerEnter;
        PlayerCardboardPointer.pointerExit += onPointerExit;

        rightLaser.initialize();
        leftLaser.initialize();
    }

    void Update()
    {
        if (canShoot) updateLaserTargetPoints();
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

    void startShooting()
    {
        if (!canShoot) return;
        isShooting = true;
        rightLaser.startShooting();
        leftLaser.startShooting();

        if (currentMeteor != null) currentMeteor.GetComponent<meteorDamager>().signalHit();
    }

    void stopShooting()
    {
        isShooting = false;
        if (currentMeteor != null) currentMeteor.signalHitEnd();
        rightLaser.stopShooting();
        leftLaser.stopShooting();
    }

    void onPointerEnter(RaycastResult result)
    {
        raycastOnMeteor = result.gameObject.layer == LayerMask.NameToLayer(meteorLayerName);

        if (raycastOnMeteor && isShooting)
        {
            currentMeteor = result.gameObject.GetComponentInParent<meteor>();
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
}
