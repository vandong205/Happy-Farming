using System.Collections.Generic;
using UnityEngine;

public enum ToolNames
{
    Hand,
    Scythe,
    Axe,
    WaterCan
}
[CreateAssetMenu(
    menuName = "Scriptable Objects/ToolData"
)]
public class ToolData : ScriptableObject
{
    public ToolNames Name;
    public List<int> validObjectID = new();
    public bool CanUseOn(int blockid)
    {
        return validObjectID.Contains(blockid);
    }
}
