using System.IO;
using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class WorldManager : SingletonPattern<WorldManager>
{
    [SerializeField] Tilemap worldMap;
    private int width;
    private int height;
    private int offsetX;
    private int offsetY;
    private byte[] tiles;

    private const byte WALKABLE = 1;

    /// <summary>
    /// Đọc dữ liệu map từ file .bin
    /// </summary>
    /// <param name="fullPath">Đường dẫn đầy đủ tới file</param>
    public IEnumerator LoadFromFile(string fullPath)
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

        while (openSet.Count > 0)
        {
            AStarNode current = GetLowestFCostNode(openSet);

            openSet.Remove(current);
            closedSet.Add(current.pos);

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

        return null;
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


}
