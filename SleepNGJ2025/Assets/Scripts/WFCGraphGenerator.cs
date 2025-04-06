using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class SocketToTiles
{
    public Dictionary<string, HashSet<(string, int)>> leftSocketTiles = new ();
    public Dictionary<string, HashSet<(string, int)>> rightSocketTiles = new ();
    public Dictionary<string, HashSet<(string, int)>> topSocketTiles = new ();
    public Dictionary<string, HashSet<(string, int)>> bottomSocketTiles = new ();
    public HashSet<(string, int)> allTiles = new();
}
public class WFCGraphGenerator : MonoBehaviour
{
    public TextAsset jsonFileToLoad;
    public int mapSizeX;
    public int mapSizeY;
    public List<GameObject> allPrefabs;
    public Dictionary<string, GameObject> PrefabReferenceMap;
    public int seed;
    public GameObject edgePrefab;

    [ContextMenu("Generate WFC thing")]
    public void Generate(int? maybeSeed = null)
    {
        var mySeed = seed;
        if (maybeSeed.HasValue)
            seed = maybeSeed.Value;
        PrefabReferenceMap = new Dictionary<string, GameObject>();
        foreach (var prefab in allPrefabs)
        {
            PrefabReferenceMap.Add(prefab.name, prefab); 
        }

        DestroyImmediate(GameObject.FindGameObjectWithTag("WFCParent"));

        var WFCParent = new GameObject("WFCParent");
        WFCParent.tag = "WFCParent";
        WFCParent.transform.position = Vector3.zero;
        WFCParent.transform.rotation = Quaternion.identity;
        
        Random.InitState(mySeed);
        // Create structure that stores set pieces
        Dictionary<(int, int), GameObject> map = new ();

        for (var y = 0; y < mapSizeY; y++)
        {
            for (var x = 0; x < mapSizeX; x++)
            {
                map.Add((x, y), null);
            }
        }

        // Collect set pieces from the scene
        var setPieces = GameObject.FindGameObjectsWithTag("WFCTile");
        foreach (var setPiece in setPieces)
        {
            var xPos = ((int)(setPiece.transform.position.x + 0.5)) / 25;
            var yPos = ((int)(setPiece.transform.position.z + 0.5)) / 25;

            map[(xPos, yPos)] = setPiece;
        }

        
        map[(0, 0)] = Instantiate(edgePrefab,
            new Vector3(0 * 25, 0, (0) * 25),
            Quaternion.Euler(0, 0, 0), WFCParent.transform);
        
        
        map[(0, mapSizeY-1)] = Instantiate(edgePrefab,
            new Vector3(0 * 25, 0, (mapSizeY-1) * 25),
            Quaternion.Euler(0, 0, 0), WFCParent.transform);
        
        map[(mapSizeX - 1, mapSizeY-1)] = Instantiate(edgePrefab,
            new Vector3((mapSizeX - 1) * 25, 0, (mapSizeY-1) * 25),
            Quaternion.Euler(0, 0, 0), WFCParent.transform);
        
        map[(mapSizeX - 1, 0)] = Instantiate(edgePrefab,
            new Vector3((mapSizeX - 1) * 25, 0, 0 * 25),
            Quaternion.Euler(0, 0, 0), WFCParent.transform);
        
        for (var xPos = 1; xPos < mapSizeX-1; xPos++)
        {
            map[(xPos, 0)] = Instantiate(edgePrefab,
                new Vector3(xPos * 25, 0, 0 * 25),
                Quaternion.Euler(0, 0, 0), WFCParent.transform);
            
            map[(xPos, mapSizeY-1)] = Instantiate(edgePrefab,
                new Vector3(xPos * 25, 0, (mapSizeY-1) * 25),
                Quaternion.Euler(0, 0, 0), WFCParent.transform);
        }
        for (var yPos = 1; yPos < mapSizeY-1; yPos++)
        {
            map[(0, yPos)] = Instantiate(edgePrefab,
                new Vector3(0 * 25, 0, yPos * 25),
                Quaternion.Euler(0, 0, 0), WFCParent.transform);
            
            map[(mapSizeX - 1, yPos)] = Instantiate(edgePrefab,
                new Vector3((mapSizeX - 1) * 25, 0, yPos * 25),
                Quaternion.Euler(0, 0, 0), WFCParent.transform);
        }

        // Load json into object
        var socketMapFlat = JsonUtility.FromJson<SocketMap>(jsonFileToLoad.text);

        // Convert json into dictionary
        var socketMap = new SocketToTiles();
        foreach (var leftSocket in socketMapFlat.LeftSocketTiles)
        {
            if (!socketMap.leftSocketTiles.ContainsKey(leftSocket.Socket))
                socketMap.leftSocketTiles.Add(leftSocket.Socket, new HashSet<(string, int)>());
            socketMap.leftSocketTiles[leftSocket.Socket].Add((leftSocket.Prefab, leftSocket.Rotation));
            socketMap.allTiles.Add((leftSocket.Prefab, leftSocket.Rotation));
        }
        foreach (var rightSocket in socketMapFlat.RightSocketTiles)
        {
            if (!socketMap.rightSocketTiles.ContainsKey(rightSocket.Socket))
                socketMap.rightSocketTiles.Add(rightSocket.Socket, new HashSet<(string, int)>());
            socketMap.rightSocketTiles[rightSocket.Socket].Add((rightSocket.Prefab, rightSocket.Rotation));
            socketMap.allTiles.Add((rightSocket.Prefab, rightSocket.Rotation));
        }
        foreach (var upSocket in socketMapFlat.UpSocketTiles)
        {
            if (!socketMap.topSocketTiles.ContainsKey(upSocket.Socket))
                socketMap.topSocketTiles.Add(upSocket.Socket, new HashSet<(string, int)>());
            socketMap.topSocketTiles[upSocket.Socket].Add((upSocket.Prefab, upSocket.Rotation));
            socketMap.allTiles.Add((upSocket.Prefab, upSocket.Rotation));
        }
        foreach (var downSocket in socketMapFlat.DownSocketTiles)
        {
            if (!socketMap.bottomSocketTiles.ContainsKey(downSocket.Socket))
                socketMap.bottomSocketTiles.Add(downSocket.Socket, new HashSet<(string, int)>());
            socketMap.bottomSocketTiles[downSocket.Socket].Add((downSocket.Prefab, downSocket.Rotation));
            socketMap.allTiles.Add((downSocket.Prefab, downSocket.Rotation));
        }


        // Flood fill from one of the static pieces (or 0,0 if no set pieces)
        var floodfillStartX = 0;
        var floodfillStartY = 0;
        if (setPieces.Length > 0)
        {
            var picked = Random.Range(0, setPieces.Length);
            var setPiece = setPieces[picked];
            
            var xPos = ((int)(setPiece.transform.position.x + 0.5)) / 25;
            var yPos = ((int)(setPiece.transform.position.y + 0.5)) / 25;

            floodfillStartX = xPos;
            floodfillStartY = yPos;
        }

        var processedOrPlanned = new HashSet<(int, int)>();
        var queue = new Queue<(int, int)>();
        queue.Enqueue((floodfillStartX, floodfillStartY));
        processedOrPlanned.Add((floodfillStartX, floodfillStartY));
        
        while (queue.TryPeek(out _))
        {
            //yield return null;
            var (nextX, nextY) = queue.Dequeue();

            var next = (nextX, nextY);
            var left = (nextX - 1, nextY);
            var right = (nextX + 1, nextY);
            var top = (nextX, nextY + 1);
            var bottom = (nextX, nextY - 1);
            
            if (!map.ContainsKey(next))
                continue;
            
            // Place one piece full logic
            if (map[next] == null)
            {
                var candidates = new HashSet<(string, int)>(socketMap.allTiles);
                // Reduce from left side
                if (map.TryGetValue(left, out var leftObject) && leftObject != null)
                    candidates.IntersectWith(
                        socketMap.leftSocketTiles[leftObject.GetComponent<TileDescriptor>().RightSocket]);
                // Reduce from right side
                if (map.TryGetValue(right, out var rightObject) && rightObject != null)
                    candidates.IntersectWith(
                        socketMap.rightSocketTiles[rightObject.GetComponent<TileDescriptor>().LeftSocket]);
                // Reduce from up side
                if (map.TryGetValue(top, out var upObject) && upObject != null)
                    candidates.IntersectWith(
                        socketMap.topSocketTiles[upObject.GetComponent<TileDescriptor>().DownSocket]);
                // Reduce from bottom side
                if (map.TryGetValue(bottom, out var downObject) && downObject != null)
                    candidates.IntersectWith(
                        socketMap.bottomSocketTiles[downObject.GetComponent<TileDescriptor>().UpSocket]);

                // pick random from candidates
                var listedCandidates = candidates.ToList();
                if (listedCandidates.Count > 0)
                {
                    var totalWeightValue = listedCandidates.Sum(x =>
                        PrefabReferenceMap[x.Item1].GetComponent<TileDescriptor>().Weight);
                    var targetWeightValue = Random.Range(0, totalWeightValue);
                    var chosenId = 0;
                    while (targetWeightValue > 0)
                    {
                        targetWeightValue -= PrefabReferenceMap[listedCandidates[chosenId].Item1].GetComponent<TileDescriptor>().Weight;
                        if (targetWeightValue > 0)
                            chosenId++;
                    }
                    
                    var obj = Instantiate(PrefabReferenceMap[listedCandidates[chosenId].Item1],
                        new Vector3(nextX * 25, 0, nextY * 25),
                        Quaternion.Euler(0, listedCandidates[chosenId].Item2 * 90, 0), WFCParent.transform);
                    map[next] = obj;
                    
                    // adjust socket based on rotation
                    var tileDescriptor = obj.GetComponent<TileDescriptor>();
                    RotateSocketDescription(tileDescriptor, listedCandidates[chosenId].Item2);
                }
            }

            // Enqueue neighbors
            if (!processedOrPlanned.Contains(left))
            {
                processedOrPlanned.Add(left);
                queue.Enqueue(left);
            }
            if (!processedOrPlanned.Contains(right))
            {
                processedOrPlanned.Add(right);
                queue.Enqueue(right);
            }
            
            if (!processedOrPlanned.Contains(top))
            {
                processedOrPlanned.Add(top);
                queue.Enqueue(top);
            }
            
            if (!processedOrPlanned.Contains(bottom))
            {
                processedOrPlanned.Add(bottom);
                queue.Enqueue(bottom);
            }
            
        }
    }
    
    private void RotateSocketDescription(TileDescriptor tileDescriptor, int item2)
    {
        if (item2 == 0)
            return;

        var top = tileDescriptor.UpSocket;
        tileDescriptor.UpSocket = tileDescriptor.LeftSocket;
        tileDescriptor.LeftSocket = tileDescriptor.DownSocket;
        tileDescriptor.DownSocket = tileDescriptor.RightSocket;
        tileDescriptor.RightSocket = top;
        
        RotateSocketDescription(tileDescriptor, item2 - 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
