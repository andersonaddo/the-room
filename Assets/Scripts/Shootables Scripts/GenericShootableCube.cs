using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericShootableCube : MonoBehaviour, IShootableCube, IFreezable
{

    [SerializeField] cubeTypes type;
    [SerializeField] float rotSpeed, explosionRadius, explosionForce;
    Vector3 prefreezeAngularVelocity, prefreezeLinearVelocity;

    public int bulletShotScore, megaCubeScore;

    public cubeTypes _type
    {
        get { return type; }
    }

    void Awake()
    {
        GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere * rotSpeed;
    }

    public void onShot(Vector3 position, damageEffectors effector)
    {
        calculatePoints(effector);
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

    public void unfreeze(bool destroyIfPossible, damageEffectors effector)
    {
        if (destroyIfPossible)
        {
            calculatePoints(effector);
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
        GameObject bc = objectPooler.Instance.requestObject("brokencube");
        bc.transform.position = transform.position;
        bc.transform.rotation = transform.rotation;
        bc.GetComponent<brokenCube>().initialize(
            GetComponent<Light>().color,
            GetComponent<MeshRenderer>().material,
            position,
            explosionForce,
            explosionRadius);
        Destroy(gameObject);
    }

    protected void calculatePoints(damageEffectors effector)
    {
        if (effector == damageEffectors.bullet)
        {
            if (gameManager.Instance.currentTargetType == type || type == cubeTypes.special)
                gameManager.Instance.incrementScore(bulletShotScore);
        }
        else if (effector == damageEffectors.megaCube)
        {
            gameManager.Instance.incrementScore(bulletShotScore);
        }
    }
}
