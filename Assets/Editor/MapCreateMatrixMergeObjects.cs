using JsonNet = Newtonsoft.Json.JsonConvert;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapObjectExporterEditor : EditorWindow
{
    [System.Serializable]
    public class ObjectConfig
    {
        public Dictionary<long, string> idToName;
    }

    [System.Serializable]
    public class ObjectEntry
    {
        public GameObject gameObject;
        public long objectId;
    }

    private Tilemap groundTilemap;
    private TextAsset jsonConfig;

    private Dictionary<long, string> idToName = new Dictionary<long, string>();
    private List<ObjectEntry> objectEntries = new List<ObjectEntry>();

    private long groundId = 0;

    [MenuItem("Tools/Map Matrix Exporter(long)")]
    public static void Open()
    {
        GetWindow<TilemapObjectExporterEditor>("Tilemap Exporter");
    }

    private void OnGUI()
    {
        GUILayout.Label("INPUT", EditorStyles.boldLabel);

        groundTilemap =
            (Tilemap)EditorGUILayout.ObjectField(
                "Ground Tilemap",
                groundTilemap,
                typeof(Tilemap),
                true
            );

        jsonConfig =
            (TextAsset)EditorGUILayout.ObjectField(
                "Object Config (JSON)",
                jsonConfig,
                typeof(TextAsset),
                false
            );

        groundId = EditorGUILayout.LongField("Ground Default ID", groundId);

        if (GUILayout.Button("Load JSON Config"))
        {
            LoadJson();
        }

        GUILayout.Space(10);
        GUILayout.Label("OBJECTS", EditorStyles.boldLabel);

        DrawObjectList();

        if (GUILayout.Button("Add Object"))
        {
            objectEntries.Add(new ObjectEntry());
        }

        GUILayout.Space(20);

        if (GUILayout.Button("EXPORT BIN"))
        {
            ExportBin();
        }
    }

    private void LoadJson()
    {
        idToName.Clear();

        if (jsonConfig == null)
        {
            Debug.LogError("JSON config chưa được gán");
            return;
        }

        try
        {
            Dictionary<string, string> rawDict =
                JsonNet.DeserializeObject<Dictionary<string, string>>(jsonConfig.text);

            foreach (var kv in rawDict)
            {
                long id = long.Parse(kv.Key);
                idToName[id] = kv.Value;
            }

            Debug.Log("Load JSON thành công");
        }
        catch
        {
            Debug.LogError("JSON không hợp lệ");
        }
    }

    private void DrawObjectList()
    {
        for (int i = 0; i < objectEntries.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            objectEntries[i].gameObject =
                (GameObject)EditorGUILayout.ObjectField(
                    objectEntries[i].gameObject,
                    typeof(GameObject),
                    true
                );

            if (idToName.Count > 0)
            {
                List<long> ids = new List<long>(idToName.Keys);
                List<string> labels = new List<string>();

                foreach (long id in ids)
                {
                    labels.Add(id + " - " + idToName[id]);
                }

                int currentIndex = Mathf.Max(0, ids.IndexOf(objectEntries[i].objectId));
                int newIndex = EditorGUILayout.Popup(currentIndex, labels.ToArray());
                objectEntries[i].objectId = ids[newIndex];
            }
            else
            {
                objectEntries[i].objectId =
                    EditorGUILayout.LongField(objectEntries[i].objectId);
            }

            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                objectEntries.RemoveAt(i);
                break;
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    private void ExportBin()
    {
        if (groundTilemap == null)
        {
            Debug.LogError("Chưa gán Tilemap nền");
            return;
        }

        groundTilemap.CompressBounds();
        BoundsInt bounds = groundTilemap.cellBounds;

        int width = bounds.size.x;
        int height = bounds.size.y;

        long[,] grid = new long[height, width];

        // Fill ground
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                grid[y, x] = groundId;
            }
        }

        // Place objects
        foreach (var entry in objectEntries)
        {
            if (entry.gameObject == null) continue;

            Vector3 worldPos = entry.gameObject.transform.position;
            Vector3Int cellPos = groundTilemap.WorldToCell(worldPos);

            int x = cellPos.x - bounds.xMin;
            int y = cellPos.y - bounds.yMin;

            if (x < 0 || x >= width || y < 0 || y >= height)
            {
                Debug.LogWarning(
                    $"Object {entry.gameObject.name} nằm ngoài Tilemap"
                );
                continue;
            }

            grid[y, x] = entry.objectId;
        }

        string dirPath = Path.Combine(
            Application.persistentDataPath,
            "Maps"
        );

        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        string filePath = Path.Combine(dirPath, "map.bin");

        using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
        {
            writer.Write(width);
            writer.Write(height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    writer.Write(grid[y, x]); // Int64
                }
            }
        }

        Debug.Log("Export BIN thành công");
        Debug.Log("Path: " + filePath);
    }
}
