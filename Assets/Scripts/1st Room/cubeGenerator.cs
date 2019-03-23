using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class cubeGenerator : MonoBehaviour {

    public float coneAngle, speed, baseWaitTime;

    public BoxCollider mainRegion, rightRegion, leftRegion;

    List<cubeTypes> types;

    //Calculates a vector inside a cone of angle coneAngle
    Vector3 calculateVelocityVector (float magnitude) {
        Vector3 vector = Vector3.forward * (Random.Range(0, 2) == 1 ? -1 : 1);
        vector = Quaternion.AngleAxis(Random.Range(-coneAngle, coneAngle), Vector3.up) * vector;
        vector = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward) * vector;
        return vector.normalized * magnitude;
    }

    void Start()
    {
        types = cubeTypes.GetValues(typeof(cubeTypes)).OfType<cubeTypes>().ToList();
        StartCoroutine("generateCubes");
    }

    IEnumerator generateCubes()
    {
        while (true)
        {
            yield return new WaitForSeconds(difficultyCurveHolder.getCurrentValue(difficultyCurveHolder.Instance.cubeGenerationRate));
            BoxCollider spawnCollider = chooseRandomCollider();
            Vector3 spawnPoint = new Vector3(
                Random.Range(spawnCollider.bounds.min.x, spawnCollider.bounds.max.x),
                Random.Range(spawnCollider.bounds.min.y, spawnCollider.bounds.max.y),
                Random.Range(spawnCollider.bounds.min.z, spawnCollider.bounds.max.z)
            );
            GameObject cube = chooseCube();
            cube.transform.position = spawnPoint;
            cube.transform.rotation = Quaternion.identity;
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
        GameObject chosenCube;
        int random = Random.Range(1, 101);
        if (random <= difficultyCurveHolder.getCurrentValue(difficultyCurveHolder.Instance.specialCubeChance))
        {
            //Choose a special cube
            chosenCube = objectPooler.Instance.requestObject("megaCube");
            return chosenCube;
        }

        random = Random.Range(1, 101); //Ok then let's reseed for the non-special cubes now.
        if (random <= difficultyCurveHolder.getCurrentValue(difficultyCurveHolder.Instance.targetCubePriority))
        {
            //Choose the target cube
            chosenCube = objectPooler.Instance.requestObject("genericCube");
            chosenCube.GetComponent<GenericShootableCube>().useConfig(gameManager.Instance.currentTargetType);
            return chosenCube;
        }
        else
        {
            //Choose the target cube
            chosenCube = objectPooler.Instance.requestObject("genericCube");
            var possibleTypes = types.Where(type => type != gameManager.Instance.currentTargetType && type != cubeTypes.special);
            chosenCube.GetComponent<GenericShootableCube>().useConfig(possibleTypes.ElementAt(Random.Range(0, possibleTypes.Count())));
            return chosenCube;
        }
    }
}
