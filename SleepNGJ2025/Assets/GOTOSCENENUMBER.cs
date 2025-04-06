using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOTOSCENENUMBER : MonoBehaviour
{
    public int sceneNumber; // The scene number to load
    // Start is called before the first frame update
    public void GoTo()
    {
        // Load the scene with the specified scene number
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneNumber);
    }
}
