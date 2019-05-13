using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meteor : MonoBehaviour
{

    [SerializeField] float rockRotSpeed;
    Vector3 rockRotVector;
    [SerializeField] Transform rock;
    [SerializeField] pathGenerator pathGenerator;

    void Start()
    {
        setUpRockRotation();
        GetComponent<pathFollower>().setPath(pathGenerator.bezierPath);
    }

    void Update()
    {
        rotateRock();
    }

    private void setUpRockRotation()
    {
        rock.rotation = Random.rotation;
        rockRotVector = Random.onUnitSphere;
    }

    private void rotateRock()
    {
        rock.Rotate(rockRotVector * Time.deltaTime * rockRotSpeed, Space.World);
    }
}
