using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundAxis : MonoBehaviour
{
    public Vector3 axis = Vector3.up;
    public float rotationSpeed = 1.5f;

    // Update is called once per frame
    void Update()
    {
        var rotationQuat = Quaternion.AngleAxis(rotationSpeed, axis);
        transform.localRotation *= rotationQuat;
    }
}
