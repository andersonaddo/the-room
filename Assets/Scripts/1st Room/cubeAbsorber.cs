using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cubeAbsorber : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IShootableCube>().type == gameManager.Instance.currentTargetType)
        {
            other.GetComponent<GenericShootableCube>().selfDestruct();
            gameManager.Instance.signalCubeAbsorption();
        }
    }
}
