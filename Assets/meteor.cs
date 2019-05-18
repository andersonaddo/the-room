using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meteor : MonoBehaviour
{
    [SerializeField] Transform rock;
    [SerializeField] pathGenerator pathGenerator;
    pathFollower follower;

    [SerializeField] float rockRotSpeed;
    Vector3 rockRotVector;

    public bool isBeingHit { get; private set; }
    [SerializeField] [Range(0,1)] float lerpLimit; //The lerp at which the comet will stop at is hit keeps being hit. Can't be stopped if is passes this
    float deceleration, originalSpeed;

    void Awake()
    {
        setUpRockRotation();
        follower = GetComponent<pathFollower>();
        follower.setPath(pathGenerator.bezierPath);
        originalSpeed = follower.speed;
    }

    void Update()
    {
        rotateRock();
    }

    void setUpRockRotation()
    {
        rock.rotation = UnityEngine.Random.rotation;
        rockRotVector = UnityEngine.Random.onUnitSphere;
    }

    void rotateRock()
    {
        rock.Rotate(rockRotVector * Time.deltaTime * rockRotSpeed, Space.World);
    }

    void FixedUpdate()
    {
        if (isBeingHit && follower.shouldMove) decelerate();
    }

    public void signalHitStart()
    {
        if (follower.currentLerp >= lerpLimit - 0.1f) return; //DOn't do anything if comet is too close to lerp limit
        calculateDeceration();
        isBeingHit = true;
    }

    public void signalHitEnd()
    {
        isBeingHit = false;
        follower.changeSpeed(originalSpeed, true);
    }

    void calculateDeceration()
    {
        //Based off the formula v^2 = u^2 + 2as
        float remainingDistance = (lerpLimit - follower.currentLerp) * follower.path.length;
        deceleration = -Mathf.Pow(follower.speed, 2) / (2 * remainingDistance);
    }

    void decelerate()
    {
        follower.changeSpeed(deceleration * Time.deltaTime);
        if (follower.speed <= 0)
        {
            follower.changeSpeed(0, true);
            follower.shouldMove = false; //Comet has been successfully stopped, don't let it move again.
        }
    }

}
