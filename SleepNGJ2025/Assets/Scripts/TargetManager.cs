using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class TargetManager : MonoBehaviour
{
    private static readonly int BoxColor = Shader.PropertyToID("_BoxColor");
    //public GameObject targetsParent;
    public GameObject targetIndicatorPrefab;
    public Transform targetTransform;
    public GameObject targetIndicator;
    public float targetRadius = 3f;
    [ColorUsage(true, true)]
    public Color notInUseColor;
    [ColorUsage(true, true)]
    public Color inUseColor;

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
        if (targetIndicator != null)
            Destroy(targetIndicator);
        targetIndicator = null;
        
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
    public void PlayerAtTargetVisuals(bool isAtTarget, float completionPercent = 0.0f)
    {
        // Change the color of the target indicator to green
        if (targetIndicator != null)
        {
            targetIndicator.GetComponent<MeshRenderer>().material.SetColor(BoxColor, isAtTarget ? Color.Lerp(notInUseColor, inUseColor, completionPercent) : notInUseColor);
        }
    }
    
}
