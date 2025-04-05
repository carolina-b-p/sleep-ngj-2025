using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SleepManager : MonoBehaviour
{
    public Image sleepBarLevel;
    public float sleepAmount = 100f;
    
    //singleton instance
    public static SleepManager Instance { get; private set; }
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
    public void ChangeSleepAmount(float amount)
    {
        sleepAmount += amount;
        UpdateSleepBar();
    }
    
    public void UpdateSleepBar()
    {
        sleepBarLevel.fillAmount = sleepAmount / 100f;
    }

}
