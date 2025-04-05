using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TargetManager : MonoBehaviour
{
    public GameObject targetsParent;
    public GameObject targetIndicatorPrefab;
    public Transform targetTransform;
    public GameObject targetIndicator;
    public float targetRadius = 3f;

    //Singleton instance
    public static TargetManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SelectNewTarget()
    {
        var targets = GameObject.FindGameObjectsWithTag("DeliveryLocation");
        var index = Random.Range(0, targets.Length);
        targetTransform = targets[index].transform;
        
        //
        // if (targetsParent.transform.childCount == 0)
        //     return;
        // int randomIndex = Random.Range(0, targetsParent.transform.childCount);
        // targetTransform = targetsParent.transform.GetChild(randomIndex);
        targetIndicator = Instantiate(targetIndicatorPrefab, targetTransform.position, Quaternion.identity);
    }
    public void PlayerAtTargetVisuals(bool isAtTarget)
    {
        // Change the color of the target indicator to green
        if (targetIndicator != null)
        {
            targetIndicator.GetComponent<Renderer>().material.color = isAtTarget ? Color.green : Color.yellow;
        }
    }
    
}
