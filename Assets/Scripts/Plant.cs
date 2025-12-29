using UnityEngine;

public class Plant : MonoBehaviour
{
    public Vector2 cellPos { get; private set; }
    public string baseName { get; private set; }
    public int ID { get; private set; }
    public int baseID { get; private set; }
    public bool canGrow {  get; private set; }
    public bool canPlantOnGrass { get; private set; }
    public int currentStage {  get; private set; }
    public int maxStage { get; private set; }
    public void GrowUp()
    {
        if(!canGrow) return;
        if(currentStage == maxStage) return;
        Debug.Log($"Cay {ID} tai {cellPos}dang phat trien");
    }
    public void BeWithered()
    {

    }
}
