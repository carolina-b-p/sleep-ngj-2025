using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform player;
    public GameObject playerArrowPrefab;
    private PlayerController playerController;
    private float timeAtTarget = 0; //the time the player has been at the target
    private float deliveryWaitTime = 1f; //the time the player needs to stay at the target to successfully deliver
    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        //instantiate arrow prefab inside player gameobject
        playerController = player.GetComponent<PlayerController>();
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
                
                TargetManager.Instance.PlayerAtTargetVisuals(true);
                
                //TODO-- delivery loading circle UI
                
                // Destroy the target indicator and select a new target
                timeAtTarget += Time.deltaTime;
                if (timeAtTarget >= deliveryWaitTime)
                {
                    Destroy(TargetManager.Instance.targetIndicator);
                    TargetManager.Instance.SelectNewTarget();
                    timeAtTarget = 0;
                }
            }
            else
            {
                TargetManager.Instance.PlayerAtTargetVisuals(false);
                timeAtTarget = 0;
                //TODO-- Hide delivery loading circle UI
            }
        }

        HandleSleepBar();

    }

    private void HandleSleepBar()
    {
        //Automatically sleep and wake up
        if (SleepManager.Instance.sleepAmount <= 0 && !playerController.isAsleep)
        {
            playerController.isAsleep = true;
        }
        else if (SleepManager.Instance.sleepAmount >= 100 && playerController.isAsleep)
        {
            playerController.isAsleep = false;
        }
        // Change sleep amount based on player state
        if (playerController.isAsleep && SleepManager.Instance.sleepAmount<=100)
        {
            SleepManager.Instance.ChangeSleepAmount(1.5f * Time.deltaTime);
        }
        else if (!playerController.isAsleep && SleepManager.Instance.sleepAmount>=0)
        {
            SleepManager.Instance.ChangeSleepAmount(-1.5f * Time.deltaTime);
        }
        
    }
}
