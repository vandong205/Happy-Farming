using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class WalkableFootMarker : EditorWindow
{
    private Tilemap baseTilemap;
    private GameObject parentObjects;
    private string fileName = "map_walkable.bin";

    private const byte WALKABLE = 1;
    private const byte BLOCKED = 0;

    [MenuItem("Tools/Map/Mark Foot Objects As Blocked")]
    public static void Open()
    {
        GetWindow<WalkableFootMarker>("Walkable Foot Marker");
    }

    private void OnGUI()
    {
        GUILayout.Label("Mark GameObject Feet as BLOCKED on Walkable Map", EditorStyles.boldLabel);

        baseTilemap = (Tilemap)EditorGUILayout.ObjectField("Base Tilemap", baseTilemap, typeof(Tilemap), true);
        parentObjects = (GameObject)EditorGUILayout.ObjectField("Parent Objects", parentObjects, typeof(GameObject), true);
        fileName = EditorGUILayout.TextField("File Name", fileName);

        GUILayout.Space(10);

        if (GUILayout.Button("Mark Foot Objects"))
        {
            MarkFootObjects();
        }

        EditorGUILayout.HelpBox(
            $"File sẽ lưu vào:\n{Application.persistentDataPath}",
            MessageType.Info);
    }

    private void MarkFootObjects()
    {
        if (baseTilemap == null || parentObjects == null)
        {
            Debug.LogError("❌ Base Tilemap hoặc ParentObjects chưa được gán!");
            return;
        }

        // ✅ Pack tilemap để lấy bounds chính xác
        baseTilemap.CompressBounds();
        BoundsInt bounds = baseTilemap.cellBounds;
        int width = bounds.size.x;
        int height = bounds.size.y;
        int offsetX = bounds.xMin;
        int offsetY = bounds.yMin;

        string path = Path.Combine(Application.persistentDataPath, fileName);
        if (!File.Exists(path))
        {
            Debug.LogError($"❌ File không tồn tại: {path}");
            return;
        }

        byte[] tiles;
        using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
        {
            int fileWidth = reader.ReadInt32();
            int fileHeight = reader.ReadInt32();
            int fileOffsetX = reader.ReadInt32();
            int fileOffsetY = reader.ReadInt32();
            tiles = reader.ReadBytes(fileWidth * fileHeight);
        }

        int markedCount = 0;

        // Lấy tất cả child có tag "ObjectFoot"
        Transform[] footObjects = parentObjects.GetComponentsInChildren<Transform>();
        foreach (Transform child in footObjects)
        {
            if (child == parentObjects.transform) continue; // bỏ qua chính parent
            if (child.tag != "ObjectFoot") continue;        // chỉ foot

            Vector3 worldPos = child.position;
            Vector3Int cellPos = baseTilemap.WorldToCell(worldPos);

            int x = cellPos.x - offsetX;
            int y = cellPos.y - offsetY;

            if (x < 0 || y < 0 || x >= width || y >= height) continue;

            int index = y * width + x;
            if (tiles[index] != BLOCKED)
            {
                tiles[index] = BLOCKED;
                markedCount++;
            }
        }

        // Ghi lại file
        using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
        {
            writer.Write(width);
            writer.Write(height);
            writer.Write(offsetX);
            writer.Write(offsetY);
            writer.Write(tiles);
        }

        Debug.Log($"✅ Đánh dấu xong {markedCount} ô là BLOCKED.");
        Debug.Log($"Map Size   : {width} x {height}");
        Debug.Log($"Offset     : ({offsetX}, {offsetY})");
        Debug.Log($"File lưu tại: {path}");
    }
}
