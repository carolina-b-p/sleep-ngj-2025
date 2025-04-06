using System;
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
            var intermediateTarget = GraphManager.Instance.UpdatePathAndGetIntermediateTarget(target.position);
            
            if (!intermediateTarget.Item1)
            {
                Debug.Log("Unreachable target detected, moving it!");
                TargetManager.Instance.SelectNewTarget();
                return;
            }

            if (intermediateTarget.Item2 > 15)
            {
                Debug.Log("Target is more than 15 tiles away, resetting it!");
                TargetManager.Instance.SelectNewTarget();
                return;
            }

            Vector3 direction = intermediateTarget.Item3 - transform.position;
            direction.y = 0; // Ignore vertical difference

            // Calculate the rotation step
            float step = rotationSpeed * Time.deltaTime;

            // Calculate the new rotation
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);
        }
    }
}
