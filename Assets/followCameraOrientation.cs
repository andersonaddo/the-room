using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followCameraOrientation : MonoBehaviour
{
    [SerializeField] Transform cameraTransform;
    Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        transform.localRotation = cameraTransform.localRotation;
        transform.localPosition = originalPosition + cameraTransform.localPosition;
    }
}
