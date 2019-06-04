using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using DG.Tweening;

/// <summary>
/// This class manages the general game loop involved when fighting the skeleton.
/// </summary>
[RequireComponent(typeof(demonFightScript))]
public class demonFightScript : MonoBehaviour
{

    demonAnimationManager demonAnimationManager;
    demonDamageScript damageManager;

   
    [SerializeField] GameObject meteor;
    bool meteorDestroyed;
    [SerializeField] List<ParticleSystem> meteorParticleSystems = new List<ParticleSystem>();
    [SerializeField] List<pathGenerator> possibleMeteorPaths = new List<pathGenerator>();
    [SerializeField] PostProcessVolume metoerPP;
    [SerializeField] float PostProcessingTweenTime;


    [SerializeField] GameObject shield;
    [SerializeField] PlayerBulletLauncher playerBulletLauncher;
    [SerializeField] PlayerMegaBlastCoordinator megaBlastCorridnator;
    [SerializeField] bulletLauncher demonBulletLauncher;
    
    meteorDestroyer.metoerDestructionModes lastDestructionMode;


    void Start(){
        foreach (ParticleSystem ps in meteorParticleSystems) ps.Stop(true);
        meteorDestroyer.meteorDestroyed += signalMeteorDestroyed;
        demonAnimationManager = GetComponent<demonAnimationManager>();
        damageManager = GetComponent<demonDamageScript>();
        StartCoroutine("bossFightGameLoop");
    }

    IEnumerator bossFightGameLoop(){
        endMeteorSetPiece(false);
        startShootingSetPiece();
        yield return new WaitUntil(()=> bulletStreakCounter.consecutiveBulletsDestroyed == 4);
        endShootingSetPiece();

        demonAnimationManager.meteorBeginningAnim();
        yield return new WaitUntil(()=> meteorDestroyed);

        if (lastDestructionMode == meteorDestroyer.metoerDestructionModes.successful){
            endMeteorSetPiece(false);
            damageManager.enableDamage();
            playerBulletLauncher.enableShooting();
            float waitTime = Time.time + 10;
            yield return new WaitUntil(()=> damageManager.isDead || Time.time >= waitTime);
            if (!damageManager.isDead){
                damageManager.disableDamage();
                startShootingSetPiece();
            } 

        }else{
            endMeteorSetPiece();
            startShootingSetPiece();
        }
    }



    //There's no need for a "startMeteorSetPiece method bacause the animations called
    //in the demonAnimationManager.meteorBeginningAnim() have that functionality

    void endMeteorSetPiece(bool shouldAnimate = true){
        DOTween.To(() => metoerPP.weight, x => metoerPP.weight = x, 0, PostProcessingTweenTime);
        foreach (ParticleSystem ps in meteorParticleSystems) ps.Stop(true);
        megaBlastCorridnator.disableShooting();
        if (shouldAnimate) demonAnimationManager.meteorEndAnim();
    }

    void startShootingSetPiece(){
        bulletStreakCounter.reset();
        demonBulletLauncher.enableShooting();
        playerBulletLauncher.enableShooting();
        shield.SetActive(true);
    }

    void endShootingSetPiece(){
        demonBulletLauncher.disableShooting();
        demonBulletLauncher.destroyAllBullets();
        playerBulletLauncher.disableShooting();
        shield.SetActive(false);       
    }

    void signalMeteorDestroyed(meteorDestroyer.metoerDestructionModes mode){
        meteorDestroyed = true;
        lastDestructionMode = mode;
    }


#region Animation-Triggered Methods

    public void launchMeteor()
    {
        GameObject newMetoer = Instantiate(meteor, Vector3.zero, Quaternion.identity);
        pathGenerator randomPathGen = possibleMeteorPaths[UnityEngine.Random.Range(0,possibleMeteorPaths.Count)];
        newMetoer.GetComponent<meteorMovementManager>().intitialize(randomPathGen);
    }

    
    public void triggerMeteorRain(){
        meteorDestroyed = false;
        foreach (ParticleSystem ps in meteorParticleSystems) ps.Play(true);
        megaBlastCorridnator.enableShooting();
        DOTween.To(() => metoerPP.weight, x => metoerPP.weight = x, 1, PostProcessingTweenTime);
    }

#endregion

}