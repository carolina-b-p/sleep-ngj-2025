using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOTOSCENENUMBER : MonoBehaviour
{
    public int sceneNumber; // The scene number to load
    // Start is called before the first frame update
    public GameObject startButton, quitButton, returnToMM, CreditsButton, movedCreditsPos, creditsStartPos;

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
        CreditsButton.transform.position = movedCreditsPos.transform.position;
        print("Show credits");
    }

    public void returnToMainMenu()
    {
        startButton.SetActive(true);
        quitButton.SetActive(true);
        returnToMM.SetActive(false);
        CreditsButton.transform.position = creditsStartPos.transform.position;

    }
}
