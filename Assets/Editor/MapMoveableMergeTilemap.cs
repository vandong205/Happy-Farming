using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapBinaryExporter : EditorWindow
{
    private Tilemap groundTilemap;
    private Tilemap objectTilemap;
    private string fileName = "map_walkable.bin";

    private const byte WALKABLE = 1;
    private const byte BLOCKED = 0;

    [MenuItem("Tools/Map/MapMoveableMergeTilemap (Binary)")]
    public static void Open()
    {
        GetWindow<TilemapBinaryExporter>("Map Binary Exporter");
    }

    private void OnGUI()
    {
        GUILayout.Label("Tilemap → Walkable Binary Map", EditorStyles.boldLabel);

        groundTilemap = (Tilemap)EditorGUILayout.ObjectField(
            "Ground Tilemap", groundTilemap, typeof(Tilemap), true);

        objectTilemap = (Tilemap)EditorGUILayout.ObjectField(
            "Object Tilemap (Block)", objectTilemap, typeof(Tilemap), true);

        fileName = EditorGUILayout.TextField("File Name", fileName);

        GUILayout.Space(10);

        if (GUILayout.Button("Export"))
        {
            Export();
        }

        EditorGUILayout.HelpBox(
            $"Save to:\n{Application.persistentDataPath}",
            MessageType.Info);
    }

    private void Export()
    {
        if (groundTilemap == null)
        {
            Debug.LogError("❌ Ground Tilemap is NULL");
            return;
        }

        // ✅ CHỈ LẤY VÙNG CÓ TILE THỰC SỰ
        groundTilemap.CompressBounds();
        BoundsInt bounds = groundTilemap.cellBounds;
       

        int width = bounds.size.x;
        int height = bounds.size.y;
        int offsetX = bounds.xMin;
        int offsetY = bounds.yMin;

        byte[] tiles = new byte[width * height];

        int walkableCount = 0;
        int blockedCount = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3Int cellPos = new Vector3Int(
                    offsetX + x,
                    offsetY + y,
                    0
                );

                int index = y * width + x;

                // Mặc định: không có ground → không tính
                tiles[index] = BLOCKED;

                if (!groundTilemap.HasTile(cellPos))
                    continue;

                // Có ground → walkable
                byte value = WALKABLE;

                // Có object → blocked
                if (objectTilemap != null && objectTilemap.HasTile(cellPos))
                    value = BLOCKED;

                tiles[index] = value;

                if (value == WALKABLE) walkableCount++;
                else blockedCount++;
            }
        }

        // 📁 Lưu vào PersistentDataPath
        string dir = Application.persistentDataPath;
        Directory.CreateDirectory(dir);
        string path = Path.Combine(dir, fileName);

        using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
        {
            // HEADER
            writer.Write(width);
            writer.Write(height);
            writer.Write(offsetX);
            writer.Write(offsetY);

            // DATA
            writer.Write(tiles);
        }

        Debug.Log("✅ MAP EXPORT DONE");
        Debug.Log($"Path       : {path}");
        Debug.Log($"Map Size   : {width} x {height}");
        Debug.Log($"Offset     : ({offsetX}, {offsetY})");
        Debug.Log($"Walkable   : {walkableCount}");
        Debug.Log($"Blocked    : {blockedCount}");
    }
}
