using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using Vector3 = System.Numerics.Vector3;

public class DeleteNearbyLamppostsOnSpawn : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var allLampposts = GameObject.FindGameObjectsWithTag("Lamppost");
        var myDistance = transform.position.magnitude;
        foreach (var otherLamppost in allLampposts)
        {
            var otherDistance = otherLamppost.transform.position.magnitude;
            if (myDistance < otherDistance)
            {
                if (Mathf.Abs(myDistance - otherDistance) > 12f)
                    continue;
                
                var luck = UnityEngine.Random.Range(0, 1.0f);
                if (luck < 0.5f)
                {
                    Destroy(this);
                }
                else
                {
                    Destroy(otherLamppost);
                }
            }
        }
    }
}
