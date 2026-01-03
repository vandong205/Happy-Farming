using UnityEngine;
using System.Collections.Generic;
public class PlantMangager : SingletonPattern<PlantMangager>
{
    [SerializeField] GameObject _cropPrefab;
    private Dictionary<long, PlantRuntimeData> plants = new Dictionary<long, PlantRuntimeData>();
    public PlantRuntimeData GetPlant(long id)
    {
        if (plants.TryGetValue(id, out PlantRuntimeData plant))
        {
            return plant;
        }
        else return null;
    }
    private void Update()
    {
        foreach (PlantRuntimeData plant in plants.Values) { 
            plant.TryGrow();
        }
    }
    public Vector2 GetPlantPos(long id)
    {
        PlantRuntimeData plant = GetPlant(id);
        if (plant != null)
        {
            return plant.cellPos;
        }
        else return new  Vector2(-1, -1);
    }
    public void PlantTree(Vector3Int pos,int baseid)
    {
        PlantData plant = GameDatabase.Instance.PlantDB.GetPlant(baseid);
        if (plant!=null)
        {
            var plantobj = Instantiate(_cropPrefab);
            PlantRuntimeData runtimePlant = plantobj.GetComponent<PlantRuntimeData>();
            if(runtimePlant == null)
            {
                NotificationManager.Instance.ShowPopUpNotify("Không tìm thấy PlantRuntimeData component, hủy trồng cây!", NotifyType.Error);
                return;
            }
            runtimePlant.Init(plant);
            long id = WorldManager.Instance.GenarateGlobalId();
            plants.Add(id,runtimePlant);
            WorldManager.Instance.SetMatrixTile(pos, id);
            WorldManager.Instance.SetBaseMatrixTile(pos, plant.baseID);
            plantobj.transform.position = WorldManager.Instance.CellPosToWorldCenter(pos);
            NotificationManager.Instance.ShowPopUpNotify($"Da trong cay {plant.baseName} tai {pos}", NotifyType.Info);

        }
    }
   
}
