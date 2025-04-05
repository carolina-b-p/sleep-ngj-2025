using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Acontroller for managing player input in a car driving game, it works for bodth wasd and controller inputs bu utilizing unitys input system.
    // the actual speed and turning of the car is managed by the CarController script, this script only handles input.
    // The car controller script is attached to the same game object as this script, so we can access it directly.
    
    public CarController carController; // Reference to the CarController script
    public float accelerationInput = 0f; // Acceleration input value
    public float steeringInput = 0f; // Steering input value
    public float handbrakeInput = 0f; // Brake input value

    void Start()
    {
        // Get the CarController component attached to the same GameObject
        carController = GetComponent<CarController>();
    }

    void Update()
    {
        // Get input from WASD keys or controller
        accelerationInput = Input.GetAxis("Acceleration");
        steeringInput = Input.GetAxis("Horizontal"); 
        handbrakeInput = Input.GetAxis("Brake"); 

        // Pass the input values to the CarController script
        carController.Move(accelerationInput, steeringInput, handbrakeInput);
    }
}
