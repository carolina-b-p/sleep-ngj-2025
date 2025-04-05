using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform player;
    public GameObject playerArrowPrefab;
    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        //instantiate arrow prefab inside player gameobject
        GameObject playerArrow = Instantiate(playerArrowPrefab, player.position, Quaternion.identity);
        playerArrow.transform.SetParent(player);
        TargetManager.Instance.SelectNewTarget();
    }

    private void Update()
    {
        if(TargetManager.Instance.targetTransform != null)
        {
            //check if player is within range of the target
            float distance = Vector3.Distance(player.position, TargetManager.Instance.targetTransform.position);
            if (distance < TargetManager.Instance.targetRadius)
            {
                Debug.Log("Target reached!!");
                // Destroy the target indicator and select a new target
                Destroy(TargetManager.Instance.targetIndicator);
                TargetManager.Instance.SelectNewTarget();
            }
        }
    }
}
