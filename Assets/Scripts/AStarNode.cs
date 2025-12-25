using UnityEngine;

public class AStarNode
{
    public Vector3Int pos;
    public int gCost;
    public int hCost;
    public int fCost => gCost + hCost;
    public AStarNode parent;

    public AStarNode(Vector3Int pos)
    {
        this.pos = pos;
    }
}