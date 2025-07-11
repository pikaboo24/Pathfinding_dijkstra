using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAgent : MonoBehaviour
{
    public TilemapGameLevel level;
    public float moveSpeed = 2f;

    public void StartFollowingPath(List<Vector2Int> path)
    {
        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("No path to follow!");
            return;
        }

        StopAllCoroutines();
        StartCoroutine(FollowPath(path));
    }

    private IEnumerator FollowPath(List<Vector2Int> path)
    {
        foreach (Vector2Int step in path)
        {
            Vector3 targetPos = level.GetTileCenter(step.x, step.y);
            while (Vector3.Distance(transform.position, targetPos) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                yield return null;
            }
            transform.position = targetPos; // Snap to center
        }
        Debug.Log("Monster reached goal!");
    }
}
