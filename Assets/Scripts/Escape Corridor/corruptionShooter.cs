using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class corruptionShooter : MonoBehaviour
{
    public Transform player;
    CorruptionShootingInfo shootingInfoHolder;

    [SerializeField] GameObject shooterCube, ricochetCube;


    [SerializeField] float shooterCubeWaitTimeMin, shooterCubeWaitTimeMax;
    [SerializeField] float richochetCubeWaitTimeMin, richochetCubeWaitTimeMax;

    //The following values should all be out of 100
    [SerializeField] int ricochetHitChance, ricochetNearmissChance; //nearmissChance of 100 means all richochet cubes that don't hit (that's based off ricochetHitChance chance) will nearmiss;
    [SerializeField] int earlyCubeChance; //The change that any cube will be released early than planned (jsut to add some unpredictability)

    bool canShoot = true;

    void Start()
    {
        shootingInfoHolder = GetComponentInChildren<CorruptionShootingInfo>();
        StartCoroutine("shootRicoshetCubes");
        StartCoroutine("shootShooterCubes");
    }

    IEnumerator shootShooterCubes()
    {
        while (canShoot)
        {
            float timeToWait = Random.Range(shooterCubeWaitTimeMin, shooterCubeWaitTimeMax);
            if (Random.Range(1, 101) <= earlyCubeChance) timeToWait /= 2;
            yield return new WaitForSeconds(timeToWait);

            if (!canShoot) break; //In case something has happened while we were waiting

            launchShooterCube();
        }
    }

    IEnumerator shootRicoshetCubes()
    {
        while (canShoot)
        {
            float timeToWait = Random.Range(richochetCubeWaitTimeMin, richochetCubeWaitTimeMax);
            if (Random.Range(1, 101) <= earlyCubeChance) timeToWait /= 2;
            yield return new WaitForSeconds(timeToWait);

            if (!canShoot) break; //In case something has happened while we were waiting

            
            if (Random.Range(0, 101) <= ricochetHitChance)
                launchRichochetCube(RicochetTracer.ricochetMode.hit);
            else if (Random.Range(0, 101) <= ricochetNearmissChance) //Retry to see if we'll do a nearmiss cube
                launchRichochetCube(RicochetTracer.ricochetMode.nearmiss);
            else
                launchRichochetCube(RicochetTracer.ricochetMode.miss);
        }
    }

    void launchShooterCube()
    {
        GameObject cube = objectPooler.Instance.requestObject("shooterCube");
        cube.transform.position = transform.parent.position;
        Vector2 zLimitations = shootingInfoHolder.getZLimitations(transform.position, player);
        Vector3 cubeDestination = new Vector3(
            transform.position.x,
            transform.position.y,
            Random.Range(zLimitations.x, zLimitations.y)
        );

        cubeDestination += shootingInfoHolder.RandomPointBetweenShooterCubeEclipses();

        cube.GetComponent<corruptionShooterCube>().initialize();
        cube.GetComponent<corruptionShooterCube>().launch(
            cubeDestination,
            player,
            difficultyCurveHolder.getCurrentValue(difficultyCurveHolder.Instance.shooterCubeLaunchSpeed));
    }

    void launchRichochetCube(RicochetTracer.ricochetMode mode)
    {
        RicochetPath path = GetComponentInChildren<RicochetTracer>().GenerateSuccessfulPath(mode);
        if (!path.isSuccessful) return;
        GameObject cube = objectPooler.Instance.requestObject("ricochetCube");
        cube.transform.position = transform.parent.position;
        cube.GetComponent<ricochetCube>().initialize(path);
    }
}
