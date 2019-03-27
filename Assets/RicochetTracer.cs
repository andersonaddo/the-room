using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RicochetTracer : MonoBehaviour
{
    public enum rickochetMode
    {
        hit,
        nearmiss,
        miss
    }

    public Transform target;
    public float ricochetRadius;
    public LayerMask interactingLayers;
    public float maxRicoshetLenth;
    public int maxNumberofBounces;
    public rickochetMode editorRicochetMode;

    RicochetPath editorRicochetPath;

    public Vector2 hitRadius, hitOffSet;
    public Vector2 nearMissRadius, nearmissOffset;
    public Vector2 missRadius, missOffset;
    [Tooltip("Buffer between the hit area and the nearmiss area to prevent boxes destined to land near the edges ")]
    public Vector2 hitBuffer, bufferOffset;

    Rect missRect, nearMissRect, bufferRect, hitRect;

    [Tooltip("x = XDirection, y = positiveYdirection, z (if not 0) represents negative y direction. None of these should be negative")]
    public Vector3 maxHitAngle, maxNearMissAngle, maxMissAngle;

    public bool shouldDrawBullsEye, shouldVisualizeReceptionAngles, shouldVisualizePaths;


    #region Editor Visualization
    void OnValidate()
    {
        maxNumberofBounces = Mathf.Max(2, maxNumberofBounces);
        initializeRects();
    }

    void OnDrawGizmos()
    {
        if (shouldDrawBullsEye)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(target.position + (Vector3)missOffset, missRadius);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(target.position + (Vector3)nearmissOffset, nearMissRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(target.position + (Vector3)hitOffSet, hitRadius);
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(target.position + (Vector3)bufferOffset, hitBuffer);
        }

        //Visualizing hit reception possibilities.
        if (shouldVisualizeReceptionAngles)
        {
            //For hit rect...
            Vector3 samplePoint = hitRect.center;
            samplePoint.z = target.position.z;
            drawReceptionAngle(samplePoint, rickochetMode.hit, 2);

            //For nearmiss rect...
            samplePoint.x = bufferRect.xMax + 0.5f;
            drawReceptionAngle(samplePoint, rickochetMode.nearmiss, 2);

            samplePoint.x = bufferRect.xMin - 0.5f;
            drawReceptionAngle(samplePoint, rickochetMode.nearmiss, 2);

            samplePoint.x = hitRect.center.x;
            samplePoint.y = bufferRect.max.y + 0.5f;
            drawReceptionAngle(samplePoint, rickochetMode.nearmiss, 2);

            //And finally for the miss region...
            samplePoint.x = missRect.max.x - 0.5f;
            samplePoint.y = missRect.center.y;
            drawReceptionAngle(samplePoint, rickochetMode.miss, 2);

            samplePoint.x = missRect.min.x + 0.5f;
            drawReceptionAngle(samplePoint, rickochetMode.miss, 2);

            samplePoint.x = missRect.center.x;
            samplePoint.y = missRect.max.y;
            drawReceptionAngle(samplePoint, rickochetMode.miss, 2);

            samplePoint.x = (missRect.max.x + nearMissRect.max.x) / 2;
            samplePoint.y = missRect.min.y;
            drawReceptionAngle(samplePoint, rickochetMode.miss, 2);

            samplePoint.x = (missRect.min.x + nearMissRect.min.x) / 2;
            samplePoint.y = missRect.min.y;
            drawReceptionAngle(samplePoint, rickochetMode.miss, 2);
        }

        if (shouldVisualizePaths)
        {
            drawRichochetPath();
        }
    }

    void drawReceptionAngle(Vector3 position, rickochetMode mode, float length)
    {

        Vector3 angleBundle = getAngleBundle(mode);
        Color drawColor = getDrawingColor(mode);
        

        Vector3 pathVector = Vector3.zero;

        //Receiving from right
        pathVector = Quaternion.AngleAxis(angleBundle.x, Vector3.up) * Vector3.forward;
        DebugExtension.DebugCapsule(position, position + pathVector * length, drawColor, ricochetRadius);

        //Receiving from left
        pathVector = Quaternion.AngleAxis(angleBundle.x, Vector3.down) * Vector3.forward;
        DebugExtension.DebugCapsule(position, position + pathVector * length, drawColor, ricochetRadius);

        //Receiving from up
        pathVector = Quaternion.AngleAxis(angleBundle.y, Vector3.left) * Vector3.forward;
        DebugExtension.DebugCapsule(position, position + pathVector * length, drawColor, ricochetRadius);

        if (angleBundle.z != 0)
        {
            //Receiving from down
            pathVector = Quaternion.AngleAxis(angleBundle.z, Vector3.right) * Vector3.forward;
            DebugExtension.DebugCapsule(position, position + pathVector * length, drawColor, ricochetRadius);
        }
    }

    /// <summary>
    /// Draws a path on the editor
    /// </summary>
    /// <param name="path"> Supplying no path will just render the editor's debug path</param>
    void drawRichochetPath(RicochetPath path = null)
    {
        if (path == null)
        {
            if (editorRicochetPath == null) setEditorPath();
            path = editorRicochetPath;
        }
        for (int i = 0; i < path.hitPositions.Count - 1; i++)
        {
            DebugExtension.DrawCapsule(path.hitPositions[i], path.hitPositions[i + 1], getDrawingColor(path.mode), ricochetRadius);
        }
    }

    public void setEditorPath()
    {
        editorRicochetPath = GenerateRicoshetPath(editorRicochetMode, maxNumberofBounces);
    }

    Color getDrawingColor(rickochetMode mode)
    {
        switch (mode)
        {
            case rickochetMode.hit:
                return Color.red;
            case rickochetMode.nearmiss:
                return Color.yellow;
            case rickochetMode.miss:
                return Color.green;
            default:
                return Color.white;
        }
    }

    #endregion

    #region Core Calulation Methods
    //Initializes the rects that represent the bullseye,
    //Using rects Makes Calculation easier
    void initializeRects()
    {
        missRect = new Rect(Vector2.zero, missRadius);
        missRect.center = target.position + (Vector3)missOffset;

        nearMissRect = new Rect(Vector2.zero, nearMissRadius);
        nearMissRect.center = target.position + (Vector3)nearmissOffset;

        bufferRect = new Rect(Vector2.zero, hitBuffer);
        bufferRect.center = target.position + (Vector3)bufferOffset;

        hitRect = new Rect(Vector2.zero, hitRadius);
        hitRect.center = target.position + (Vector3)hitOffSet;
    }

    Vector3 getAngleBundle(rickochetMode mode)
    {
        switch (mode)
        {
            case rickochetMode.hit:
                return maxHitAngle;
            case rickochetMode.nearmiss:
                return maxNearMissAngle;
            case rickochetMode.miss:
                return maxMissAngle;
            default:
                return Vector3.zero;
        }
    }

    //This code relies on the assumption that for each bullseye later, a certain part of its bottom middle resion will be overlapped by the previous layer
    public RicochetPath GenerateRicoshetPath(rickochetMode mode, int maxNumberOfBounces)
    {

        RicochetPath path = new RicochetPath();
        path.mode = mode;

        Vector3 angleBundle = getAngleBundle(mode);
        Vector3 startingPoint = generateStartingPoint(mode);  //Calculating the starting point    
        Vector3 launchVector = generateLaunchVector(mode);  //Calculating launch direction

        //Calculating the ricochet points...
        path.hitPositions.Add(startingPoint);
        RaycastHit hitResults = new RaycastHit();
        for (int i = 0; i < maxNumberOfBounces; i++)
        {
            Physics.SphereCast(startingPoint, ricochetRadius, launchVector.normalized, out hitResults, maxRicoshetLenth, interactingLayers, QueryTriggerInteraction.Ignore);
            if (hitResults.collider == null) //Nothing was hit...
            {
                path.hitPositions.Add(startingPoint + launchVector * maxRicoshetLenth);
                break;
            }
            else
            {
                path.bounces++;
                path.hitPositions.Add(hitResults.point);
                startingPoint = hitResults.point;
                launchVector = Vector3.Reflect(launchVector, hitResults.normal);
            }
        }
        return path;
    }

    Vector3 generateStartingPoint(rickochetMode mode)
    {
        Vector3 startingPoint = Vector3.zero;
        startingPoint.z = target.position.z;

        //Setting the rects needed for the calculations...
        Rect targetRect = new Rect();
        Rect overlappingRect = new Rect(Vector3.zero , Vector3.zero);
        switch (mode)
        {
            case rickochetMode.hit:
                targetRect = hitRect;
                break;
            case rickochetMode.nearmiss:
                targetRect = nearMissRect;
                overlappingRect = bufferRect;
                break;
            case rickochetMode.miss:
                targetRect = missRect;
                overlappingRect = nearMissRect;
                break;

        }


        startingPoint.y = Random.Range(targetRect.yMin, targetRect.yMax);

        if (overlappingRect.size.SqrMagnitude() == 0 && startingPoint.y <= overlappingRect.max.y)
        {
            //Only choose an x from the sides
            if (Random.Range(0, 2) == 1) startingPoint.x = Random.Range(overlappingRect.xMin, targetRect.xMin);
            else startingPoint.x = Random.Range(overlappingRect.xMax, targetRect.xMax);
        }
        else
        {
            //Choose any x
            startingPoint.x = Random.Range(targetRect.xMin, targetRect.xMax);
        }

        return startingPoint;
    }

    Vector3 generateLaunchVector(rickochetMode mode)
    {
        Vector3 launchVector = Vector3.forward;
        Vector3 angleBundle = getAngleBundle(mode);

        launchVector = Quaternion.AngleAxis(Random.Range(-angleBundle.x, angleBundle.x), Vector3.up) * launchVector;
        if (angleBundle.z == 0) //Ie if only an upwards rotation is allowed...
            launchVector = Quaternion.AngleAxis(Random.Range(0, angleBundle.y), Vector3.left) * launchVector;
        else
            launchVector = Quaternion.AngleAxis(Random.Range(-angleBundle.z, angleBundle.y), Vector3.left) * launchVector;

        return launchVector;
    }
    #endregion

}

public class RicochetPath
{
    public RicochetTracer.rickochetMode mode;
    public int bounces;
    public List<Vector3> hitPositions = new List<Vector3>();
}
