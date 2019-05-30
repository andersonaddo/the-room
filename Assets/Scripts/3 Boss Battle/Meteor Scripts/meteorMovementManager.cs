using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meteorMovementManager : MonoBehaviour
{
    meteorDamager damager;

    [SerializeField] Transform rock;
    public pathFollower follower {get; private set; }

    [SerializeField] float rockRotSpeed;
    Vector3 rockRotVector;

    public bool isBeingHit { get; private set; }
    [SerializeField] [Range(0,1)] float pathLerpLimit; //The path lerp at which the comet will stop at if it keeps being hit. Can't be stopped if is passes this
    float deceleration, originalSpeed;


    //This should be called immediately upon instantiation
    public void intitialize(pathGenerator pathGen)
    {
        setUpRockRotation();
        follower = GetComponent<pathFollower>();
        follower.setPath(pathGen.bezierPath);
        originalSpeed = follower.speed;
        damager = GetComponent<meteorDamager>();
        Invoke("destroyMeteorCompleteFail", follower.travelTime * 2.5f);
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
        if (follower.currentLerp >= pathLerpLimit - 0.1f) return; //DOn't do anything if comet is too close to lerp limit
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
        float remainingDistance = (pathLerpLimit - follower.currentLerp) * follower.path.length;
        deceleration = -Mathf.Pow(follower.speed, 2) / (2 * remainingDistance);
    }

    void decelerate()
    {
        follower.changeSpeed(deceleration * Time.deltaTime);
        if (follower.speed <= 0 && !damager.canBeDamaged)
        {
            transitionToDestructionMode();
        }
    }

    void destroyMeteorCompleteFail(){
        GetComponent<meteorDestroyer>().destroyMeteor(meteorDestroyer.metoerDestructionModes.completeFail);
    }

    void destroyMeteorUnsuccessful(){
        GetComponent<meteorDestroyer>().destroyMeteor(meteorDestroyer.metoerDestructionModes.stoppedButUnsuccessful);
    }

    private void transitionToDestructionMode()
    {
        follower.changeSpeed(0, true);
        follower.shouldMove = false; //Comet has been successfully stopped, don't let it move again.
        damager.enableDamage();
        FindObjectOfType<PlayerMegaBlastCoordinator>().setToDelayMode();
        CancelInvoke();
    }

    //Called by meteor reseters
    public void transitionBackToMovementMode(){
        damager.disableDamage();

        //Will ensure that the meteor will be destroyed 2 seconds after hitting the player
        Invoke("destroyMeteorUnsuccessful", follower.travelTime * (1-pathLerpLimit) + 2); 

        FindObjectOfType<PlayerMegaBlastCoordinator>().setToNormalMode();
        follower.changeSpeed(originalSpeed, true);
        follower.shouldMove = true;
    }
}
