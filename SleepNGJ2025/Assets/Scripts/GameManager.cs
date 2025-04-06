using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform player;
    public GameObject playerArrowPrefab;
    private PlayerController playerController;
    private float timeAtTarget = 0; //the time the player has been at the target
    private float lastTimeSleepBarUpdate = 0; //the last time the sleep bar was updated

    public TextMeshProUGUI deliveryDeadlineText; //the text that shows the delivery deadline

    public float deliveryTime = 0f; //the time the player has to deliver the package
    public float deadlineStart = 120f;
    public float deadline = 120f; //the time the player has to deliver the package
    public float deadlineChange  = 0.95f; //the time the player has to deliver the package
    
    [SerializeField] private float deliveryWaitTime = 1f; //the time the player needs to stay at the target to successfully deliver
    private void Start()
    {
        InitializeGame();
        lastTimeSleepBarUpdate = Time.deltaTime;
    }

    private void InitializeGame()
    {
        //instantiate arrow prefab inside player gameobject
        playerController = player.GetComponent<PlayerController>();
        GameObject playerArrow = Instantiate(playerArrowPrefab, player.position, Quaternion.identity);
        playerArrow.transform.SetParent(player);
        TargetManager.Instance.SelectNewTarget();
        deliveryTime = deadline;
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

                AudioManager.Instance.PlayCustomerSfx();
                
                TargetManager.Instance.PlayerAtTargetVisuals(true, timeAtTarget / (deliveryWaitTime * 0.8f));
                
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
            SleepManager.Instance.ChangeSleepAmount(40f * Mathf.Abs(Time.deltaTime-lastTimeSleepBarUpdate));
        }
        else if (!playerController.isAsleep && SleepManager.Instance.sleepAmount>=0)
        {
            SleepManager.Instance.ChangeSleepAmount(-15f * Mathf.Abs(Time.deltaTime-lastTimeSleepBarUpdate));
        }
        lastTimeSleepBarUpdate = Time.deltaTime;
    }
}
