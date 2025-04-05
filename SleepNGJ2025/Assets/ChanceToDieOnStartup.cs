using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChanceToDieOnStartup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (Random.Range(0, 1.0f) < 0.995f)
            Destroy(gameObject);
    }
}
