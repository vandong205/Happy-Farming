using UnityEngine;

public class PlantRuntimeData : MonoBehaviour
{
    [SerializeField] SpriteRenderer _render;
    private int baseID;

    public Vector2 cellPos { get; private set; }
    public string baseName { get; private set; }
    public int havestId { get; private set; }
    public int ID { get; private set; }
    public bool canGrow { get; private set; }
    public int currentStage { get; private set; }
    public int maxStage { get; private set; }
    public bool witherd { get; private set; }


    // 👉 GIÂY GAME
    public double nextGrowTime { get; private set; }
    public double lastUpdateTime { get; private set; }

    private PlantData _data;

    public void Init(PlantData data)
    {
        _data = data;
            _render = GetComponent<SpriteRenderer>();
        Vector3Int pos =
            WorldManager.Instance.WorldPosToCellPos(transform.position);
        cellPos = new Vector2(pos.x, pos.y);
        baseName = data.baseName;
        canGrow = data.canGrow;
        currentStage = 1;
        maxStage = data.maxStage;
        witherd = false;
        havestId = data.harvestId;
        _render.sprite = data._stages[currentStage - 1]._sprite;
        lastUpdateTime = GameTimer.Instance.GameTimeSeconds;
        ScheduleNextGrow();
    }

    void ScheduleNextGrow()
    {   
        nextGrowTime = lastUpdateTime + _data.growTimePerStagePercentOfDay*GameTimer.GAME_SECONDS_PER_DAY;
    }

    public void TryGrow()
    {
        if (!canGrow || witherd) return;
        if (currentStage >= maxStage) return;

        double now = GameTimer.Instance.GameTimeSeconds;
        if (now < nextGrowTime) return;

        GrowUp();

        lastUpdateTime = now;
        ScheduleNextGrow();
    }

    public void GrowUp()
    {
        currentStage++;

        _render.sprite =
            _data._stages[currentStage - 1]._sprite;

        Debug.Log(
            $"Cay {baseName} tai {cellPos} len stage {currentStage}"
        );
    }

    public void BeWithered()
    {
        witherd = true;
        _render.sprite =
            GameDatabase.Instance
                .PlantDB.GetPlant(baseID)
                ._witheredStage[currentStage - 1]._sprite;
    }
}
