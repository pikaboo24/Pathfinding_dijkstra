using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAgent : MonoBehaviour
{
    public TilemapGameLevel level;
    public float moveSpeed = 2f;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(FollowPlayer());
    }

    private IEnumerator FollowPlayer()
    {
        while (true)
        {
            Vector3Int monsterTile3D = level.map.WorldToCell(transform.position);
            Vector2Int monsterTile = new Vector2Int(monsterTile3D.x, monsterTile3D.y);

            Vector3Int playerTile3D = level.map.WorldToCell(player.position);
            Vector2Int playerTile = new Vector2Int(playerTile3D.x, playerTile3D.y);

            Pathfinder pf = FindFirstObjectByType<Pathfinder>();
            pf.start = monsterTile;
            pf.end = playerTile;
            yield return StartCoroutine(pf.DijkstraSearchCoroutine(monsterTile, playerTile));

            if (pf.solution != null && pf.solution.Count > 0)
            {
                Debug.Log("Monster found path with " + pf.solution.Count + " steps.");
                StartFollowingPath(pf.solution);
            }
            else
            {
                Debug.LogWarning("Monster: No path found.");
            }

            yield return new WaitForSeconds(2f);
        }
    }

    public void StartFollowingPath(List<Vector2Int> path)
    {
        StartCoroutine(FollowPath(path));
    }

    private IEnumerator FollowPath(List<Vector2Int> path)
    {
        foreach (Vector2Int step in path)
        {
            Vector3 targetPos = level.GetTileCenter(step.x, step.y);
            Debug.Log("Monster moving to: " + targetPos);
            while (Vector3.Distance(transform.position, targetPos) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                yield return null;
            }
            transform.position = targetPos;
        }
    }
}
