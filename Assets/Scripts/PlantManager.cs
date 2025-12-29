using UnityEngine;
using System.Collections.Generic;
public class PlantMangager : MonoBehaviour
{
    private Dictionary<int,Plant> plants = new Dictionary<int,Plant>();
    private Plant GetPlant(int id)
    {
        if (plants.TryGetValue(id, out Plant plant))
        {
            return plant;
        }
        else return null;
    }
    public Vector2 GetPlantPos(int id)
    {
        Plant plant = GetPlant(id);
        if (plant != null)
        {
            return plant.cellPos;
        }
        else return new  Vector2(-1, -1);
    }
    public void GrowPlant(int id)
    {
        Plant plant = GetPlant(id);
        if (plant != null)
        {
            plant.GrowUp();
        }
    }
    public void PlantTree(Vector2 cellPos,int baseid)
    {

    }
   
}
