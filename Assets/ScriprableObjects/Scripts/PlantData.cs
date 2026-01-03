using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlantData", menuName = "Scriptable Objects/PlantData")]
public class PlantData : ScriptableObject
{
    [System.Serializable]
    public struct PlantStage
    {
        public int _stage;
        public Sprite _sprite;
    }
    public string baseName;
    public int baseID;
    public int harvestId;
    public bool canGrow;
    public int maxStage;
    public double growTimePerStagePercentOfDay;
    public List<PlantStage> _stages = new List<PlantStage> ();
    public List<PlantStage> _witheredStage = new List<PlantStage>();
}
