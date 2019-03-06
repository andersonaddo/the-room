using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerBullet : MonoBehaviour {

    public float speed;
    public GameObject explosion;
    public float raycastR;
    public LayerMask mask;

	// Use this for initialization
	public void launch (Vector3 forwardDirection) {
        GetComponent<Rigidbody>().velocity = forwardDirection.normalized * speed;
	}


    void destroyBullet(Vector3 explosionPoint)
    {
        Instantiate(explosion, explosionPoint, Quaternion.identity);
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision other)
    {
        IShootableCube cubeScript = other.gameObject.GetComponent<IShootableCube>();
        if (cubeScript != null) cubeScript.onShot(transform.position, damageEffectors.bullet);
        destroyBullet(transform.position);
        //print("Bullet hit " + other.gameObject.name);
    }
}
