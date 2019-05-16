using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followCameraOrientation : MonoBehaviour
{
    enum orientationTractingMode
    {
        makeParent, //Make the camera a your parent after the game starts
        copyRotPos //Copy the camera's rotation and position. This can have the same apparent effect as makeParent if object is close enough to cmaera transform
    }

    [SerializeField] orientationTractingMode mode = orientationTractingMode.copyRotPos;

    [SerializeField] Transform cameraTransform;
    Vector3 originalPosition;
    Quaternion originalRotation;

    void Start()
    {
        if (mode == orientationTractingMode.makeParent) transform.SetParent(cameraTransform);
        if (mode == orientationTractingMode.copyRotPos)
        {
            originalPosition = transform.localPosition;
            originalRotation = transform.localRotation;
        }
    }

    void Update()
    {
        if (mode == orientationTractingMode.copyRotPos)
        {
            transform.localRotation = cameraTransform.localRotation * originalRotation;
            transform.localPosition = originalPosition + cameraTransform.localPosition;
        }
    }
}
