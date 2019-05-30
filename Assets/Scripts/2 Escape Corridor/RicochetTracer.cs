using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RicochetTracer : MonoBehaviour
{
    public enum ricochetMode
    {
        hit,
        nearmiss,
        miss
    }

    [SerializeField] Transform target;
    [SerializeField] float _ricochetObjectRadius;
    public float ricochetObjectRadius { get { return _ricochetObjectRadius; } }
    [SerializeField] LayerMask interactingLayers;
    LayerMask destinationLayerMask;
    [SerializeField] string destinationLayer;
    [SerializeField] float maxPathSegmentLength;
    [SerializeField] int maxNumberofBounces;

    [SerializeField] ricochetMode editorRicochetMode;
    RicochetPath editorRicochetPath;

    [SerializeField] Vector2 hitRegionSize, hitRegionOffset;
    [SerializeField] Vector2 nearMissRegionSize, nearMissRegionOffset;
    [SerializeField] Vector2 missRegionSize, missRegionOffset;
    [Tooltip("Buffer between the hit area and the nearmiss area to prevent nearmiss boxes coming too close to hit area")]
    [SerializeField]
    Vector2 hitBufferRegionSize, bufferRegionOffset;

    Rect missRect, nearMissRect, bufferRect, hitRect;

    [Tooltip("x = x direction, y = positive y direction, z (if not 0) represents negative y direction. None of these should be negative")]
    [SerializeField]
    Vector3 maxHitAngles, maxNearMissAngles, maxMissAngles;

    public bool shouldDrawBullsEye;
    [Tooltip("This is expensive, so it'll show down the Scene View noticeably")]
    public bool shouldVisualizeReceptionAngles;
    public bool shouldVisualizePaths;

    void Awake()
    {
        initialize();
    }

    void OnValidate()
    {
        maxNumberofBounces = Mathf.Max(2, maxNumberofBounces);
        initialize();
    }

    void initialize()
    {
        destinationLayerMask = new LayerMask();
        destinationLayerMask |= (1 << LayerMask.NameToLayer(destinationLayer)); //Adding destinationlayer to layer mask
        initializeRects();
    }

    #region Editor Visualization

    void OnDrawGizmos()
    {
        if (shouldDrawBullsEye)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(target.position + (Vector3)missRegionOffset, missRegionSize);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(target.position + (Vector3)nearMissRegionOffset, nearMissRegionSize);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(target.position + (Vector3)hitRegionOffset, hitRegionSize);
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(target.position + (Vector3)bufferRegionOffset, hitBufferRegionSize);
        }

        //Visualizing hit reception possibilities.
        if (shouldVisualizeReceptionAngles)
        {
            //For hit rect...
            Vector3 samplePoint = hitRect.center;
            samplePoint.z = target.position.z;
            drawReceptionAngle(samplePoint, ricochetMode.hit, 2);

            //For nearmiss rect...
            samplePoint.x = bufferRect.xMax + 0.5f;
            drawReceptionAngle(samplePoint, ricochetMode.nearmiss, 2);

            samplePoint.x = bufferRect.xMin - 0.5f;
            drawReceptionAngle(samplePoint, ricochetMode.nearmiss, 2);

            samplePoint.x = hitRect.center.x;
            samplePoint.y = bufferRect.max.y + 0.5f;
            drawReceptionAngle(samplePoint, ricochetMode.nearmiss, 2);

            //And finally for the miss region...
            samplePoint.x = missRect.max.x - 0.5f;
            samplePoint.y = missRect.center.y;
            drawReceptionAngle(samplePoint, ricochetMode.miss, 2);

            samplePoint.x = missRect.min.x + 0.5f;
            drawReceptionAngle(samplePoint, ricochetMode.miss, 2);

            samplePoint.x = missRect.center.x;
            samplePoint.y = missRect.max.y;
            drawReceptionAngle(samplePoint, ricochetMode.miss, 2);

            samplePoint.x = (missRect.max.x + nearMissRect.max.x) / 2;
            samplePoint.y = missRect.min.y;
            drawReceptionAngle(samplePoint, ricochetMode.miss, 2);

            samplePoint.x = (missRect.min.x + nearMissRect.min.x) / 2;
            samplePoint.y = missRect.min.y;
            drawReceptionAngle(samplePoint, ricochetMode.miss, 2);
        }

        if (shouldVisualizePaths)
        {
            drawRichochetPath();
        }
    }

    void drawReceptionAngle(Vector3 position, ricochetMode mode, float length)
    {

        Vector3 angleBundle = getAngleBundle(mode);
        Color drawColor = getDrawingColor(mode);
        

        Vector3 pathVector = Vector3.zero;

        //Receiving from right
        pathVector = Quaternion.AngleAxis(angleBundle.x, Vector3.up) * Vector3.forward;
        DebugExtension.DebugCapsule(position, position + pathVector * length, drawColor, _ricochetObjectRadius);

        //Receiving from left
        pathVector = Quaternion.AngleAxis(angleBundle.x, Vector3.down) * Vector3.forward;
        DebugExtension.DebugCapsule(position, position + pathVector * length, drawColor, _ricochetObjectRadius);

        //Receiving from up
        pathVector = Quaternion.AngleAxis(angleBundle.y, Vector3.left) * Vector3.forward;
        DebugExtension.DebugCapsule(position, position + pathVector * length, drawColor, _ricochetObjectRadius);

        if (angleBundle.z != 0)
        {
            //Receiving from down
            pathVector = Quaternion.AngleAxis(angleBundle.z, Vector3.right) * Vector3.forward;
            DebugExtension.DebugCapsule(position, position + pathVector * length, drawColor, _ricochetObjectRadius);
        }
    }

    /// <summary>
    /// Draws a path on the editor
    /// </summary>
    /// <param name="path"> Supplying no path will just render the editor's debug path</param>
    public void drawRichochetPath(RicochetPath path = null)
    {
        if (path == null)
        {
            if (editorRicochetPath == null) setEditorPath();
            path = editorRicochetPath;
        }
        for (int i = 0; i < path.ricochetPositions.Count - 1; i++)
        {
            //I'm drawing based off the objrct positond, but since the object has a raduis I have to extend the drawing to account for its volume.
            Vector3 segmentVector = (-path.ricochetPositions[i] + path.ricochetPositions[i + 1]).normalized;
            DebugExtension.DrawCapsule(path.ricochetPositions[i] + -segmentVector * ricochetObjectRadius, path.ricochetPositions[i + 1] + segmentVector * ricochetObjectRadius, getDrawingColor(path.mode), _ricochetObjectRadius);
        }

        foreach (Vector3 point in path.ricochetPositions) DebugExtension.DrawPoint(point, Color.blue, 1);
    }

    public void setEditorPath()
    {
        editorRicochetPath = GenerateSuccessfulPath(editorRicochetMode);
    }

    Color getDrawingColor(ricochetMode mode)
    {
        switch (mode)
        {
            case ricochetMode.hit:
                return Color.red;
            case ricochetMode.nearmiss:
                return Color.yellow;
            case ricochetMode.miss:
                return Color.green;
            default:
                return Color.white;
        }
    }

    #endregion

    #region Core Calulation Methods
    /// <summary>
    /// Initializes the rects that represent the bullseye,
    /// Using rects Makes Calculation easier
    /// </summary>
    void initializeRects()
    {
        missRect = new Rect(Vector2.zero, missRegionSize);
        missRect.center = target.position + (Vector3)missRegionOffset;

        nearMissRect = new Rect(Vector2.zero, nearMissRegionSize);
        nearMissRect.center = target.position + (Vector3)nearMissRegionOffset;

        bufferRect = new Rect(Vector2.zero, hitBufferRegionSize);
        bufferRect.center = target.position + (Vector3)bufferRegionOffset;

        hitRect = new Rect(Vector2.zero, hitRegionSize);
        hitRect.center = target.position + (Vector3)hitRegionOffset;
    }

    /// <summary>
    /// Genertates a richochet path. If the scene is set up properly, this is guaranteeded to return a successful path. Check just in case, though.
    /// </summary>
    /// <param name="mode"></param>
    /// <returns></returns>
    public RicochetPath GenerateSuccessfulPath(ricochetMode mode)
    {
        RicochetPath path = new RicochetPath();
        path.mode = mode;
        Vector3 angleBundle = getAngleBundle(mode);

        for (int attempt = 0; attempt < 100; attempt++)
        {
            attemptBonucyRicochetPath(mode, path, angleBundle);
            if (path.isSuccessful) break;
            path.reset();
        }

        if (!path.isSuccessful) generateGuaranteedPath(mode, path);
        return path;
    }

    /// <summary>
    /// Attempts to generate a bouncy richochet path. Not guaranteed to leave make path successful
    /// </summary>
    /// <param name="mode"></param>
    /// <param name="path"></param>
    /// <param name="angleBundle"></param>
    void attemptBonucyRicochetPath(ricochetMode mode, RicochetPath path, Vector3 angleBundle)
    {
        Vector3 launchVector = generateLaunchVector(angleBundle);  //Calculating launch direction
        Vector3 objectPositionWhenContactMade = generateStartingContactPoint(mode) + launchVector * _ricochetObjectRadius; //Calculating the starting point 

        //Calculating the ricochet points...
        path.ricochetPositions.Add(objectPositionWhenContactMade);
        RaycastHit hitResults = new RaycastHit();

        for (int i = 0; i < maxNumberofBounces; i++)
        {
            Physics.SphereCast(objectPositionWhenContactMade, _ricochetObjectRadius, launchVector, out hitResults, maxPathSegmentLength, interactingLayers, QueryTriggerInteraction.Ignore);
            if (hitResults.collider == null) //Nothing was hit...
            {
                path.ricochetPositions.Add(objectPositionWhenContactMade + launchVector * maxPathSegmentLength);
                break;
            }
            else if (hitResults.collider.gameObject.layer == LayerMask.NameToLayer(destinationLayer))
            { //Found where it was supposed to go! End here
                objectPositionWhenContactMade = objectPositionWhenContactMade + hitResults.distance * launchVector;
                path.ricochetPositions.Add(objectPositionWhenContactMade);
                path.isSuccessful = true;
                break;
            }
            else //Just hit a bouncing surface
            {
                objectPositionWhenContactMade = objectPositionWhenContactMade + hitResults.distance * launchVector;
                path.bounces++;
                path.ricochetPositions.Add(objectPositionWhenContactMade);
                launchVector = Vector3.Reflect(launchVector, hitResults.normal);
            }
        }
    }


    /// <summary>
    /// Generates a straight path to the corruption sphere.
    /// If the scene is set up properly, this code should never leave path unsuccessful.
    /// </summary>
    /// <param name="mode"></param>
    /// <param name="path"></param>
    void generateGuaranteedPath(ricochetMode mode, RicochetPath path)
    {
        Vector3 launchVector = Vector3.forward;
        Vector3 objectPositionWhenContactMade = generateStartingContactPoint(mode) + launchVector * _ricochetObjectRadius; //Calculating the starting point 

        //Calculating the ricochet points...
        path.ricochetPositions.Add(objectPositionWhenContactMade);
        RaycastHit hitResults = new RaycastHit();
        Physics.SphereCast(objectPositionWhenContactMade, _ricochetObjectRadius, launchVector, out hitResults, maxPathSegmentLength, destinationLayerMask, QueryTriggerInteraction.Ignore);
        if (hitResults.collider == null) //Nothing was hit...
        {
            return; //Worse case scenario
        }
        else
        {
            objectPositionWhenContactMade = objectPositionWhenContactMade + hitResults.distance * launchVector;
            path.ricochetPositions.Add(objectPositionWhenContactMade);
            path.isSuccessful = true;
        }
    }

    /// <summary>
    /// Returns the angle limits to a particular ricochet path mode
    /// </summary>
    /// <param name="mode"></param>
    /// <returns></returns>
    Vector3 getAngleBundle(ricochetMode mode)
    {
        switch (mode)
        {
            case ricochetMode.hit:
                return maxHitAngles;
            case ricochetMode.nearmiss:
                return maxNearMissAngles;
            case ricochetMode.miss:
                return maxMissAngles;
            default:
                return Vector3.zero;
        }
    }


    /// <summary>
    /// Generates the point at which this path shoudl start (from the player bullseye).
    /// This code relies on the assumption that for each bullseye later, a certain part of its bottom middle resion will be overlapped by the previous layer
    /// </summary>
    Vector3 generateStartingContactPoint(ricochetMode mode)
    {
        Vector3 startingPoint = Vector3.zero;
        startingPoint.z = target.position.z;

        //Setting the rects needed for the calculations...
        Rect targetRect = new Rect();
        Rect overlappingRect = new Rect(Vector3.zero , Vector3.zero);
        switch (mode)
        {
            case ricochetMode.hit:
                targetRect = hitRect;
                break;
            case ricochetMode.nearmiss:
                targetRect = nearMissRect;
                overlappingRect = bufferRect;
                break;
            case ricochetMode.miss:
                targetRect = missRect;
                overlappingRect = nearMissRect;
                break;

        }


        startingPoint.y = Random.Range(targetRect.yMin, targetRect.yMax);

        if (overlappingRect.size.SqrMagnitude() != 0 && startingPoint.y <= overlappingRect.max.y)
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

    Vector3 generateLaunchVector(Vector3 angleBundle)
    {
        Vector3 launchVector = Vector3.forward;

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
    public RicochetTracer.ricochetMode mode;
    public int bounces;
    /// <summary>
    /// An array of points in space that the object will be at when it experiences a bounce
    /// </summary>
    public List<Vector3> ricochetPositions = new List<Vector3>();
    public bool isSuccessful;

    public void reset()
    {
        bounces = 0;
        ricochetPositions.Clear();
    }
}
