using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meteorRestarter : MonoBehaviour
{
    void OnParticleCollision(GameObject other){
        other.GetComponentInParent<meteorMovementManager>().transitionBackToMovementMode();
        Destroy(gameObject);
    }
}
