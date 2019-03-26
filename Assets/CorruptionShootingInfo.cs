using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptionShootingInfo : MonoBehaviour
{
    public bool shouldDraw;
    public Transform player;

    public Vector2 shooterCubeMinRad, shooterCubeMaxRad;
    public float shooterCubeMinDistance, shooterCubeMaxDistance; //The minimum and maximum distance (in z-coords only) that shooter cubes can be from the player
    public float shooterSourceBuffer; //The minimum buffer distance that cubes should have between the shooting source no matter what. Overrides shooterCubeMaxDistance

    void OnValidate()
    {
        shooterSourceBuffer = Mathf.Max(shooterSourceBuffer, 0);
    }

    void OnDrawGizmos()
    {
        if (!shouldDraw) return;
        DrawEllipse(transform.position, shooterCubeMaxRad.x, shooterCubeMaxRad.y, 32, Color.yellow);
        DrawEllipse(transform.position, shooterCubeMinRad.x, shooterCubeMinRad.y, 32, Color.yellow);

        Vector2 limitations = getZLimitations(transform.position, player);

        Debug.DrawLine(transform.position, transform.position - Vector3.forward * shooterSourceBuffer, Color.magenta);

        Debug.DrawLine(
            new Vector3(transform.position.x, transform.position.y, limitations.y),
            new Vector3(transform.position.x, transform.position.y, limitations.x),
            Color.green);
    }

    /// <summary>
    /// Returns the z limits for the resting positions of the shooter cubes given thier shooting source
    /// </summary>
    /// <param name="shootingSourcePosition"></param>
    /// <param name="target"></param>
    /// <returns>A vector2 with x being the minimum and y being the maximum (note that the cubes actually move towards the negative z region)</returns>
    public Vector2 getZLimitations(Vector3 shootingSourcePosition, Transform target)
    {
        Vector2 limitaions = Vector2.zero;
        limitaions.x = target.position.z + shooterCubeMinDistance;

        float zUsingMaxDistance = target.position.z + shooterCubeMaxDistance;
        float zUsingBuffer = shootingSourcePosition.z - shooterSourceBuffer;
        limitaions.y = Mathf.Min(zUsingBuffer, zUsingMaxDistance);
        return limitaions;
    }

    public Vector3 RandomPointBetweenShhooterCubeEclipses()
    {
        Vector3 point = Vector3.zero;
        point.x = Mathf.Sin(Mathf.Deg2Rad * Random.Range(0, 360)) * Random.Range(shooterCubeMinRad.x, shooterCubeMaxRad.x);
        point.y = Mathf.Cos(Mathf.Deg2Rad * Random.Range(0, 360)) * Random.Range(shooterCubeMinRad.y, shooterCubeMaxRad.y);
        return point;
    }


    //https://forum.unity.com/threads/solved-debug-drawline-circle-ellipse-and-rotate-locally-with-offset.331397/
    private static void DrawEllipse(Vector3 pos, float radiusX, float radiusY, int segments, Color color, float duration = 0)
    {
        float angle = 0f;
        Quaternion rot = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        Vector3 lastPoint = Vector3.zero;
        Vector3 thisPoint = Vector3.zero;

        for (int i = 0; i < segments + 1; i++)
        {
            thisPoint.x = Mathf.Sin(Mathf.Deg2Rad * angle) * radiusX;
            thisPoint.y = Mathf.Cos(Mathf.Deg2Rad * angle) * radiusY;

            if (i > 0)
            {
                Debug.DrawLine(rot * lastPoint + pos, rot * thisPoint + pos, color, duration);
            }

            lastPoint = thisPoint;
            angle += 360f / segments;
        }
    }
}
