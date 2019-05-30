using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//There should be only one of these in the scene
public class PlayerCardboardPointer : GvrBasePointer
{
    public float _maxRaycastingDistance = 80;

    public static float maxRaycastingDistance;
    public static Vector3 forwardDirection, position;
    public static RaycastResult raycast;
    public static bool isOnObject;
    public static event System.Action pointerClickDown, pointerClickUp;
    public static event System.Action<RaycastResult> pointerHover, pointerEnter;
    public static event System.Action<GameObject> pointerExit;


    void Awake()
    {
        maxRaycastingDistance = _maxRaycastingDistance;
    }

    void Update()
    {
        forwardDirection = transform.forward;
        position = transform.position;
    }

    public override float MaxPointerDistance
    {
        get
        {
            return maxRaycastingDistance;
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
        if (pointerClickDown != null) pointerClickDown();
    }

    public override void OnPointerClickUp()
    {
        if (pointerClickUp != null) pointerClickUp();
    }

    //Things are marked as interactive if they have an event system on them
    //The layer mask for the Raycast results can be altered by alertering the GvrPointerPhysicsRaycaster in the scene
    public override void OnPointerEnter(RaycastResult raycastResult, bool isInteractive)
    {
        raycast = raycastResult;
        isOnObject = true;
        if (pointerEnter != null) pointerEnter(raycastResult);
    }

    public override void OnPointerHover(RaycastResult raycastResult, bool isInteractive)
    {
        raycast = raycastResult;
        isOnObject = true;
        if (pointerHover != null) pointerHover(raycastResult);
    }

    public override void OnPointerExit(GameObject previousObject)
    {
        if (pointerExit != null) pointerExit(previousObject);
        isOnObject = false;
    }
}
