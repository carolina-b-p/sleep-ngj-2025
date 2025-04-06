using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SleepManager : MonoBehaviour
{
    public Image sleepBarLevel;
    public float sleepAmount = 100f;
    public List<Color> sleepColors = new List<Color>();
    
    //singleton instance
    public static SleepManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
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
        // Update the color of the sleep bar based on the sleep amount
        if (sleepAmount / 100 <= 1 && sleepAmount / 100 > 0.75f)
        {
            sleepBarLevel.color = sleepColors[1];
        }
        else if (sleepAmount / 100 <= 0.75f && sleepAmount / 100 > 0.5f)
        {
            sleepBarLevel.color = sleepColors[2];
        }
        else if (sleepAmount / 100 <= 0.5f && sleepAmount / 100 > 0.25f)
        {
            sleepBarLevel.color = sleepColors[3];
        }
        else if (sleepAmount / 100 <= 0.25f && sleepAmount / 100 > 0.1f)
        {
            sleepBarLevel.color = sleepColors[4];
        }
        else if (sleepAmount / 100 <= 0.1f)
        {
            sleepBarLevel.color = sleepColors[5];
        }
    }

}
