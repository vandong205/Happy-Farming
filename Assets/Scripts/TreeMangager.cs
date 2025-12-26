using UnityEngine;
using System.Collections.Generic;
public class TreeMangager : SingletonPattern<TreeMangager>
{
    private Dictionary<int, Vector2> treePos = new();
    public Vector2 GetTreePos(int id) => treePos[id];
    public void SpawnTree(int treeId)
    {

    }
    public void DeleteTree(int treeId)
    {
        treePos.Remove(treeId);
    }
}
