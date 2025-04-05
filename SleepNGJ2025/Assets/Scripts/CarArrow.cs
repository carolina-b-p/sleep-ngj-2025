using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarArrow : MonoBehaviour
{
    //arrow rotates pointing towards target
    public Transform target;
    public float rotationSpeed = 5f; // Speed of rotation
    
    void Update()
    {
        if(target != TargetManager.Instance.targetTransform)
        {
            // Find the target in the scene
            target = TargetManager.Instance.targetTransform;
        }
        if (target != null)
        {
            // Calculate the direction to the target
            Vector3 direction = target.position - transform.position;
            direction.y = 0; // Ignore vertical difference

            // Calculate the rotation step
            float step = rotationSpeed * Time.deltaTime;

            // Calculate the new rotation
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);
        }
    }
}
