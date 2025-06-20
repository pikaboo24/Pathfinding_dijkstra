using System.Collections;
using UnityEngine;

public class Move : MonoBehaviour
{
    public TilemapGameLevel level;
    public float moveSpeed = 5f;

    private Vector3Int currentTile;
    private Vector3 targetPosition;
    private bool isMoving = false;

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        BoundsInt bounds = level.GetBounds();
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                if (level.IsTraversible(x, y))
                {
                    currentTile = new Vector3Int(x, y, 0);
                    targetPosition = level.GetTileCenter(x, y);
                    transform.position = targetPosition;
                    yield break;
                }
            }
        }

        Debug.LogError("No traversible tile found for spawning!");
    }

    void Update()
    {
        if (!isMoving)
        {
            Vector3Int moveDir = Vector3Int.zero;

            if (Input.GetKeyDown(KeyCode.W)) moveDir = new Vector3Int(0, 1, 0);
            if (Input.GetKeyDown(KeyCode.S)) moveDir = new Vector3Int(0, -1, 0);
            if (Input.GetKeyDown(KeyCode.D)) moveDir = new Vector3Int(1, 0, 0);
            if (Input.GetKeyDown(KeyCode.A)) moveDir = new Vector3Int(-1, 0, 0);

            Vector3Int nextTile = currentTile + moveDir;

            if (moveDir != Vector3Int.zero && level.IsTraversible(nextTile.x, nextTile.y))
            {
                currentTile = nextTile;
                targetPosition = level.GetTileCenter(currentTile.x, currentTile.y);
                isMoving = true;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                isMoving = false;
            }
        }
    }
}
