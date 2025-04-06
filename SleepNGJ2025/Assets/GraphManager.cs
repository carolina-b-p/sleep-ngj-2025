using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Random = System.Random;

public class GraphManager : MonoBehaviour
{
    private Dictionary<(int, int), HashSet<(int, int)>> graph = new();
    private (int, int) lastGeneratedTarget = (-1, -1);
    private Dictionary<(int, int), (int, int)> path = new();
    private bool graphBuilt = false;
    
    //singleton instance
    public static GraphManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [ContextMenu("Build Graph")]
    public void BuildGraph()
    {
        graph = new();
        var source = GameObject.FindGameObjectWithTag("WFCParent");
        for (int i = 0; i < source.transform.childCount; i++)
        {
            var child = source.transform.GetChild(i);
            var x = (int)(child.transform.position.x + 0.5) / 25;
            var y = (int)(child.transform.position.z + 0.5) / 25;
            graph.Add((x, y), new HashSet<(int, int)>());

            var tileDescriptor = child.GetComponent<TileDescriptor>();
            if (tileDescriptor.LeftSocket.Contains("R"))
                graph[(x, y)].Add((x - 1, y));
            if (tileDescriptor.RightSocket.Contains("R"))
                graph[(x, y)].Add((x + 1, y));
            if (tileDescriptor.UpSocket.Contains("R"))
                graph[(x, y)].Add((x, y + 1));
            if (tileDescriptor.DownSocket.Contains("R"))
                graph[(x, y)].Add((x, y - 1));
        }
        
        graphBuilt = true;
    }

    private void OnDrawGizmosSelected()
    {
        foreach (var key in graph.Keys)
        {
            var prevColor = Gizmos.color;
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(new Vector3(key.Item1 * 25, 10, key.Item2 * 25), 3);
            foreach (var value in graph[key])
            {
                Gizmos.DrawLine(new Vector3(key.Item1 * 25, 10, key.Item2 * 25),
                    new Vector3(value.Item1 * 25, 10, value.Item2 * 25));
            }

            Gizmos.color = prevColor;
        }

    }

    // Update is called once per frame
    public (bool, int, Vector3) UpdatePathAndGetIntermediateTarget(Vector3 position)
    {
        if (!graphBuilt)
            BuildGraph();
        
        var playerObject = GameObject.FindGameObjectWithTag("Player");
        var playerx = Mathf.RoundToInt(playerObject.transform.position.x / 25);
        var playery = Mathf.RoundToInt(playerObject.transform.position.z / 25);

        if (graph[(playerx, playery)].Count == 0)
        {
            do
            {
                playerx = UnityEngine.Random.Range(0, 10);
                playery = UnityEngine.Random.Range(0, 10);
            } while (graph[(playerx, playery)].Count == 0);
            playerObject.transform.position = new Vector3(playerx * 25, 1, playery * 25);
        }
        
        var targetx = Mathf.RoundToInt(position.x / 25);
        var targety = Mathf.RoundToInt(position.z / 25);
        if (targetx != lastGeneratedTarget.Item1 || targety != lastGeneratedTarget.Item2 ||
            !path.ContainsKey((playerx, playery)))
        {
            if (!BFS((playerx, playery), (targetx, targety)))
                return (false, -1, Vector3.zero);
            lastGeneratedTarget = (targetx, targety);
        }
        
        var nextGoal = path[(playerx, playery)];
        // try to lookahead
        if (path.ContainsKey(nextGoal))
            nextGoal = path[nextGoal];
        if (path.ContainsKey(nextGoal))
            nextGoal = path[nextGoal];

        return (true, path.Count, new Vector3(nextGoal.Item1 * 25, 5, nextGoal.Item2 * 25));
    }

    private bool BFS((int X, int Y) player, (int X, int Y) target)
    {
        path = new();
        var q = new Queue<((int, int), (int, int))>();
        var vnp = new HashSet<(int, int)>();
        var visited = new Dictionary<(int, int), (int, int)>();
        q.Enqueue(((player.X, player.Y), (player.X, player.Y)));
        vnp.Add(player);
        
        // explore
        while (q.TryPeek(out _))
        {
            var next = q.Dequeue();
            visited.Add(next.Item1, next.Item2);
            
            if (next.Item1 == target)
                break;

            var key = next.Item1;
            if (!graph.ContainsKey(key))
                continue;
            
            var neighbors = graph[key];
            foreach (var neighbor in neighbors)
            {
                if (vnp.Contains(neighbor))
                    continue;
                vnp.Add(neighbor);
                q.Enqueue((neighbor, key));
            }
        }
        
        // rebuild
        if (visited.ContainsKey(target))
        {
            path.Add(target, target);
            while (target != player)
            {
                path.Add(visited[target], target);
                target = visited[target];
            }

            return true;
        }
        return false;
    }
    
}
