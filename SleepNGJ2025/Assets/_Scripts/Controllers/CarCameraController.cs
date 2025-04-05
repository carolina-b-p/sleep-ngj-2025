using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCameraController : MonoBehaviour
{
    //a controller for a third person camera in a car driving game, the camera should follow the car around but avoid rotating in other axis than the y axis
    //the camera should zoom in slightly when the car is going slower and zoom out when going faster, this will make the camera feel more dynamic and less static.
    //the camera should also be a bit lazy to rotate with the ar to make it more smooth and feel more fluid.
    
    public CarController targetCarController; // Reference to the CarController script of the target car, allows us to retrieve the speed and position of the car

    public float distance = 5f; // Distance from the target car
    public float height = 2f; // Height above the target car
    public Vector3 cameraOffset = Vector3.zero; // Offset relative to the target car's rotation

    public float rotationDamping = 3f; // Damping for the rotation of the camera
    public float minDistance = 2f; // Minimum distance from the target car
    public float maxDistance = 10f; // Maximum distance from the target car
    
    public float minDistanceSpeed = 10f; // Minimum distance from the target car
    public float maxDistanceSpeed = 50; // Maximum distance from the target car

    public void FixedUpdate()
    {
        //the distance is set to be a value in between minDistance and maxDistance, this will make the camera zoom in when the car is going slower and zoom out when going faster.
        float distanceLerp  = Mathf.InverseLerp(minDistanceSpeed, maxDistanceSpeed, targetCarController.currentSpeed);
        distance = Mathf.Lerp(minDistance, maxDistance, distanceLerp); // Lerp the distance based on the car's speed

        // Get the target car's position and rotation
        Vector3 targetPosition = targetCarController.transform.position + Vector3.up * height - targetCarController.transform.forward * distance;

        // Apply the camera offset relative to the target's rotation
        targetPosition += targetCarController.transform.rotation * cameraOffset;

        Quaternion targetRotation = Quaternion.LookRotation(targetCarController.transform.position - transform.position, Vector3.up);

        // Calculate the desired position and rotation of the camera
        Vector3 desiredPosition = targetPosition;
        Quaternion desiredRotation = targetRotation;

        // Smoothly interpolate the camera's position and rotation towards the desired position and rotation
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * rotationDamping);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * rotationDamping);
    }
}
