using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class CarController : MonoBehaviour
{
    // A controller for handling driving a car, it is matched up with either a player controller or a npc controller which is responsible for giving stering acceleration and braking input
    //The car has four wheel controllers, one for each wheel, and a rigidbody for physics simulation.

    //the wheels are set up in a list, so we can easily add or remove wheels without changing the code. In the order of front right, front left, back right, back left.

    public float acceleration = 0f; // Acceleration input value, between 0 and 1
    public float steering = 0f; // Steering input value, between -1 and 1
    public float brake = 0f; // Brake input value, between 0 and 1
    public float handbrake = 0f; // Brake input value, between 0 and 1

    private Rigidbody rb; // Reference to the Rigidbody component
    public List<WheelCollider> wheelColliders; // List of WheelColliders for the car's wheels
    public List<Transform> wheelMeshes; // List of Transforms for the car's wheel meshes
    
    public float maxMotorTorque = 1500f; // Maximum motor torque applied to the wheels
    public float maxSpeed = 200f; // Maximum speed of the car, it will be capped at this speed, if the car goes faster than this speed, it will be slowed down to this speed
    public float currentSpeed = 0f; // Current speed of the car, it is used to calculate the steering angle based on the speed of the car
    public float steeringAngle = 0f;

    public float frontWheelSpeed = 0f;
    public float backWheelSpeed = 0f;

    public float maxSteeringAngle = 30f; // Maximum steering angle for the front wheels when the car goes the slowest
    public float minSteeringAngle = 10f; // Minimum steering angle for the front wheels when the car goes the fastest
    public AnimationCurve steeringAngleSpeedCurve; // Animation curve to control the steering angle based on speed, goes from 0 to 1, where 0 is the minSteeringAngle and 1 is the maxSteeringAngle
    public float maxSteeringSpeed = 200f; // Speed of the car when it steers the least, this is the speed at which the steering angle is the maxSteeringAngle
    public float minSteeringSpeed = 50f; // Speed of the car when it steers the most, this is the speed at which the steering angle is the minSteeringAngle
    public float brakeTorque = 3000f; // Brake torque applied to the wheels when braking

    public Vector3 centerOfMassOffset = new Vector3(0f, -0.5f, 0f); // Offset for the center of mass of the car, this is used to make the car more stable and less likely to flip over

    //the function called by other scripts to move the car, it takes in acceleration and brake input values which are between 0 and 1, and steering which is between -1 and 1.
    public void Move(float accelerationInput, float steeringInput, float handbrakeInput, bool isAsleep)
    {
        // Set the acceleration, steering, and brake input values based on the input from other scripts
        acceleration = accelerationInput; // Set the acceleration input value
        steering = steeringInput; // Set the steering input value
        handbrake = handbrakeInput; // Set the brake input value
    }
        
    public void Update(){

        frontWheelSpeed = wheelColliders[0].rpm * (wheelColliders[0].radius * 2 * Mathf.PI) * 60f / 1000f ; // Calculate the speed of the front wheels in km/h
        backWheelSpeed = wheelColliders[2].rpm * (wheelColliders[2].radius * 2 * Mathf.PI) * 60f / 1000f; // Calculate the speed of the back wheels in km/h

        //if the car is mooving in the opposite direction of the acceleration input, we will make the car brake as well, this will make the car slow down if it is going in the opposite direction of the acceleration input.
        //this should work both for accelerating forwards and reversing (accelerating negatively)
        //we check this by using dot product between the car's velocity and the acceleration input, if the dot product is less than 0, it means the car is going in the opposite direction of the acceleration input.
        //this will make the car slow down if it is going in the opposite direction of the acceleration input.
        
        //player input shoud deteriorate if no input is given, so we will set the acceleration input to 0 if it is less than 0.1f, this will make the car slow down if no input is given.
        if (MathF.Abs(acceleration) < 0.1f)
        {
            brake = 1f;

        }else if (rb.velocity.magnitude > 0.25f && Vector3.Dot(rb.velocity, transform.forward * acceleration) < 0f)
        {
            brake = 1;
        } else
        {
            brake = 0;
        }

        


        // Calculate the steering angle based on the steering input and the current speed of the car
        currentSpeed = rb.velocity.magnitude * 3.6f; // Convert from m/s to km/h
        //steering amount should be calculated as the current speed inverse lerped between minsteering speed and max steering speed
        float steeringAmount = Mathf.InverseLerp(maxSteeringSpeed, minSteeringSpeed, currentSpeed);
        float targetsteeringAngle = Mathf.Lerp(minSteeringAngle, maxSteeringAngle, steeringAngleSpeedCurve.Evaluate(steeringAmount)) * steering;
        steeringAngle = Mathf.Lerp(steeringAngle, targetsteeringAngle, Time.deltaTime * 5f); // Smoothly interpolate the steering angle

        // Apply the steering angle to the front wheels (index 0 and 1 in the wheelColliders list)
        wheelColliders[0].steerAngle = steeringAngle;
        wheelColliders[1].steerAngle = steeringAngle;

        //if the car is not accelerating in either direction, it will brake slightly to slow down.
        if (MathF.Abs(acceleration) < 0.1f)
        {
            // Apply a small brake torque to all wheels to slow down the car
            foreach (WheelCollider wheelCollider in wheelColliders)
            {
                wheelCollider.brakeTorque = brakeTorque * 0.1f;
            }
        }
        // Apply handbrake torque to all wheels if handbrake input is greater than 0
        //otherwise, apply brake torque only to the back wheels (index 2 and 3 in the wheelColliders list)
        if (handbrake > 0f)
        {
            acceleration = 0;
            for (int i = 0; i < wheelColliders.Count; i++)
            {
                WheelCollider wheelCollider = wheelColliders[i];
                if (i == 2 || i == 3) // Back wheels
                {
                    wheelCollider.brakeTorque = brakeTorque * handbrake;
                }
                else // Front wheels
                {
                    wheelCollider.brakeTorque = 0f; // No brake torque for front wheels when braking
                }
            }
        }
        else if (brake > 0f)
        {
            acceleration = 0;
            for (int i = 0; i < wheelColliders.Count; i++)
            {
                WheelCollider wheelCollider = wheelColliders[i];
                if (i == 2 || i == 3) // Back wheels
                {
                    wheelCollider.brakeTorque = brakeTorque * brake;
                }
                else // Front wheels
                {
                    wheelCollider.brakeTorque = 0f; // No brake torque for front wheels when braking
                }
            }
        }
        else
        {
            foreach (WheelCollider wheelCollider in wheelColliders)
            {
                wheelCollider.brakeTorque = 0f; // No brake torque when not braking
            }
        }

        
        // Calculate the motor torque based on the acceleration input and the maximum motor torque
        float motorTorque = acceleration * maxMotorTorque;

        // Apply the motor torque to the rear wheels (index 2 and 3 in the wheelColliders list)
        wheelColliders[2].motorTorque = motorTorque;
        wheelColliders[3].motorTorque = motorTorque;

        // Update the visual representation of the wheels
        UpdateWheelMeshes();
    }

    public void UpdateWheelMeshes()
    {
        // Loop through each wheel collider and update the corresponding wheel mesh
        for (int i = 0; i < wheelColliders.Count; i++)
        {
            WheelCollider wheelCollider = wheelColliders[i];
            Transform wheelMesh = wheelMeshes[i];

            // Get the position and rotation of the wheel collider
            Vector3 position;
            Quaternion rotation;
            wheelCollider.GetWorldPose(out position, out rotation);

            // Set the position and rotation of the wheel mesh to match the wheel collider
            wheelMesh.position = position;
            wheelMesh.rotation = rotation;
        }
    }
    void Start()
    {
        // Get the Rigidbody component attached to the car
        rb = GetComponent<Rigidbody>();

        // Set the center of mass of the car to be lower for better stability
        rb.centerOfMass = centerOfMassOffset;
    }
}
