using System.Collections.Generic;
using UnityEngine;

public enum ToolNames
{
    None,
    Scythe,
    Axe,
    WaterCan
}
[CreateAssetMenu(
    menuName = "Scriptable Objects/Tool"
)]
public class Tool : ScriptableObject
{
    public ToolNames Name;
    public List<int> validObjectID = new();
    public bool CanUseOn(int blockid)
    {
        return validObjectID.Contains(blockid);
    }
}
