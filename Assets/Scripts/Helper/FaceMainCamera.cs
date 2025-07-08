using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceMainCamera : MonoBehaviour
{
    protected Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("No main camera found in the scene. Please ensure there is a camera tagged as 'MainCamera'.");
        }
    }

    void Update()
    {
        if (mainCamera != null)
        {
            transform.LookAt(mainCamera.transform);
        }   
    }
}
