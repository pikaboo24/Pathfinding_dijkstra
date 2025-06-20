using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TilemapGameLevel : MonoBehaviour
{
    //Tilemap representing the game world
    [SerializeField]private Tilemap map;

    //Tilebase
    [SerializeField]
    TileBase floorTile;

    // How large a level should we generate randomly (width and hight)
    public Vector2Int mapSizeTiles = new Vector2Int(10, 10);

    //Used in random generation to determine how likely floor tiles are
    public float chanceToSpawnFloor = 0.75f;

    public float perlinScale = 0.1f;
    private void Start()
    {
        map = GetComponent<Tilemap>();


        GenerateMap();

    }

    public void SetChanceToSpawnFloor(float chance)
    {

        chanceToSpawnFloor = chance;

    }


    public void GenerateMap()
    {
        map.ClearAllTiles();
        for (int x = 0; x < mapSizeTiles.x; x++)
        {
            for (int y = 0; y <= mapSizeTiles.y; y++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                if (Mathf.PerlinNoise(x * perlinScale, y * perlinScale) <= chanceToSpawnFloor)
                {
                    map.SetTile(tilePos, floorTile);
                }

            }

        }

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
            for(int y = bounds.yMin; y < bounds.yMax;y++)
            {
                if(!IsTraversible(x, y)) continue;

                Vector3Int tilePos = new Vector3Int(x, y, 0);
                Vector3 center = GetTileCenter (x, y);
                Gizmos.DrawSphere(center, 0.1f);

                foreach(Vector3Int neighbour in GetTraversibleNeighbours(tilePos))
                {
                    Vector3 neighbourCenter = GetTileCenter(neighbour.x,neighbour.y);
                    Gizmos.DrawLine(center, neighbourCenter);
                }
            }



        }


    }



}
