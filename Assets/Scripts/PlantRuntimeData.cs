using UnityEngine;

public class PlantRuntimeData : MonoBehaviour
{
    [SerializeField] SpriteRenderer _render;
    [SerializeField] int baseID;
    public  Vector2 cellPos {  get; private set; }
    public string baseName { get; private set; }
    public int ID { get; private set; }
    public bool canGrow { get; private set; }
    public int currentStage { get; private set; }
    public int maxStage { get; private set; }
    public bool witherd { get; private set; }
    public double nextGrowTime { get; private set; }
    public double lastUpdateTime { get; private set; }
    private void Awake()
    {
        if (_render == null)
            _render = GetComponent<SpriteRenderer>();
        if (GameDatabase.Instance != null)
        {
            PlantData _data = GameDatabase.Instance.PlantDB.GetPlant(baseID);
            if (_data != null) { 
                Vector3Int pos = WorldManager.Instance.WorldPosToCellPos(transform.position);
                cellPos = new Vector2(pos.x, pos.y);
                baseName = _data.baseName;
                canGrow = _data.canGrow;
                currentStage = 1;
                maxStage = _data.maxStage;
                witherd = false;
                _render.sprite = _data._stages[currentStage - 1]._sprite;
            }
           
        }
        lastUpdateTime = GameTimer.Instance.CurrentTime;
        ScheduleNextGrow();

    }

    void ScheduleNextGrow()
    {
        PlantData data = GameDatabase.Instance.PlantDB.GetPlant(baseID);
        nextGrowTime = lastUpdateTime + data.growTimePerStage;
    }
    public void TryGrow(double now)
    {
        if (!canGrow || witherd) return;
        if (currentStage >= maxStage) return;
        if (now < nextGrowTime) return;

        GrowUp();

        lastUpdateTime = now;
        ScheduleNextGrow();
    }
    public void GrowUp()
    {
        currentStage++;
        PlantData data = GameDatabase.Instance.PlantDB.GetPlant(baseID);
        _render.sprite = data._stages[currentStage - 1]._sprite;

        Debug.Log($"Cay {baseName} tai {cellPos} len stage {currentStage}");
    }
    public void BeWithered()
    {
        witherd = true;
        _render.sprite = GameDatabase.Instance
            .PlantDB.GetPlant(baseID)._witheredStage[currentStage-1]._sprite;
    }
}


