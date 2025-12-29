using UnityEngine;

public class GameDatabase : SingletonPattern<GameDatabase>
{
    [SerializeField] ToolDatabase toolDB;
    [SerializeField] PlantDatabase plantDB;
    [SerializeField] TileBaseDatabase tileDB;
    public ToolDatabase ToolDB
    {
        get { return toolDB; }
    }
    public PlantDatabase PlantDB
    {
        get { return plantDB; }
    }
    public TileBaseDatabase TileDB
    {
        get { return tileDB; }
    }
}
