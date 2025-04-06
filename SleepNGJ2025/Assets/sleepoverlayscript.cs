using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sleepoverlayscript : MonoBehaviour
{
    //i have a canvasgroup and want to update the alpa by lerping towards the target alpha
    public CanvasGroup canvasGroup;
    public CarController carController;
    // Start is called before the first frame update
    void Start()
    {
        carController = FindObjectOfType<CarController>();    
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f; // Set initial alpha to 0
    }

    // Update is called once per frame
    void Update()
    {
        var targetAlpha = carController.isSleeping ? 1f : 0f;
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, 5f * Time.deltaTime);
    }
}
