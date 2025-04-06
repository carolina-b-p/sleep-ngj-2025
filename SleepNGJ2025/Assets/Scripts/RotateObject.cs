using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    private Transform target; // The target object to rotate around y-axis
    public float rotationSpeed = 10f; // Speed of rotation

    
    private void Update()
    {
        if (target == null)
        {
            target = gameObject.transform;
        }
        // Calculate the rotation step
        float step = rotationSpeed * Time.deltaTime;
        // Calculate the new rotation
        Quaternion targetRotation = Quaternion.Euler(0, step, 0);
        // Apply the rotation to the target object
        target.rotation *= targetRotation;
        
    }
}
