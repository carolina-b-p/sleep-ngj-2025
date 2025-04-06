using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnoozeTester : MonoBehaviour
{
    public ParticleSystem particles; // Reference to the ParticleSystem component
    public CarController carController; // Reference to the CarController script
    // Update is called once per frame
    void Update()
    {
        //start or stop the particle system based on wether the car is asleep or not
        if (carController.isSleeping && !particles.isPlaying) // Check if the car is asleep and the particle system is not playing
        {
            particles.Play(); // Start the particle system
        }
        else if (!carController.isSleeping && particles.isPlaying) // Check if the car is not asleep and the particle system is playing
        {
            particles.Stop(); // Stop the particle system
        }
    }
}
