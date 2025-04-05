using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChanceToDieOnStartup : MonoBehaviour
{
    public float ChanceToLive = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        if (Random.Range(0, 1.0f) > ChanceToLive)
            Destroy(gameObject);
    }
}
