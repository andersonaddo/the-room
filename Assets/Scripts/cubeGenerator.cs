using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cubeGenerator : MonoBehaviour {

    public float coneAngle, speed, baseWaitTime;

    public List<GameObject> cubes;
    public BoxCollider mainRegion, rightRegion, leftRegion;

    //Calculates a vector inside a cone of angle coneAngle
	Vector3 calculateVelocityVector (float magnitude) {
        Vector3 vector = Vector3.forward * (Random.Range(0, 2) == 1 ? -1 : 1);
        vector = Quaternion.AngleAxis(Random.Range(-coneAngle, coneAngle), Vector3.up) * vector;
        vector = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward) * vector;
        return vector.normalized * magnitude;
    }

    void Start()
    {
        StartCoroutine("generateCubes");
    }

    IEnumerator generateCubes()
    {
        while (true)
        {
            yield return new WaitForSeconds(difficultyCurveHolder.Instance.cubeGenerationRate.Evaluate(difficultyCurveHolder.currentDifficulty));
            BoxCollider spawnCollider = chooseRandomCollider();
            Vector3 spawnPoint = new Vector3(
                Random.Range(spawnCollider.bounds.min.x, spawnCollider.bounds.max.x),
                Random.Range(spawnCollider.bounds.min.y, spawnCollider.bounds.max.y),
                Random.Range(spawnCollider.bounds.min.z, spawnCollider.bounds.max.z)
            );
            GameObject cube = Instantiate(chooseCube(), spawnPoint, Quaternion.identity);
            cube.GetComponent<Rigidbody>().velocity = calculateVelocityVector(speed);
            cube.GetComponent<IShootableCube>().initialize();
        }
    }	

    BoxCollider chooseRandomCollider()
    {
        int seed = Random.Range(1, 5);
        if (seed == 1) return leftRegion;
        if (seed == 2) return rightRegion;
        return mainRegion;
    }

    GameObject chooseCube()
    {
        return null;
    }
}
