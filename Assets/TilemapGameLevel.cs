using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using NUnit.Framework.Constraints;

public class TilemapGameLevel : MonoBehaviour
{
    [SerializeField] public  Tilemap map;
    public GameObject PlayerPrefab;
    public GameObject MonsterPrefab;
    [SerializeField] TileBase floorTile;
    public Vector2Int mapSizeTiles = new Vector2Int(10, 10);
    public float chanceToSpawnFloor = 0.75f;
    public float perlinScale = 0.1f;

    private void Start()
    {
        GenerateMap();
    }

    private void Update()
    {
        DrawTileGraph();
    }

    public void SetChanceToSpawnFloor(float chance)
    {
        chanceToSpawnFloor = chance;
    }

    public void GenerateMap()
    {
        Debug.Log("generate map called" + map);
        map.ClearAllTiles();

        GameObject thePlayer = GameObject.FindWithTag("Player");
        if (thePlayer != null)
        {
            Destroy(thePlayer);
        }

        GameObject theMonster = GameObject.FindWithTag("Monster");
        if (theMonster != null)
        {
            Destroy(theMonster);
        }

        float randomSeedX = Random.Range(20f, 100f);
        float randomSeedY = Random.Range(20f, 100f);

        for (int x = 0; x < mapSizeTiles.x; x++)
        {
            for (int y = 0; y <= mapSizeTiles.y; y++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                float noiseValue = Mathf.PerlinNoise(x * perlinScale + randomSeedX, y * perlinScale + randomSeedY);
                if (noiseValue <= chanceToSpawnFloor)
                {
                    map.SetTile(tilePos, floorTile);
                    Debug.Log($"Floor tile placed at: {tilePos}");
                }
            }
        }

        for (int x = 0; x < mapSizeTiles.x; x++)
        {
            for (int y = 0; y <= mapSizeTiles.y; y++)
            {
                if (IsTraversible(x, y))
                {
                    Vector3 spawnPos = GetTileCenter(x, y);

                    GameObject newPlayer = Instantiate(PlayerPrefab, spawnPos, Quaternion.identity);
                    Move moveScript = newPlayer.GetComponent<Move>();
                    if (moveScript != null)
                    {
                        moveScript.level = GetComponent<TilemapGameLevel>();
                    }
                    else
                    {
                        Debug.LogError("Move.cs not found");
                    }

                    Vector3 randomPos = GetTileCenter(Random.Range(0, mapSizeTiles.x), Random.Range(0, mapSizeTiles.y));
                    while (!IsTraversible((int)randomPos.x, (int)randomPos.y))
                    {
                        randomPos = GetTileCenter(Random.Range(0, mapSizeTiles.x), Random.Range(0, mapSizeTiles.y));
                    }

                    GameObject newMonster = Instantiate(MonsterPrefab, randomPos, Quaternion.identity);
                    MonsterAgent monsterScript = newMonster.GetComponent<MonsterAgent>();
                    if (monsterScript != null)
                    {
                        monsterScript.level = GetComponent<TilemapGameLevel>();
                    }
                    else
                    {
                        Debug.LogError("MonsterAgent.cs not found on prefab!");
                    }

                    return;
                }
            }
        }

        Debug.LogError("No tile found for player spawn!");
    }

    public bool IsTraversible(int x, int y)
    {
        TileBase tile = GetTile(x, y);
        return tile != null;
    }

    public TileBase GetTile(int x, int y)
    {
        return map.GetTile(new Vector3Int(x, y, 0));
    }

    public Vector3 GetTileCenter(int x, int y)
    {
        return map.GetCellCenterWorld(new Vector3Int(x, y, 0));
    }

    public BoundsInt GetBounds()
    {
        return map.cellBounds;
    }

    public float GetCostToEnterTile(int x, int y)
    {
        return 1;
    }

    public List<Vector3Int> GetTraversibleNeighbours(Vector3Int tilePos)
    {
        List<Vector3Int> neighbours = new List<Vector3Int>();
        Vector3Int[] directions = new Vector3Int[]
        {
           new Vector3Int(0, 1, 0),
           new Vector3Int(0, -1, 0),
           new Vector3Int(1, 0, 0),
           new Vector3Int(-1, 0, 0)
        };

        foreach (Vector3Int dir in directions)
        {
            Vector3Int neighbourPos = tilePos + dir;
            if (IsTraversible(neighbourPos.x, neighbourPos.y))
            {
                neighbours.Add(neighbourPos);
            }
        }
        return neighbours;
    }

    private void DrawTileGraph()
    {
        if (map == null) return;

        BoundsInt bounds = GetBounds();
        Gizmos.color = Color.green;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                if (!IsTraversible(x, y)) continue;

                Vector3Int tilePos = new Vector3Int(x, y, 0);
                Vector3 center = GetTileCenter(x, y);
                Debug.DrawRay(center + Vector3.up * 0.1f, Vector3.down * 0.2f, Color.green);
                Debug.DrawRay(center + Vector3.left * 0.1f, Vector3.right * 0.2f, Color.green);

                foreach (Vector3Int neighbour in GetTraversibleNeighbours(tilePos))
                {
                    Vector3 neighbourCenter = GetTileCenter(neighbour.x, neighbour.y);
                    Debug.DrawLine(center, neighbourCenter, Color.red);
                }
            }
        }
    }
}
