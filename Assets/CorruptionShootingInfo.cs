using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptionShootingInfo : MonoBehaviour
{
    public bool shouldDraw;
    public Vector2Int shooterCubeMinRad, shooterCubeMaxRad;
    public float shooterCubeMinDistance, shooterCubeMaxDistance;
    public Transform player;



    private void OnDrawGizmos()
    {
        if (!shouldDraw) return;
        DrawEllipse(transform.position, shooterCubeMaxRad.x, shooterCubeMaxRad.y, 32, Color.yellow);
        DrawEllipse(transform.position, shooterCubeMinRad.x, shooterCubeMinRad.y, 32, Color.yellow);
        Debug.DrawLine(player.position + Vector3.forward * shooterCubeMinDistance, player.position + Vector3.forward * shooterCubeMaxDistance, Color.green);
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
