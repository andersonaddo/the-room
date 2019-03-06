using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class megaCubeScript : GenericShootableCube, IShootableCube, IFreezable
{

    [SerializeField] float postHitRotSpeed;
    public float explosionWaitTime;
    public GameObject explosionPS;
    Vector3 rotateVector;
    bool isActivated;

    new public void onShot(Vector3 shotPosition, damageEffectors effector)
    {
        if (effector != damageEffectors.bullet) return;
        if (!isActivated)
        {
            calculatePoints(effector);
            StartCoroutine("destroyAllCubes");
        }
    }

    IEnumerator destroyAllCubes()
    {
        GetComponent<timedSelfDestruct>().cancel();
        preExplosionFreeze();
        Instantiate(explosionPS, transform.position, Quaternion.identity);

        List<GameObject> freezables = FindObjectsOfType<GameObject>().Where(go => go.GetComponent<IFreezable>() != null).ToList<GameObject>();
        freezables.Remove(gameObject); //So that it doesn't freeze itself
        foreach (GameObject ga in freezables)
        {
            if (ga) ga.GetComponent<IFreezable>().freeze();
        }

        yield return new WaitForSeconds(explosionWaitTime);

        foreach (GameObject ga in freezables)
        {
            //Making sure the thing can still be shattered...
            if (!ga) continue;
            if (ga.GetComponent<megaCubeScript>() && ga.GetComponent<megaCubeScript>().isActivated) continue;

            ga.GetComponent<IFreezable>().unfreeze(true, damageEffectors.megaCube); //Destroying all cubes
        }

        unfreeze(true, damageEffectors.megaCube); //Destroying itself
    }

    void preExplosionFreeze()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rotateVector = Random.onUnitSphere * postHitRotSpeed;
        isActivated = true;
    }

    void Update()
    {
        if (isActivated)
        {
            transform.Rotate(rotateVector * Time.deltaTime);
        }
    }

}
