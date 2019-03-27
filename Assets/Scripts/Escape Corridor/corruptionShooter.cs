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
        Vector2 zLimitations = shootingInfo.getZLimitations(transform.position, player);
        Vector3 cubeDestination = new Vector3(
            transform.position.x,
            transform.position.y,
            Random.Range(zLimitations.x, zLimitations.y)
        );

        cubeDestination += shootingInfo.RandomPointBetweenShhooterCubeEclipses();

        cube.GetComponent<corruptionShooterCube>().launch(
            cubeDestination,
            player,
            difficultyCurveHolder.getCurrentValue(difficultyCurveHolder.Instance.shooterCubeLaunchSpeed));
    }
}
