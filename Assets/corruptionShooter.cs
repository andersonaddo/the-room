using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class corruptionShooter : MonoBehaviour
{
    public GameObject shooterCube;
    public Transform player;
    CorruptionShootingInfo shootingInfo;

    void Start()
    {
        shootingInfo = GetComponentInChildren<CorruptionShootingInfo>();
        launchShooterCube();
        launchShooterCube();
        launchShooterCube();
        launchShooterCube();
        launchShooterCube();

    }


    void launchShooterCube()
    {
        GameObject cube = Instantiate(shooterCube, transform.parent.position, Quaternion.identity);
        Vector3 cubeDestination = new Vector3(
            transform.position.x,
            transform.position.y,
            player.transform.position.z + Random.Range(shootingInfo.shooterCubeMinDistance, shootingInfo.shooterCubeMaxDistance)
        );

        cubeDestination += RandomPointBetweenEclipses(shootingInfo.shooterCubeMinRad, shootingInfo.shooterCubeMaxRad);

        cube.GetComponent<corruptionShooterCube>().launch(
            cubeDestination,
            player,
            difficultyCurveHolder.getCurrentValue(difficultyCurveHolder.Instance.shooterCubeLaunchSpeed));
    }

    Vector3 RandomPointBetweenEclipses(Vector2 minRad, Vector2 maxRad)
    {
        Vector3 point = Vector3.zero;
        point.x = Mathf.Sin(Mathf.Deg2Rad * Random.Range(0, 360)) * Random.Range(minRad.x, maxRad.x);
        point.y = Mathf.Cos(Mathf.Deg2Rad * Random.Range(0, 360)) * Random.Range(minRad.y, maxRad.y);
        return point;
    }
}
