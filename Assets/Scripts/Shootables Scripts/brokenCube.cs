using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class brokenCube : MonoBehaviour, ISelfDestructInstructions {

    [SerializeField] Material brokenCubeMaterial;
    Material brokenCubeMaterialCopy;

    Dictionary<Rigidbody, Vector3> originalLocalPositions = new Dictionary<Rigidbody, Vector3>();

    void Awake()
    {
        recordOriginalPositions();
    }

    public void initialize (Color lightColor, Material cubeMat, Vector3 position, float explosionForce, float explosionRadius) {

        GetComponent<timedSelfDestruct>().startTimer();

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

    void resetForPooling()
    {
        GetComponent<timedSelfDestruct>().cancel();

        //Resetting all the smaller pieces...
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.velocity = new Vector3(0f, 0f, 0f);
            rb.angularVelocity = new Vector3(0f, 0f, 0f);
            rb.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
            rb.transform.localPosition = originalLocalPositions[rb];
        }
        gameObject.SetActive(false); //Can now be reused by pooler.
    }

    void recordOriginalPositions()
    {
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            originalLocalPositions.Add(rb, rb.transform.localPosition);
        }
    }

    public void selfDestruct()
    {
        resetForPooling();
    }
}
