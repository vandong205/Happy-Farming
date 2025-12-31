using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "ToolDatabase",
    menuName = "Scriptable Objects/Database/ToolDatabase"
)]
public class ToolDatabase : ScriptableObject
{
    [SerializeField] private List<ToolData> tools;

    private Dictionary<ToolNames, ToolData> toolDict;

    private void OnEnable()
    {
        BuildDictionary();
    }

    private void BuildDictionary()
    {
        toolDict = new Dictionary<ToolNames, ToolData>();

        foreach (var tool in tools)
        {
            if (tool == null) continue;

            if (!toolDict.ContainsKey(tool.Name))
                toolDict.Add(tool.Name, tool);
        }
    }

    public bool CheckToolCanUseOn(ToolNames toolName, int id)
    {
        Debug.Log("Dang check tool" + toolName + "tren" + id);
        if (toolDict == null) BuildDictionary();
        return toolDict.TryGetValue(toolName, out var tool)
               && tool.CanUseOn(id);
    }
}
