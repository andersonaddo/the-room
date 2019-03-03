using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class brokenCube : MonoBehaviour {

    [SerializeField] Material brokenCubeMaterial;
    Material brokenCubeMaterialCopy;

    public void initialize (Color lightColor, Material cubeMat, Vector3 position, float explosionForce, float explosionRadius) {
        brokenCubeMaterialCopy = new Material(brokenCubeMaterial);
        brokenCubeMaterialCopy.color = cubeMat.color;

        if (cubeMat.IsKeywordEnabled("_EMISSION"))
        {
            brokenCubeMaterialCopy.EnableKeyword("_EMISSION");
            brokenCubeMaterialCopy.SetColor("_EmissionColor", cubeMat.GetColor("_EmissionColor"));
        }

        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.AddExplosionForce(explosionForce, position, explosionRadius);
            rb.GetComponent<Light>().color = lightColor;
            rb.GetComponent<MeshRenderer>().material = brokenCubeMaterialCopy;
        }
	}
}
