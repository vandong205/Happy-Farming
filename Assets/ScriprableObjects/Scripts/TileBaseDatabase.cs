using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Scriptable Objects/Database/TileDatabase")]
public class TileBaseDatabase : ScriptableObject
{
    public List<TileEntry> tiles = new();

    private Dictionary<int, TileBase> dict = new();

    private void OnEnable()
    {
        dict = new();
        foreach (var t in tiles)
            dict[t.id] = t.tile;
    }

    public TileBase Get(int id) => dict[id];
}

[System.Serializable]
public class TileEntry
{
    public int id;
    public TileBase tile;
}
