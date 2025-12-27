using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

public class TileSelectEffect : MonoBehaviour
{
    public Tilemap tilemap;               // Tilemap cần highlight
    public GameObject highlightPrefab;    // Sprite highlight
    private GameObject highlightInstance;

    private Vector2 mousePos;

    private void Start()
    {
        if (highlightPrefab != null)
        {
            highlightInstance = Instantiate(highlightPrefab);
            highlightInstance.SetActive(false); // ẩn ban đầu
        }
    }

    // Hàm InputSystem gọi khi chuột di chuyển
    private void Update()
    {
        if (highlightInstance == null) return;
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue(); // <-- InputSystem
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            worldPos.z = 0f; // quan trọng: z = 0 để đúng tilemap plane
            Vector3Int cellPos = WorldManager.Instance.WorldPosToCellPos(worldPos);

            if (tilemap.HasTile(cellPos))
            {
                highlightInstance.SetActive(true);
                highlightInstance.transform.position = tilemap.GetCellCenterWorld(cellPos);
                Vector2 tilemappos = new Vector2(cellPos.x,cellPos.y);
                Debug.Log("Dang click vao tile: "+WorldManager.Instance.GetTileID(tilemappos));
            }
            else
            {
                highlightInstance.SetActive(false);
            }
        }
    }

}
