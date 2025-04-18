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

    public bool isSleeping = false; // Flag to indicate if the car is asleep, this is used to make the car more stable and less likely to flip over

    public Vector3 centerOfMassOffset = new Vector3(0f, -0.5f, 0f); // Offset for the center of mass of the car, this is used to make the car more stable and less likely to flip over

    //the function called by other scripts to move the car, it takes in acceleration and brake input values which are between 0 and 1, and steering which is between -1 and 1.
    public void Move(float accelerationInput, float steeringInput, float handbrakeInput, bool isAsleep)
    {
        // if the driver is asleeep the acceleration should start increasing over time towards 0.5f.
        // furthermore the car should start to swerve randomly, maybe using a perlin noise function to get a random value between -1 and 1

        if (isAsleep)
        {
            acceleration = Mathf.Lerp(acceleration, 1f, Time.deltaTime * 0.2f); // Increase the acceleration input towards 0.5f over time
            steering = (Mathf.PerlinNoise(Time.time, 0) - 0.5f)*2f; // Randomly swerve the car using Perlin noise
            handbrake = 0f; // Set handbrake input to 0 when the driver is asleep
        }
        else
        {
            // Set the acceleration, steering, and brake input values based on the input from other scripts
            acceleration = accelerationInput; // Set the acceleration input value
            steering = steeringInput; // Set the steering input value
            handbrake = handbrakeInput; // Set the brake input value
        }
        
        isSleeping = isAsleep;
    }
        
    public void Update(){

        frontWheelSpeed = wheelColliders[0].rpm * (wheelColliders[0].radius * 2 * Mathf.PI) * 60f / 1000f ; // Calculate the speed of the front wheels in km/h
        backWheelSpeed = wheelColliders[2].rpm * (wheelColliders[2].radius * 2 * Mathf.PI) * 60f / 1000f; // Calculate the speed of the back wheels in km/h

        //if the car is mooving in the opposite direction of the acceleration input, we will make the car brake as well, this will make the car slow down if it is going in the opposite direction of the acceleration input.
        //this should work both for accelerating forwards and reversing (accelerating negatively)
        //we check this by using dot product between the car's velocity and the acceleration input, if the dot product is less than 0, it means the car is going in the opposite direction of the acceleration input.
        //this will make the car slow down if it is going in the opposite direction of the acceleration input.
        
        if (!isSleeping)
        {
            // Calculate the brake input based on the acceleration input and the car's velocity
            if (acceleration > 0f && rb.velocity.magnitude > 0.25f && Vector3.Dot(rb.velocity.normalized, transform.forward) < 0f)
            {
                brake = 1f; // Apply full brake if going in the opposite direction of forward acceleration
            }
            else if (acceleration < 0f && rb.velocity.magnitude > 0.25f && Vector3.Dot(rb.velocity.normalized, -transform.forward) < 0f)
            {
                brake = 1f; // Apply full brake if going in the opposite direction of reverse acceleration
            }
            else
            {
                brake = 0f; // No brake if not going in the opposite direction of acceleration
            }
        }

        // Calculate the steering angle based on the steering input and the current speed of the car
        currentSpeed = rb.velocity.magnitude * 3.6f; // Convert from m/s to km/h

        //steering amount should be calculated as the current speed inverse lerped between minsteering speed and max steering speed
        // if handbraking we shouldnt limit steering, so we set the steering amount to 1f
        float steeringAmount = Mathf.InverseLerp(maxSteeringSpeed, minSteeringSpeed, currentSpeed);
        if (handbrake > 0f)
        {
            steeringAmount = 1f; // Set to 1 when handbraking
        }
        float targetsteeringAngle = Mathf.Lerp(minSteeringAngle, maxSteeringAngle, steeringAngleSpeedCurve.Evaluate(steeringAmount)) * steering;
        steeringAngle = Mathf.Lerp(steeringAngle, targetsteeringAngle, Time.deltaTime * 5f); // Smoothly interpolate the steering angle

        // Apply the steering angle to the front wheels (index 0 and 1 in the wheelColliders list)
        wheelColliders[0].steerAngle = steeringAngle;
        wheelColliders[1].steerAngle = steeringAngle;

        //reset the brake torque to 0
        foreach (WheelCollider wheelCollider in wheelColliders)
        {
            wheelCollider.brakeTorque = 0f; // No brake torque when not braking
        }

        if (!isSleeping)
        {
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
                for (int i = 0; i < wheelColliders.Count; i++)
                {
                    WheelCollider wheelCollider = wheelColliders[i];
                    if (i == 0 || i == 1) // Front wheels
                    {
                        wheelCollider.brakeTorque = brakeTorque * handbrake * 0.25f;
                    }
                    else // Front wheels
                    {
                        wheelCollider.brakeTorque = 0f; // No brake torque for front wheels when braking
                    }
                }
            }
        }

        if (brake > 0f)
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

        //if the handbrake is held we wand to apply a slight sideways force to the front of the car in the direction of the steering but it should be lowered when car speed is lowered
        // the back of the car should also be pushed in the same direction as the front of the car, but it should be less than the front of the car, this will make the car feel more stable and less likely to flip over.
        if (handbrake > 0f && currentSpeed > 10f)
        {
            Vector3 sidewaysForce = transform.right * steering * handbrake * .5f * Mathf.InverseLerp(0, maxSpeed, currentSpeed+10);
            rb.AddForceAtPosition(sidewaysForce * 1, (wheelColliders[0].transform.position + wheelColliders[1].transform.position)/2, ForceMode.Force);
            
            rb.AddForceAtPosition(sidewaysForce * -2f, (wheelColliders[2].transform.position + wheelColliders[3].transform.position) / 2, ForceMode.Acceleration);

        }

        // Calculate the motor torque based on the acceleration input and the maximum motor torque
        //motor torque should decrease towards 0 as the player nears the max speed
        float motorTorque = acceleration * maxMotorTorque * (1 - Mathf.InverseLerp(0, maxSpeed, currentSpeed)); // Calculate the motor torque based on the acceleration input and the maximum motor torque

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
        rb.centerOfMass += centerOfMassOffset;
    }
}
