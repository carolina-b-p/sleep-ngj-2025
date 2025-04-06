using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class GameManager : MonoBehaviour
{
    public Transform player;
    public GameObject playerArrowPrefab;
    private GameObject playerArrowInstance = null;
    private PlayerController playerController;
    private float timeAtTarget = 0; //the time the player has been at the target
    private float lastTimeSleepBarUpdate = 0; //the last time the sleep bar was updated

    public TextMeshProUGUI deliveryDeadlineText; //the text that shows the delivery deadline

    public float deliveryTime = 0f; //the time the player has to deliver the package
    public float deadlineStart = 120f;
    public float deadline = 120f; //the time the player has to deliver the package
    public float deadlineChange  = 5f; //the time the player has to deliver the package

    public bool gameover = false; //if the game is over or not

    public bool deadlineclose = false; //if the deadline is close or not
    public float deadlineCloseTime = 30f; //the time the player has to deliver the package

    public GameObject MenuButton; //the button to go back to the menu

    [SerializeField] private float deliveryWaitTime = 1f; //the time the player needs to stay at the target to successfully deliver
    private void Start()
    {
        StartCoroutine(InitializeGame());
        lastTimeSleepBarUpdate = Time.deltaTime;
    }

    IEnumerator InitializeGame()
    {
        //instantiate arrow prefab inside player gameobject
        playerController = player.GetComponent<PlayerController>();
        if (playerArrowInstance != null)
            DestroyImmediate(playerArrowInstance);
        playerArrowInstance = Instantiate(playerArrowPrefab, player.position, Quaternion.identity);
        var WFCGenerator = GameObject.FindGameObjectWithTag("WFCGenerator");
        WFCGenerator.GetComponent<WFCGraphGenerator>().Generate(UnityEngine.Random.Range(int.MinValue, int.MaxValue));
        yield return null;
        GraphManager.Instance.BuildGraph();
        yield return null;
        playerArrowInstance.transform.SetParent(player);
        TargetManager.Instance.SelectNewTarget();
        deadline = deadlineStart;
        deliveryTime = deadline;
        MenuButton.SetActive(false);
        gameover = false;
    }

    public void UpdateDeadlineText()
    {
        //update the deadline time
        if (deliveryTime > 0)
        {
            deliveryTime -= Time.deltaTime * Time.timeScale;
        }
        else
        {
            gameover = true;
            ResetDeadlineText();
            //TODO-- game over screen
            deliveryDeadlineText.text = "Game Over!!";
            MenuButton.SetActive(true);
            return;
        }

        //reset pusling 
        if (deliveryTime < deadlineCloseTime && !deadlineclose)
        {
            deadlineclose = true;
            StartCoroutine(DeadlineTextPulse());
        }
        else if (deliveryTime >= deadlineCloseTime && deadlineclose)
        {
            ResetDeadlineText();
            deadlineclose = false;
        }
        //write the time left as minutes and seconds
        TimeSpan time = TimeSpan.FromSeconds(deliveryTime);
        string timeString = string.Format("{0:D2}:{1:D2}", (int)time.TotalMinutes, time.Seconds);
        deliveryDeadlineText.text = timeString;
        //update the deadline text
    }

    //when picking up a package we want to set the deadline to a high value again but slightly lower than the last one
    public void OnDeliveryDone()
    {
        deadline = deadline - deadlineChange;
        deliveryTime = deadline;
        ResetDeadlineText();
    }

//reset the deadline text to white and stop the pulsing
// and reset the size of the text to normal white
    public void ResetDeadlineText()
    {
        deliveryDeadlineText.color = Color.white;
        deliveryDeadlineText.transform.localScale = Vector3.one;
        StopCoroutine(DeadlineTextPulse());
    }

    //a coroutine which makes the deadline text pulse and change color to red
    //preferably scale over time gradually on sine curve
    private IEnumerator DeadlineTextPulse()
    {
            deliveryDeadlineText.color = Color.red;
        while (true)
        {
            deliveryDeadlineText.transform.localScale = Vector3.one * Mathf.PingPong(Time.time, 0.2f) + Vector3.one;
            yield return null;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && Input.GetKeyDown(KeyCode.T))
        {
            Start();
        }
        
        UpdateDeadlineText();
        if(TargetManager.Instance.targetTransform != null)
        {
            //check if player is within range of the target
            float distance = Vector3.Distance(player.position, TargetManager.Instance.targetTransform.position);
            if (distance < TargetManager.Instance.targetRadius)
            {
                Debug.Log("Target reached!!");
                
                TargetManager.Instance.PlayerAtTargetVisuals(true, timeAtTarget / (deliveryWaitTime * 0.8f));
                
                //TODO-- delivery loading circle UI
                
                // Destroy the target indicator and select a new target
                timeAtTarget += Time.deltaTime;
                if (timeAtTarget >= deliveryWaitTime)
                {
                    Destroy(TargetManager.Instance.targetIndicator);
                    TargetManager.Instance.SelectNewTarget();
                    timeAtTarget = 0;
                    OnDeliveryDone();

                    AudioManager.Instance.PlayCustomerCheerSfx();
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
