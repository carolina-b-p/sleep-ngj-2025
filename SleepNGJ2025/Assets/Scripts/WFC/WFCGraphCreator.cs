using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;


public class TileDescription
{
    public string prefabName;
    public int rotation;

    public HashSet<TileDescription> leftNeighbors = new();
    public HashSet<TileDescription> rightNeighbors = new();
    public HashSet<TileDescription> upNeighbors = new();
    public HashSet<TileDescription>  downNeighbors = new();

    public string GetKey()
    {
        return prefabName + rotation;
    }
}

[Serializable]
public class SocketTileDescription
{
    public string prefabName;
    public int rotation;
    public string LeftSocket;
    public string RightSocket;
    public string UpSocket;
    public string DownSocket;
}

[Serializable]
public class SocketMap
{
    public List<SocketPrefabRotation> LeftSocketTiles = new();
    public List<SocketPrefabRotation> RightSocketTiles = new();
    public List<SocketPrefabRotation> UpSocketTiles = new();
    public List<SocketPrefabRotation> DownSocketTiles = new();
}

[Serializable]
public class SocketPrefabRotation
{
    public string Socket;
    public string Prefab;
    public int Rotation;

    public static implicit operator SocketPrefabRotation((string, string, int) tuple)
        => new()
        {
            Socket = tuple.Item1, Prefab = tuple.Item2, Rotation = tuple.Item3
        };
}

public class WFCGraphCreator : MonoBehaviour
{
    private string GetPrefabName(GameObject go)
    {
        return go.GetPrefabDefinition().name;
    }
    
    int GetRotationForTileDescription(GameObject go)
    {
        return (int)(((go.transform.rotation.eulerAngles.y + 720 + 45) % 360) / 90); // make sure to not have negative angles
    }
    
    private string GetTileDescription(GameObject go)
    {
        return GetPrefabName(go) + "-" + GetRotationForTileDescription(go);

    }
    
    private const string WFCTileTag = "WFCTile";
    private const int WFCTileSize = 25;
    
    // Start is called before the first frame update
    [ContextMenu("Create WFC Graph")]
    public void CreateWFCGraph()
    {
        var tiles = new Dictionary<(int, int), GameObject>();
        var gos = GameObject.FindGameObjectsWithTag(WFCTileTag);
        var tileDescriptions = new Dictionary<string, TileDescription>();
        foreach (var go in gos)
        {
            var key = ((int)(go.transform.position.x / WFCTileSize), (int)(go.transform.position.y / WFCTileSize));
            tiles.TryAdd(key, go);
        }

        foreach (var tile in tiles)
        {
            var desc = GetTileDescription(tile.Value);
            if (tileDescriptions.ContainsKey(desc))
                continue;
            
            tileDescriptions.Add(desc, new TileDescription
            {
                prefabName = GetPrefabName(tile.Value),
                rotation = GetRotationForTileDescription(tile.Value)
            });
        }

        foreach (var tile in tiles)
        {
            var x = tile.Key.Item1;
            var y = tile.Key.Item2;

            var leftCoordKey = (x - 1, y);
            var rightCoordKey = (x + 1, y);
            var upCoordKey = (x, y - 1);
            var downCoordKey = (x, y + 1);

            var hasLeft = tiles.TryGetValue(leftCoordKey, out var leftItem);
            var hasRight = tiles.TryGetValue(rightCoordKey, out var rightItem);
            var hasUp = tiles.TryGetValue(upCoordKey, out var upItem);
            var hasDown = tiles.TryGetValue(downCoordKey, out var downItem);

            if (hasLeft)
                tileDescriptions[GetTileDescription(tile.Value)].leftNeighbors
                    .Add(tileDescriptions[GetTileDescription(leftItem)]);
            if (hasRight)
                tileDescriptions[GetTileDescription(tile.Value)].rightNeighbors
                    .Add(tileDescriptions[GetTileDescription(rightItem)]);
            if (hasUp)
                tileDescriptions[GetTileDescription(tile.Value)].upNeighbors
                    .Add(tileDescriptions[GetTileDescription(upItem)]);
            if (hasDown)
                tileDescriptions[GetTileDescription(tile.Value)].downNeighbors
                    .Add(tileDescriptions[GetTileDescription(downItem)]);
        }

        var json = JsonUtility.ToJson(tileDescriptions, true);
        File.WriteAllBytes("Assets/TileDescriptions.json", Encoding.UTF8.GetBytes(json));
    }

    [ContextMenu("Create WFC Graph (sockets)")]
    public void CreateWFCGraphSockets()
    {
        var gos = GameObject.FindGameObjectsWithTag(WFCTileTag);
        var tileDescriptions = new List<SocketTileDescription>();
        
        foreach (var go in gos)
        {
            var tileDescriptor = go.gameObject.GetComponent<TileDescriptor>();
            tileDescriptions.Add(new SocketTileDescription
            {
                prefabName = GetPrefabName(go),
                rotation = 0,
                LeftSocket = tileDescriptor.LeftSocket,
                RightSocket = tileDescriptor.RightSocket,
                UpSocket = tileDescriptor.UpSocket,
                DownSocket = tileDescriptor.DownSocket,
            });
            tileDescriptions.Add(new SocketTileDescription
            {
                prefabName = GetPrefabName(go),
                rotation = 1,
                LeftSocket = tileDescriptor.DownSocket,
                RightSocket = tileDescriptor.UpSocket,
                UpSocket = tileDescriptor.LeftSocket,
                DownSocket = tileDescriptor.RightSocket,
            });
            tileDescriptions.Add(new SocketTileDescription
            {
                prefabName = GetPrefabName(go),
                rotation = 2,
                LeftSocket = tileDescriptor.RightSocket,
                RightSocket = tileDescriptor.LeftSocket,
                UpSocket = tileDescriptor.DownSocket,
                DownSocket = tileDescriptor.UpSocket,
            });
            tileDescriptions.Add(new SocketTileDescription
            {
                prefabName = GetPrefabName(go),
                rotation = 3,
                LeftSocket = tileDescriptor.UpSocket,
                RightSocket = tileDescriptor.DownSocket,
                UpSocket = tileDescriptor.RightSocket,
                DownSocket = tileDescriptor.LeftSocket,
            });
        }
        
        var socketMap = new SocketMap();
        foreach (var desc in tileDescriptions)
        {
            socketMap.LeftSocketTiles.Add((desc.LeftSocket, desc.prefabName, desc.rotation));
            socketMap.RightSocketTiles.Add((desc.RightSocket, desc.prefabName, desc.rotation));
            socketMap.UpSocketTiles.Add((desc.UpSocket, desc.prefabName, desc.rotation));
            socketMap.DownSocketTiles.Add((desc.DownSocket, desc.prefabName, desc.rotation));
        }
                
        var descJson = JsonUtility.ToJson(socketMap, true);
        File.WriteAllBytes("Assets/SocketedTileDescriptions.json", Encoding.UTF8.GetBytes(descJson));
    }
}
