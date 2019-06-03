using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using DG.Tweening;

[RequireComponent(typeof(demonFightScript))]
public class demonFightScript : MonoBehaviour
{

    [SerializeField] List<ParticleSystem> meteorParticleSystems = new List<ParticleSystem>();
    [SerializeField] GameObject meteor;
    [SerializeField] List<pathGenerator> possibleMeteorPaths = new List<pathGenerator>();
    [SerializeField] GameObject shield;
    [SerializeField] PlayerBulletLauncher playerBulletLauncher;
    [SerializeField] PlayerMegaBlastCoordinator megaBlastCorridnator;
    [SerializeField] bulletLauncher demonBulletLauncher;
    [SerializeField] PostProcessVolume metoerPP;
    [SerializeField] float PostProcessingTweenTime;
    
    bool meteorDestroyed;

    demonAnimationManager demonAnimationManager;


    void Awake(){
        foreach (ParticleSystem ps in meteorParticleSystems) ps.Stop(true);
        meteorDestroyer.meteorDestroyed += signalMeteorDestroyed;
        demonAnimationManager = GetComponent<demonAnimationManager>();
        StartCoroutine("bossFightGameLoop");
    }

    IEnumerator bossFightGameLoop(){
        endMeteorSetPiece(false);
        startShootingSetPiece();
        yield return new WaitUntil(()=> bulletStreakCounter.consecutiveBulletsDestroyed == 2);
        endShootingSetPiece();

        demonAnimationManager.meteorBeginningAnim();
        yield return new WaitUntil(()=> meteorDestroyed);
        endMeteorSetPiece();

        startShootingSetPiece();
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