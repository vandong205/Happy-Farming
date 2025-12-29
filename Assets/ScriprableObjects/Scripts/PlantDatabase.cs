using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "PlantDatabase",
    menuName = "Scriptable Objects/Database/PlantDatabase")]
public class PlantDatabase : ScriptableObject
{
    [Header("Editable in Inspector")]
    [SerializeField]
    private List<PlantData> plantDatas = new List<PlantData>();

    private Dictionary<int, PlantData> plantDict;

    private void OnEnable()
    {
        BuildDictionary();
    }

    private void BuildDictionary()
    {
        plantDict = new Dictionary<int, PlantData>();

        foreach (PlantData data in plantDatas)
        {

            if (plantDict.ContainsKey(data.baseID))
            {
                Debug.LogWarning($"Trùng plantId: {data.baseID}");
                continue;
            }

            plantDict.Add(data.baseID, data);
        }
    }
    public PlantData GetPlant(int baseid)
    {
        if (plantDict.TryGetValue(baseid, out PlantData data))
            return data;

        Debug.LogWarning($"Không tìm thấy PlantData: {baseid}");
        return null;
    }
    public int Count()
    {
        return plantDict.Count;
    }
}
