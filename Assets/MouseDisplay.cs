using UnityEngine;
using UnityEngine.Tilemaps;

public class MouseDisplay : MonoBehaviour
{
    private Tilemap map;
    public TMPro.TextMeshProUGUI TextMeshGraphic;

    void Start()
    {
        map = FindAnyObjectByType<Tilemap>();
    }

    void Update()
    {
        Vector2 pixelPos = Input.mousePosition;
        Vector3 worldSpacePositionOfMouse = Camera.main.ScreenToWorldPoint(pixelPos);
        worldSpacePositionOfMouse.z = 0;

        int tilePosX = (int)worldSpacePositionOfMouse.x;
        int tilePosY = (int)worldSpacePositionOfMouse.y;
        Vector3Int tilePos = new Vector3Int(tilePosX, tilePosY, 0);
        transform.position = map.GetCellCenterWorld(tilePos);

        TileBase tile = map.GetTile(tilePos);
        if (tile != null)
        {
            TextMeshGraphic.text = tile.name;
        }
        else
        {
            TextMeshGraphic.text = "empty";
        }

        TextMeshGraphic.text += "\n" + tilePosX + "," + tilePosY;

        if (Input.GetMouseButtonDown(1))
        {
            FindAnyObjectByType<Pathfinder>().start = new Vector2Int(tilePosX, tilePosY);
            Debug.Log($"Start set to: {tilePosX},{tilePosY}");
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (tile != null)
            {
                FindAnyObjectByType<Pathfinder>().end = new Vector2Int(tilePosX, tilePosY);
                FindAnyObjectByType<Pathfinder>().FindPathDebugging();
                Debug.Log($"Goal set to: {tilePosX},{tilePosY}");
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            MonsterAgent monster = FindFirstObjectByType<MonsterAgent>();
            Pathfinder pf = FindFirstObjectByType<Pathfinder>();

            if (pf.solution != null && pf.solution.Count > 0)
            {
                monster.StartFollowingPath(pf.solution);
                Debug.Log("Monster started following the path.");
            }
            else
            {
                Debug.LogWarning("No solution path for Monster to follow!");
            }
        }
    }
}
