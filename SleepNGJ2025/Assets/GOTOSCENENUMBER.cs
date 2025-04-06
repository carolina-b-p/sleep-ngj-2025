using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOTOSCENENUMBER : MonoBehaviour
{
    public int sceneNumber; // The scene number to load
    // Start is called before the first frame update
    public GameObject startButton, quitButton, returnToMM, CreditsButton, creditsText;


    public void GoTo()
    {
        // Load the scene with the specified scene number
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneNumber);
    }

    public void QuitGame()
    {
        Application.Quit();
        print("quit");
    } 

    public void OpenCredit()
    {
        startButton.SetActive(false);
        quitButton.SetActive(false);
        returnToMM.SetActive(true);
        creditsText.SetActive(true);
        CreditsButton.SetActive(false);
        print("Show credits");
    }

    public void returnToMainMenu()
    {
        startButton.SetActive(true);
        quitButton.SetActive(true);
        creditsText.SetActive(false);
        CreditsButton.SetActive(true);
        returnToMM.SetActive(false);
    }
}
