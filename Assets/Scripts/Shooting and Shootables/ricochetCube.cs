using System;
using UnityEngine;

public class ricochetCube : MonoBehaviour, IShootableCube
{
    public cubeTypes type
    {
        get
        {
            throw new System.NotImplementedException();
        }
    }

    RicochetPath path;
    int destinationPointIndex;
    public float speed, rotationSpeed;
    Vector3 rotationVector;

    bool hasDamagedPlayer;
    [SerializeField] string playerLayer;
    [SerializeField] int damageOnHit;

    void Awake()
    {
        rotationVector = UnityEngine.Random.onUnitSphere * rotationSpeed;
    }

    void Update()
    {
        rotate();
        followPath();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(playerLayer))
        {
            //Since the cubeis rotating all over the place
            //it may be possible that an edge could ennter and exit the player collider before the whole cube does
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

    public void setPath(RicochetPath path)
    {
        this.path = path;
        destinationPointIndex = path.ricochetPositions.Count - 1;
        positionForFollowing();
    }

    //Places the cube just behind the first point in the path at the right angle
    void positionForFollowing()
    {
        Vector3 firstSegmentVector = path.ricochetPositions[path.ricochetPositions.Count - 2] -  path.ricochetPositions[path.ricochetPositions.Count - 1];
        transform.position = path.ricochetPositions[path.ricochetPositions.Count - 1] - firstSegmentVector.normalized;
    }

    public void initialize()
    {
        throw new System.NotImplementedException();
    }

    public void onShot(Vector3 shotPosition, damageEffectors damageEffector)
    {
        throw new System.NotImplementedException();
    }

}
