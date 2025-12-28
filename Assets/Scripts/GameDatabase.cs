using UnityEngine;

public class GameDatabase : SingletonPattern<GameDatabase>
{
    [SerializeField] ToolDatabase toolDB;
    public ToolDatabase ToolDB
    {
        get { return toolDB; }
        private set { 
            toolDB = value;
        }
    }
}
