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
    public bool canGrow;
    public int maxStage;
    public double growTimePerStage;
    public GameObject _prefab;
    public List<PlantStage> _stages = new List<PlantStage> ();
    public List<PlantStage> _witheredStage = new List<PlantStage>();
}
