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
        if (targetsParent.transform.childCount == 0)
            return;
        int randomIndex = Random.Range(0, targetsParent.transform.childCount);
        targetTransform = targetsParent.transform.GetChild(randomIndex);
        targetIndicator = Instantiate(targetIndicatorPrefab, targetTransform.position, Quaternion.identity);
    }

    
}
