using System;
using UnityEngine;

public class ricochetCube : MonoBehaviour, IShootableCube
{
    [SerializeField] cubeTypes _type;
    public cubeTypes type
    {
        get
        {
            return _type;
        }
    }

    RicochetPath path; //Set externally when the cube is created
    int destinationPointIndex; //Index of the point in the cube's path it is currently moving to
    [SerializeField] float rotationSpeed;
    float speed;
    Vector3 rotationVector;

    bool hasDamagedPlayer;
    [SerializeField] string playerLayer;
    [SerializeField] int damageOnHit;

    void Awake()
    {
        rotationVector = UnityEngine.Random.onUnitSphere * rotationSpeed;
        speed =  difficultyCurveHolder.getCurrentValue(difficultyCurveHolder.Instance.richochetCubeSpeed);
    }

    void Update()
    {
        rotate();
        followPath();
    }

    void OnTriggerEnter(Collider other)
    {
        if (path.mode != RicochetTracer.ricochetMode.hit) return; //You were never meant to hit the player in the first place. The buffer region in the player bullseye should have prevented this actually.
        if (other.gameObject.layer == LayerMask.NameToLayer(playerLayer))
        {
            //Since the cube is rotating all over the place
            //it may be possible that an edge could enter and exit the player's collider before the whole cube does (meaning ontriggerenter gets called twice)
            if (hasDamagedPlayer) return;
            hasDamagedPlayer = true;
            other.GetComponentInParent<playerDamager>().inflictDamage(damageOnHit);
        }
    }

    void rotate()
    {
        transform.Rotate(rotationVector * Time.deltaTime);
    }

    private void followPath()
    {
        if (path == null) return;

        if (destinationPointIndex != -1) { //Hasn't reached the last point yet...
            transform.position = Vector3.MoveTowards(transform.position, path.ricochetPositions[destinationPointIndex], speed * Time.deltaTime);
            if (transform.position == path.ricochetPositions[destinationPointIndex])
            {
                rotationVector = UnityEngine.Random.onUnitSphere * rotationSpeed;
                destinationPointIndex--;
            }
        }
        else
        {
            Vector3 translationVector = path.ricochetPositions[0] - path.ricochetPositions[1];
            transform.Translate(translationVector.normalized * Time.deltaTime * speed, Space.World);
        }

    }

    //Called externally when the cube is created
    public void setPath(RicochetPath path)
    {
        this.path = path;
        destinationPointIndex = path.ricochetPositions.Count - 1;
        positionForFollowing();
    }

    //Places the cube just behind the first point in the path at the appropriate trajectory
    void positionForFollowing()
    {
        Vector3 firstSegmentVector = path.ricochetPositions[path.ricochetPositions.Count - 2] -  path.ricochetPositions[path.ricochetPositions.Count - 1];
        transform.position = path.ricochetPositions[path.ricochetPositions.Count - 1] - firstSegmentVector.normalized;
    }

    public void initialize()
    {
        
    }

    public void onShot(Vector3 shotPosition, damageEffectors damageEffector)
    {
        Destroy(gameObject);
    }

}
