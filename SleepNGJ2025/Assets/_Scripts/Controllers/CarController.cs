using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    // A controller for handling driving a car, it is matched up with either a player controller or a npc controller which is responsible for giving stering acceleration and braking input
    //The car has four wheel controllers, one for each wheel, and a rigidbody for physics simulation.

    //the wheels are set up in a list, so we can easily add or remove wheels without changing the code. In the order of front right, front left, back right, back left.

    public float accelerationInput = 0f; // Acceleration input value, between 0 and 1
    public float steeringInput = 0f; // Steering input value, between -1 and 1
    public float brakeInput = 0f; // Brake input value, between 0 and 1

    private Rigidbody rb; // Reference to the Rigidbody component
    public List<WheelCollider> wheelColliders; // List of WheelColliders for the car's wheels
    public List<Transform> wheelMeshes; // List of Transforms for the car's wheel meshes
    
    public float maxMotorTorque = 1500f; // Maximum motor torque applied to the wheels
    public float maxSpeed = 200f; // Maximum speed of the car, it will be capped at this speed, if the car goes faster than this speed, it will be slowed down to this speed
    public float currentSpeed = 0f; // Current speed of the car, it is used to calculate the steering angle based on the speed of the car
    public float maxSteeringAngle = 30f; // Maximum steering angle for the front wheels when the car goes the slowest
    public float minSteeringAngle = 10f; // Minimum steering angle for the front wheels when the car goes the fastest
    public AnimationCurve steeringAngleSpeedCurve; // Animation curve to control the steering angle based on speed, goes from 0 to 1, where 0 is the minSteeringAngle and 1 is the maxSteeringAngle
    public float maxSteeringSpeed = 200f; // Speed of the car when it steers the least, this is the speed at which the steering angle is the maxSteeringAngle
    public float minSteeringSpeed = 50f; // Speed of the car when it steers the most, this is the speed at which the steering angle is the minSteeringAngle
    public float brakeTorque = 3000f; // Brake torque applied to the wheels when braking

    public Vector3 centerOfMassOffset = new Vector3(0f, -0.5f, 0f); // Offset for the center of mass of the car, this is used to make the car more stable and less likely to flip over

    //the function called by other scripts to move the car, it takes in acceleration and brake input values which are between 0 and 1, and steering which is between -1 and 1.
    public void Move(float accelerationInput, float steeringInput, float brakeInput)
    {
        this.accelerationInput = accelerationInput; // Set the acceleration input value
        this.steeringInput = steeringInput; // Set the steering input value
        this.brakeInput = brakeInput; // Set the brake input value
    }
        
    public void Update(){

        //player input shoud deteriorate if no input is given, so we will set the acceleration input to 0 if it is less than 0.1f, this will make the car slow down if no input is given.
        if (MathF.Abs(accelerationInput) < 0.1f)
        {
            brakeInput = Mathf.Max(brakeInput,0.1f);
        }

        //if the car is mooving in the opposite direction of the acceleration input, we will make the car brake as well, this will make the car slow down if it is going in the opposite direction of the acceleration input.
        //this should work both for accelerating forwards and reversing (accelerating negatively)
        //we check this by using dot product between the car's velocity and the acceleration input, if the dot product is less than 0, it means the car is going in the opposite direction of the acceleration input.
        //this will make the car slow down if it is going in the opposite direction of the acceleration input.
        if (Vector3.Dot(rb.velocity, transform.forward) < 0f && accelerationInput > 0f)
        {
            brakeInput = 1f;
        }

        // Calculate the motor torque based on the acceleration input and the maximum motor torque
        float motorTorque = accelerationInput * maxMotorTorque;

        // Apply the motor torque to the rear wheels (index 2 and 3 in the wheelColliders list)
        wheelColliders[2].motorTorque = motorTorque;
        wheelColliders[3].motorTorque = motorTorque;

        // Calculate the steering angle based on the steering input and the current speed of the car
        float currentSpeed = rb.velocity.magnitude * 3.6f; // Convert from m/s to km/h
        float steeringAngle = Mathf.Lerp(minSteeringAngle, maxSteeringAngle, steeringAngleSpeedCurve.Evaluate(currentSpeed / maxSpeed)) * steeringInput;

        // Apply the steering angle to the front wheels (index 0 and 1 in the wheelColliders list)
        wheelColliders[0].steerAngle = steeringAngle;
        wheelColliders[1].steerAngle = steeringAngle;

        //if the car is not accelerating in either direction, it will brake slightly to slow down.
        if (MathF.Abs(accelerationInput) < 0.1f)
        {
            // Apply a small brake torque to all wheels to slow down the car
            foreach (WheelCollider wheelCollider in wheelColliders)
            {
                wheelCollider.brakeTorque = brakeTorque * 0.1f;
            }
        }
        // Apply brake torque to all wheels if brake input is greater than 0
        if (brakeInput > 0f)
        {
            foreach (WheelCollider wheelCollider in wheelColliders)
            {
                wheelCollider.brakeTorque = brakeTorque * brakeInput;
            }
        }
        else
        {
            // Reset brake torque to zero when not braking
            foreach (WheelCollider wheelCollider in wheelColliders)
            {
                wheelCollider.brakeTorque = 0f;
            }
        }

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
