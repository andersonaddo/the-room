using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

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
    
    bool meteorDestroyed;


    void Awake(){
        foreach (ParticleSystem ps in meteorParticleSystems) ps.Stop(true);
        meteorDestroyer.meteorDestroyed += signalMeteorDestroyed;
        StartCoroutine("bossFightGameLoop");
    }

    void startMeteorSetPiece(){
        meteorDestroyed = false;
        foreach (ParticleSystem ps in meteorParticleSystems) ps.Play(true);
        megaBlastCorridnator.enableShooting();
        metoerPP.weight = 1;
        launchMeteor();
    }

    void endMeteorSetPiece(){
        metoerPP.weight = 0;
        foreach (ParticleSystem ps in meteorParticleSystems) ps.Stop(true);
        megaBlastCorridnator.disableShooting();
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

    void launchMeteor()
    {
        GameObject newMetoer = Instantiate(meteor, Vector3.zero, Quaternion.identity);
        pathGenerator randomPathGen = possibleMeteorPaths[UnityEngine.Random.Range(0,possibleMeteorPaths.Count)];
        newMetoer.GetComponent<meteorMovementManager>().intitialize(randomPathGen);
    }

    void signalMeteorDestroyed(meteorDestroyer.metoerDestructionModes mode){
        meteorDestroyed = true;
    }

    IEnumerator bossFightGameLoop(){
        endMeteorSetPiece();
        startShootingSetPiece();
        yield return new WaitUntil(()=> bulletStreakCounter.consecutiveBulletsDestroyed == 5);
        endShootingSetPiece();

        startMeteorSetPiece();
        yield return new WaitUntil(()=> meteorDestroyed);
        endMeteorSetPiece();

        startShootingSetPiece();
    }
}
