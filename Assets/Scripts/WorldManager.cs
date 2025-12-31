using System.IO;
using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class WorldManager : SingletonPattern<WorldManager>
{
    Dictionary<Vector2, long> mapMatrix = new();
    Dictionary<Vector2, int> mapBaseMatrix = new();
    Dictionary<Vector2, int> mapGroundMatrix = new();
    [SerializeField] Tilemap worldMap;
    private int width;
    private int height;
    private int offsetX;
    private int offsetY;
    private byte[] tiles;
    /// <summary>
    /// Global Object ID
    /// </summary>
    private long _nextID = 0;
    private const byte WALKABLE = 1;

    /// <summary>
    /// Đọc dữ liệu map từ file .bin
    /// </summary>
    /// <param name="fullPath">Đường dẫn đầy đủ tới file</param>
    public IEnumerator LoadMapWalkableData(string fullPath)
    {
        if (!File.Exists(fullPath))
        {
            Debug.LogError($"Map file not found: {fullPath}");
            yield break;
        }

        using (BinaryReader reader = new BinaryReader(File.OpenRead(fullPath)))
        {
            width = reader.ReadInt32();
            height = reader.ReadInt32();
            offsetX = reader.ReadInt32();
            offsetY = reader.ReadInt32();

            tiles = reader.ReadBytes(width * height);
        }

        Debug.Log("=== MAP LOADED ===");
        Debug.Log($"Path      : {fullPath}");
        Debug.Log($"Size      : {width} x {height}");
        Debug.Log($"Offset    : ({offsetX}, {offsetY})");
    }
    public long GetTileID(Vector2 tilemappos)
    {
        if (mapMatrix.ContainsKey(tilemappos))
        {
            return mapMatrix[tilemappos];
        }
        else return -1;
    }
    public int GetTileBaseId(Vector2 tilemappos)
    {
        if (mapBaseMatrix.ContainsKey(tilemappos))
        {
            return mapBaseMatrix[tilemappos];
        }
        else return -1;
    }
    public int GetTileGroundId(Vector2 tilemappos)
    {
        if (mapGroundMatrix.ContainsKey(tilemappos))
        {
            return mapGroundMatrix[tilemappos];
        }
        else return -1;
    }
    public int GetGroundTileId(Vector2 tilemappos)
    {
        if (mapGroundMatrix.ContainsKey(tilemappos))
        {
            return mapGroundMatrix[tilemappos];
        }
        else return -1;
    }
    public IEnumerator LoadWorldMatrix(string fullPath)
    {
        mapMatrix.Clear();

        if (!File.Exists(fullPath))
        {
            Debug.LogError($"World matrix file not found: {fullPath}");
            yield break;
        }

        using (BinaryReader reader = new BinaryReader(File.OpenRead(fullPath)))
        {
            width = reader.ReadInt32();
            height = reader.ReadInt32();

            // Dùng bounds tilemap hiện tại để align grid
            worldMap.CompressBounds();
            BoundsInt bounds = worldMap.cellBounds;

            int baseX = bounds.xMin;
            int baseY = bounds.yMin;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    long objectId = reader.ReadInt64();

                    // Bỏ qua ground mặc định nếu muốn
                    //if (objectId == 0)
                    //    continue;

                    int gridX = baseX + x;
                    int gridY = baseY + y;

                    mapMatrix[new Vector2(gridX, gridY)] = objectId;
                }
            }
        }

        Debug.Log("=== WORLD MATRIX LOADED ===");
        Debug.Log($"Path   : {fullPath}");
        Debug.Log($"Size   : {width} x {height}");
        Debug.Log($"Cells  : {mapMatrix.Count}");
    }
    public IEnumerator LoadWorldBaseMatrix(string fullPath)
    {
        mapBaseMatrix.Clear();

        if (!File.Exists(fullPath))
        {
            Debug.LogError($"World matrix file not found: {fullPath}");
            yield break;
        }

        using (BinaryReader reader = new BinaryReader(File.OpenRead(fullPath)))
        {
            width = reader.ReadInt32();
            height = reader.ReadInt32();

            // Dùng bounds tilemap hiện tại để align grid
            worldMap.CompressBounds();
            BoundsInt bounds = worldMap.cellBounds;

            int baseX = bounds.xMin;
            int baseY = bounds.yMin;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int objectId = reader.ReadInt32();

                    // Bỏ qua ground mặc định nếu muốn
                    //if (objectId == 0)
                    //    continue;

                    int gridX = baseX + x;
                    int gridY = baseY + y;

                    mapBaseMatrix[new Vector2(gridX, gridY)] = objectId;
                }
            }
        }

        Debug.Log("=== WORLD BASEMATRIX LOADED ===");
        Debug.Log($"Path   : {fullPath}");
        Debug.Log($"Size   : {width} x {height}");
        Debug.Log($"Cells  : {mapBaseMatrix.Count}");
    }
    public IEnumerator LoadWorldGroundMatrix(string fullPath)
    {
        mapGroundMatrix.Clear();

        if (!File.Exists(fullPath))
        {
            Debug.LogError($"World matrix file not found: {fullPath}");
            yield break;
        }

        using (BinaryReader reader = new BinaryReader(File.OpenRead(fullPath)))
        {
            width = reader.ReadInt32();
            height = reader.ReadInt32();

            // Dùng bounds tilemap hiện tại để align grid
            worldMap.CompressBounds();
            BoundsInt bounds = worldMap.cellBounds;

            int baseX = bounds.xMin;
            int baseY = bounds.yMin;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int objectId = reader.ReadInt32();

                    // Bỏ qua ground mặc định nếu muốn
                    //if (objectId == 0)
                    //    continue;

                    int gridX = baseX + x;
                    int gridY = baseY + y;

                    mapGroundMatrix[new Vector2(gridX, gridY)] = objectId;
                }
            }
        }

        Debug.Log("=== WORLD BASEMATRIX LOADED ===");
        Debug.Log($"Path   : {fullPath}");
        Debug.Log($"Size   : {width} x {height}");
        Debug.Log($"Cells  : {mapGroundMatrix.Count}");
    }
    public void setNextGlobalId(long nextId)
    {
        _nextID = nextId;
    }
    public long GenarateGlobalId()
    {
        return _nextID++;
    }
    public long getNextId()
    {
        return _nextID;
    }
    /// <summary>
    /// Kiểm tra 1 cell có đi được không (tọa độ tile)
    /// </summary>
    public bool IsWalkable(int tileX, int tileY)
    {
        if (tiles == null || tiles.Length == 0)
            return false;

        int x = tileX - offsetX;
        int y = tileY - offsetY;

        if (x < 0 || y < 0 || x >= width || y >= height)
            return false;

        int index = y * width + x;
        return tiles[index] == WALKABLE;
    }
    public Vector3Int WorldPosToCellPos(Vector3 worldPos)
    {
        return worldMap.WorldToCell(worldPos);
    }
    #region A*
    public List<Vector3Int> FindPath(Vector3Int start, Vector3Int target)
    {
        Dictionary<Vector3Int, AStarNode> allNodes = new();
        List<AStarNode> openSet = new();
        HashSet<Vector3Int> closedSet = new();

        AStarNode startNode = new(start);
        startNode.gCost = 0;
        startNode.hCost = Heuristic(start, target);

        openSet.Add(startNode);
        allNodes[start] = startNode;

        AStarNode bestNode = startNode; // node gần target nhất

        while (openSet.Count > 0)
        {
            AStarNode current = GetLowestFCostNode(openSet);

            openSet.Remove(current);
            closedSet.Add(current.pos);

            // Cập nhật bestNode nếu gần target hơn
            if (current.hCost < bestNode.hCost)
                bestNode = current;

            if (current.pos == target)
                return RetracePath(current);

            foreach (Vector3Int neighborPos in GetNeighbors(current.pos))
            {
                if (closedSet.Contains(neighborPos))
                    continue;

                if (!IsWalkable(neighborPos.x, neighborPos.y))
                    continue;

                int newGCost = current.gCost + 10;

                if (!allNodes.TryGetValue(neighborPos, out AStarNode neighbor))
                {
                    neighbor = new AStarNode(neighborPos);
                    allNodes[neighborPos] = neighbor;
                }

                if (!openSet.Contains(neighbor) || newGCost < neighbor.gCost)
                {
                    neighbor.gCost = newGCost;
                    neighbor.hCost = Heuristic(neighborPos, target);
                    neighbor.parent = current;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        // Nếu không tìm được đường tới target, trả về đường tới node gần target nhất
        return RetracePath(bestNode);
    }

    private AStarNode GetLowestFCostNode(List<AStarNode> nodes)
    {
        AStarNode best = nodes[0];

        for (int i = 1; i < nodes.Count; i++)
        {
            if (nodes[i].fCost < best.fCost ||
                nodes[i].fCost == best.fCost &&
                nodes[i].hCost < best.hCost)
            {
                best = nodes[i];
            }
        }

        return best;
    }

    private int Heuristic(Vector3Int a, Vector3Int b)
    {
        // Manhattan distance
        return (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y)) * 10;
    }

    private List<Vector3Int> GetNeighbors(Vector3Int pos)
    {
        return new List<Vector3Int>
    {
        new Vector3Int(pos.x + 1, pos.y, 0),
        new Vector3Int(pos.x - 1, pos.y, 0),
        new Vector3Int(pos.x, pos.y + 1, 0),
        new Vector3Int(pos.x, pos.y - 1, 0),
    };
    }

    private List<Vector3Int> RetracePath(AStarNode endNode)
    {
        List<Vector3Int> path = new();
        AStarNode current = endNode;

        while (current != null)
        {
            path.Add(current.pos);
            current = current.parent;
        }

        path.Reverse();
        return path;
    }
    #endregion
    public Vector3 CellPosToWorldCenter(Vector3Int cellPos)
    {
        return worldMap.GetCellCenterWorld(cellPos);
    }
    public void SetMatrixTile(Vector3Int pos,long id,bool setUseTile = false)
    {
        Vector2 cellPos = new Vector2(pos.x, pos.y);
        if(setUseTile)
        {
            worldMap.SetTile(pos, GameDatabase.Instance.TileDB.Get(id));
        }
        mapMatrix[cellPos] = id;
    }
    public void SetBaseMatrixTile(Vector3Int pos, int id)
    {
        Vector2 cellPos = new Vector2(pos.x, pos.y);
        mapBaseMatrix[cellPos] =id;
    }
    public void SetBaseGroundTile(Vector3Int pos, int id)
    {
        Vector2 cellPos = new Vector2(pos.x, pos.y);
        mapGroundMatrix[cellPos] = id;
    }
    public bool HasObjectOn(Vector2 cellppos)
    {
        return mapBaseMatrix[cellppos] != -1;
    }

}
