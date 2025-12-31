using System.Buffers.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Scriptable Objects/Database/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    public List<ItemData> items = new List<ItemData>();    
    private Dictionary<int,ItemData> itemDict = new Dictionary<int,ItemData>();
    private void OnEnable()
    {
        BuildDictionary();
    }

    private void BuildDictionary()
    {
        foreach (ItemData data in items)
        {

            if (itemDict.ContainsKey(data.baseID))
            {
                Debug.LogWarning($"Trùng plantId: {data.baseID}");
                continue;
            }

            itemDict.Add(data.baseID, data);
        }
    }
    public ItemData GetItem(int id)
    {
        if (itemDict.TryGetValue(id, out ItemData data))
            return data;

        Debug.LogWarning($"Không tìm thấy ItemData: {id}");
        return null;
    }
    public bool CheckItemCanUseOn(int itemId,int blockId)
    {
        
        ItemData item = GetItem(itemId);
        if (item == null)
        {
            return false;
        }
        NotificationManager.Instance.ShowPopUpNotify("Đang check item " + item.Name + "trên id " + blockId);
        if (!item.canUseOnBlockIds.Contains(blockId)) return false;
        return true;
    }
    
}
