using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericShootableCube : MonoBehaviour, IShootableCube, IFreezable
{

    [SerializeField] cubeTypes type;
    [SerializeField] float rotSpeed, explosionRadius, explosionForce;
    public GameObject brokenCube;
    Vector3 prefreezeAngularVelocity, prefreezeLinearVelocity;

    public cubeTypes _type
    {
        get { return type; }
    }

    void Awake()
    {
        GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere * rotSpeed;
    }

    public void onShot(Vector3 position)
    {
        doPoints();
        explode(position);
    }

    public void freeze()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        prefreezeAngularVelocity = rb.angularVelocity;
        prefreezeLinearVelocity = rb.velocity;
        GetComponent<timedSelfDestruct>().cancel();
        rb.isKinematic = true;
    }

    public void unfreeze(bool destroyIfPossible)
    {
        if (destroyIfPossible)
        {
            doPoints();
            explode(transform.position);
        }
        else
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.angularVelocity = prefreezeAngularVelocity;
            rb.velocity = prefreezeLinearVelocity;
        }
    }

    void explode(Vector3 position)
    {
        GameObject bc = Instantiate(brokenCube, transform.position, transform.rotation);
        bc.GetComponent<brokenCube>().initialize(
            GetComponent<Light>().color,
            GetComponent<MeshRenderer>().material,
            position,
            explosionForce,
            explosionRadius);
        Destroy(gameObject);
    }

    void doPoints()
    {

    }
}
