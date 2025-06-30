using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;



public class Pathfinder : MonoBehaviour
{
    struct DijkstraNodeData
    {
        public Vector2Int previous;
        public float gCost;

        public DijkstraNodeData(Vector2Int previous, float gCost)
        {
            this.previous = previous;
            this.gCost = gCost;

        }
    }

    private TilemapGameLevel level;

    public Vector2Int start;
    public Vector2Int end;
    public Vector2Int current;
    public List<Vector2Int> solution;
    private float iteratuinDelay = 0.5f;

    private List<Vector2Int> unvisited;
    private List<Vector2Int> visited;
    private Dictionary<Vector2Int, DijkstraNodeData> nodes;

    private void Awake()
    {
        level = GetComponent<TilemapGameLevel>();
    }

    internal void FindPathDebugging()
    {
        StopAllCoroutines();
        StartCoroutine(DijkstraSearchCoroutine(start, end));


    }
    public IEnumerator DijkstraSearchCoroutine(Vector2Int origin, Vector2Int destination)
    {
        start = origin;
        end = destination;
        solution = new List<Vector2Int>();
        nodes = new Dictionary<Vector2Int, DijkstraNodeData>();
        unvisited = new List<Vector2Int> {start };
        visited = new List<Vector2Int>();

        UpdateBestWayToReachTile(start, start, 0);

        while(!IsComplete())
        {
            DijkstraIteration();
            yield return new WaitForSeconds(iteratuinDelay);
        }

        if (IsSolved())
        {
            Debug.Log("Dijkstra's Algo Success");
            GenerateSolution();

        }
        else
        {
            Debug.Log("Dijkstra's Algo Failed");
        }
    }
    void GenerateSolution()
    {
        solution = new List<Vector2Int>();
        if (!IsSolved() || !nodes.ContainsKey(end))

        {
            Debug.LogWarning("No Valid Path Found. No solution");
            return;

        }
        Vector2Int currentNode = end;

        while (currentNode != start)
        { 
            solution.Add(currentNode);
            if (!nodes.ContainsKey(currentNode))
            {
                Debug.LogWarning("SomeThing went wrong While finding path ");
                return ;
            }
            currentNode = nodes[currentNode].previous;
        }
        solution.Add(start);
        solution.Reverse();
    
    
    }
    public bool IsDiscovered(Vector2Int node)
    { 
    
        return unvisited.Contains(node) || visited.Contains(node);
    
    }
    void MoveToVisitedSet(Vector2Int node)
    { 
        unvisited.Remove(node);
        visited.Add(node);
    }

    public bool IsVisited(Vector2Int node)
    {
        return visited.Contains(node);
    }
    public bool IsSolved()
    { 
        return IsVisited(end);    
    }
    public bool IsComplete()
    {
        return IsSolved() || GetLowestCostInUnvisited().Item2 == float.PositiveInfinity;
    }
    public float GetTotalCostToReach (Vector2Int node)
    {
        if(nodes.ContainsKey(node))
        {
            return nodes[node].gCost;

        }
        else
        {
            return float.PositiveInfinity;
        }

    }

    public void UpdateBestWayToReachTile(Vector2Int origin, Vector2Int destination, float cost)
    {
        nodes[destination] = new DijkstraNodeData(origin, cost);

    }
    public Tuple<Vector2Int, float> GetLowestCostInUnvisited()
    {
        if (unvisited == null || unvisited.Count == 0)        
        {
            return new Tuple<Vector2Int, float> (new Vector2Int(-1, -1),float .PositiveInfinity);

        }
        Vector2Int bestNode = unvisited[0];
        float bestCost = GetTotalCostToReach(bestNode);

        foreach(Vector2Int node in unvisited)
        {
            float cost = GetTotalCostToReach(node);
            if (cost < bestCost)
            {
                bestCost = cost;
                bestNode = node;    

            }

        }
        return new Tuple<Vector2Int, float> (bestNode, bestCost);


    }

    public void DijkstraIteration()
    {
        current = GetLowestCostInUnvisited().Item1;

        Debug.Log("Visiting: " + current + ", cost" + nodes[current].gCost);
        DebugDrawing.DrawCircle(level.GetTileCenter(current.x, current.y), Quaternion.AngleAxis(90, Vector3.forward), 0.6f, 16, Color.yellow, iteratuinDelay, false);

        foreach (Vector3Int neighbour in level.GetTraversibleNeighbours(new Vector3Int(current.x, current.y, 0))) 
        {
            Vector2Int connected = new Vector2Int(neighbour.x, neighbour.y);
            float costToConnected = nodes[current].gCost + level.GetCostToEnterTile(connected.x, connected.y);

            if (!IsDiscovered(connected))
            {
                Debug.Log("Discovered new node:" + connected);
                unvisited.Add(connected);
                UpdateBestWayToReachTile(current, connected, costToConnected);


            }
            else if (costToConnected < GetTotalCostToReach(connected))
            {
                Debug.Log("Better path to " + connected);
                UpdateBestWayToReachTile(current, connected, costToConnected);

            
            }
            MoveToVisitedSet(current);

        }

      

    }

    void OnDrawGizmos()
    {
        if (level == null || nodes == null) return;

        GUIStyle style = new GUIStyle();
        style.fontSize = 24;
        style.fontStyle = FontStyle.Bold;

        Vector3 startWorld = level.GetTileCenter(start.x, start.y);
        Vector3 endWorld = level.GetTileCenter(end.x, end.y);

        style.normal.textColor = Color.green;
        Handles.Label(startWorld + Vector3.up * 0.4f, "START", style);
        DebugDrawing.DrawCircle(startWorld, Quaternion.AngleAxis(90, Vector3.forward), 0.8f, 8, Color.green, Time.deltaTime, false);

        style.normal.textColor = Color.red;
        Handles.Label(endWorld + Vector3.up * 0.4f, "END", style);
        DebugDrawing.DrawCircle(endWorld, Quaternion.AngleAxis(90, Vector3.forward), 0.8f, 8, Color.red, Time.deltaTime, false);

        Gizmos.color = Color.cyan;
        foreach (var pair in nodes)
        {
            Vector2Int node = pair.Key;
            Vector2Int from = pair.Value.previous;
            float cost = pair.Value.gCost;

            Vector3 nodePos = level.GetTileCenter(node.x, node.y);
            Vector3 fromPos = level.GetTileCenter(from.x, from.y);

            Vector3 dir = nodePos - fromPos;
            float length = dir.magnitude;
            Vector3 mid = (fromPos + nodePos) / 2f;
            Vector3 thickness = Vector3.Cross(dir.normalized, Vector3.forward).normalized * 0.08f;


            Gizmos.DrawCube(mid, new Vector3(length, thickness.magnitude * 2f, 0.01f));
            Handles.Label(nodePos + Vector3.up * 0.2f, cost.ToString("F0"), style);
        }

        if (solution != null)
        {
            Gizmos.color = Color.magenta;
            for (int i = 0; i < solution.Count - 1; i++)
            {
                Vector3 from = level.GetTileCenter(solution[i].x, solution[i].y);
                Vector3 to = level.GetTileCenter(solution[i + 1].x, solution[i + 1].y);
                
                Vector3 dir = to - from;
                float length = dir.magnitude;
                Vector3 mid = (from + to) / 2f;
                Vector3 thickness = Vector3.Cross(dir.normalized, Vector3.forward).normalized * 0.1f;

                Gizmos.DrawCube(mid, new Vector3(length, thickness.magnitude * 2f, 0.01f));
            }
        }
    }


}