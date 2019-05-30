using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerBullet : MonoBehaviour, ISelfDestructInstructions {

    public float speed;
    public GameObject explosion;
    public float raycastR;
    public LayerMask mask;

	// Use this for initialization
	public void launch (Vector3 forwardDirection) {
        GetComponent<Rigidbody>().velocity = forwardDirection.normalized * speed;
        GetComponent<timedSelfDestruct>().startTimer();
    }


    void destroyBullet(Vector3 explosionPoint)
    {
        GameObject explosion = objectPooler.Instance.requestObject("bulletExplosion");
        explosion.transform.position = explosionPoint;
        explosion.GetComponent<bulletExplosionScript>().explode();
        resetForPooling();
    }

    void OnCollisionEnter(Collision other)
    {
        IShootableCube cubeScript = other.gameObject.GetComponent<IShootableCube>();
        if (cubeScript != null) cubeScript.onShot(transform.position, damageEffectors.bullet);
        destroyBullet(transform.position);
    }

    void resetForPooling()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<timedSelfDestruct>().cancel();
        objectPooler.Instance.returnObject("bullet", gameObject);
    }

    public void selfDestruct()
    {
        resetForPooling();
    }
}
