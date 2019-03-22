using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cubeAbsorber : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IShootableCube>().type == gameManager.Instance.currentTargetType)
        {
            Destroy(other.gameObject);
            gameManager.Instance.signalCubeAbsorption();
        }
    }
}
