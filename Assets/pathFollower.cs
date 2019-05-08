using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pathFollower : MonoBehaviour
{
    public float travelTime;
    [SerializeField] Vector3 pointRotation;

    travelPath path;
    float speed;
    float lerpPerFixedUpdate, currentLerp;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (path != null)
        {
            Vector3 nextPoint = getGetNextPointOnPath();
            transform.LookAt(nextPoint);
            transform.Rotate(pointRotation, Space.Self);
            transform.position = nextPoint;
        }
    }

    //Should be called in fixedUpdate
    public Vector3 getGetNextPointOnPath()
    {
        if (currentLerp >= 1)
        {
            return path.finalDirection * speed * Time.fixedDeltaTime + transform.position;
        }

        if (path.isBezier)
        {
            currentLerp += lerpPerFixedUpdate;

            Vector3 m1 = Vector3.Lerp(path.start, path.bezierPoint, currentLerp);
            Vector3 m2 = Vector3.Lerp(path.bezierPoint, path.destination, currentLerp);
            return Vector3.Lerp(m1, m2, currentLerp);
        }
        else
        {
            currentLerp += lerpPerFixedUpdate;
            return Vector3.Lerp(path.start, path.destination, currentLerp);
        }
    }

    public void setPath(travelPath path)
    {
        this.path = path;
        speed = path.length / travelTime;
        lerpPerFixedUpdate = 1 / (path.length / speed) * Time.fixedDeltaTime;
        currentLerp = 0;
    }
}
