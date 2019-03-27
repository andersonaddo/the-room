using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardboardGunPointer : GvrBasePointer
{
    public float maxShootingDistance;
    public GameObject bullet;
    bool canShoot = true;

    public override float MaxPointerDistance
    {
        get
        {
            return maxShootingDistance;
        }
    }

    public override void GetPointerRadius(out float enterRadius, out float exitRadius)
    {
        //Setting both of these to 0 makes the ray of the run just an actual ray rather than a cylinder
        enterRadius = 0;
        exitRadius = 0;
    }

    public override void OnPointerClickDown()
    {
        GameObject newBullet = objectPooler.Instance.requestObject("bullet");
        newBullet.transform.position = transform.position;
        newBullet.GetComponent<playerBullet>().launch(transform.forward);
    }

    public override void OnPointerClickUp()
    {
        
    }

    //Things are marked as interactive if they have an event system on them
    public override void OnPointerEnter(RaycastResult raycastResult, bool isInteractive)
    {
    }

    public override void OnPointerExit(GameObject previousObject)
    {
        
    }

    public override void OnPointerHover(RaycastResult raycastResult, bool isInteractive)
    {
        
    }
}
