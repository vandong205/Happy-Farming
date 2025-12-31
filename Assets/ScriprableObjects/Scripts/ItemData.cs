using UnityEngine;
using System.Collections.Generic;
public enum ItemType
{
    PlantSeed,
    Structure
}

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    public int baseID;
    public string Name;
    public ItemType Type;
    public List<int> canUseOnBlockIds = new List<int>();
    public int durability;
    public int cost;
    public Sprite sprite;

}
