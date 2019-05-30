using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//All logic here assumes the origin to be the center of the playing field
public class pathGenerator : MonoBehaviour {

    [SerializeField] Transform target;
    Vector3 startingPosition, destination;

    [SerializeField] float bezierHeightMultiplier;
    [SerializeField] Vector3 midPointDelta; //The shift that will be applied to the midpoint to make the bezier point
    Vector3 midpoint, bezierPoint;

    [SerializeField] bool shouldDrawStraightPath, shouldDrawBezierPath, shouldUpdatePathsLive;
    [SerializeField] int drawingSegments;
    [SerializeField] Color gizmoColor;

    public travelPath straightPath = new travelPath(false); //Straight path to destination
    public travelPath bezierPath = new travelPath(true); //Bezier path to destination

    void OnValidate()
    {
        setUpPaths();
    }

    void Start()
    {
        setUpPaths();
    }

    void Update()
    {
        if (shouldUpdatePathsLive) setUpPaths();
    }

    private void setUpPaths()
    {
        startingPosition = transform.position;
        destination = target.position;
        midpoint = startingPosition + 0.5f * (destination - startingPosition);
        bezierPoint = midpoint + midPointDelta * bezierHeightMultiplier;

        straightPath.destination = destination;
        straightPath.length = Vector3.Distance(startingPosition, destination);
        straightPath.start = startingPosition;
        straightPath.finalDirection = (destination - startingPosition).normalized;

        bezierPath.bezierPoint = bezierPoint;
        bezierPath.destination = destination;
        bezierPath.start = startingPosition;
        bezierPath.length = estimateBezierCurveLength();
        bezierPath.finalDirection = calculateFinalBezierVector();
    }


    void OnDrawGizmos()
    {
        if (shouldDrawStraightPath) drawStraightPath();
        if (shouldDrawBezierPath) drawBezierPath();
    }


    private void drawStraightPath()
    {
        Gizmos.DrawLine(startingPosition, destination);
    }

    private void drawBezierPath()
    {

        Gizmos.color = Color.grey;
        Gizmos.DrawCube(midpoint, Vector3.one * 0.3f);
        Gizmos.DrawLine(midpoint, bezierPoint);

        Gizmos.color = gizmoColor;

        //Splitting the curve into segments
        float increment = 1f / drawingSegments;
        Vector3 p1 = Vector3.zero;
        Vector3 p2 = Vector3.zero;

        for (int i = 0; i <= drawingSegments -1 ; i++)
        {
            //Getting the first point
            Vector3 m1 = Vector3.Lerp(startingPosition, bezierPoint, increment * i);
            Vector3 m2 = Vector3.Lerp(bezierPoint, destination, increment * i);
            p1 = Vector3.Lerp(m1, m2, increment * i);

            //Second point
            m1 = Vector3.Lerp(startingPosition, bezierPoint, increment * i + increment);
            m2 = Vector3.Lerp(bezierPoint, destination, increment * i + increment);
            p2 = Vector3.Lerp(m1, m2, increment * i + increment);
            Gizmos.DrawLine(p1, p2);
        }

        Gizmos.DrawLine(destination, destination + bezierPath.finalDirection * 10);

    }

    float estimateBezierCurveLength()
    {
        //Splitting the curve into 10 segments
        float totalDistance = 0;
        float increment = 1f / 10f;

        for (int i = 0; i <= 10; i++)
        {
            //Getting the first point
            Vector3 m1 = Vector3.Lerp(startingPosition, bezierPoint, increment * i);
            Vector3 m2 = Vector3.Lerp(bezierPoint, destination, increment * i);
            Vector3 p1 = Vector3.Lerp(m1, m2, increment * i);

            //Second point
            m1 = Vector3.Lerp(startingPosition, bezierPoint, increment * i + increment);
            m2 = Vector3.Lerp(bezierPoint, destination, increment * i + increment);
            Vector3 p2 = Vector3.Lerp(m1, m2, increment * i + increment);

            totalDistance += Vector3.Distance(p1, p2);
        }

        return totalDistance;
    }

    private Vector3 calculateFinalBezierVector()
    {
        float lerp = 0.99999f;
        Vector3 m1 = Vector3.Lerp(startingPosition, bezierPoint, lerp);
        Vector3 m2 = Vector3.Lerp(bezierPoint, destination, lerp);
        Vector3 point = Vector3.Lerp(m1, m2, lerp);
        return (destination - point).normalized;
    }
}

public class travelPath
{
    public bool isBezier;
    public float length;
    public Vector3 start, destination, bezierPoint;

    //This is the direction of travel the object following the path will take once it passes the destination
    //This could have been calculated by simply making the object move along the direction between the destination and the 
    //last point it was at before getting to the destination, but that ran into a few problems.
    //Depending on how fast the object is moving, that vector could be different things. This inconsistency also made the scene view previews of the paths unreliable
    //in the region. If I tried to sync the two by getting the currentLerp of the path follower jsut before it reaches the destination and using that to draw the path,
    //it still wouldn't work (probably due to floating point estimations I didn't know were happening). So, I'll just precalcualte this vector for everyone to use consistently
    public Vector3 finalDirection;

    public travelPath(bool isBezier)
    {
        this.isBezier = isBezier;
    }
}
