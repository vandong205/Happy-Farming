using UnityEngine;

[CreateAssetMenu(fileName = "PlantData", menuName = "Scriptable Objects/PlantData")]
public class PlantData : ScriptableObject
{
    public string baseName;
    public int baseID;
    public bool canGrow;
    public bool canPlantOnGrass;
    public int maxStage;
    
}
