using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDescriptor : MonoBehaviour
{
    [SerializeField] public string UpSocket = string.Empty;
    [SerializeField] public string RightSocket = string.Empty;
    [SerializeField] public string DownSocket = string.Empty;
    [SerializeField] public string LeftSocket = string.Empty;
    [SerializeField] public int Weight = 1;

    [SerializeField] public List<GameObject> DeliveryLocations = new();
}
