using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapGameLevel : MonoBehaviour
{
    //Tilemap representing the game world
    Tilemap map;

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
        for(int x =0; x < mapSizeTiles.x; x++)
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




}