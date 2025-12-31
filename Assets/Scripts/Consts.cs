using System.IO;
using UnityEngine;

public static class Consts
{
    public static class DataPaths
    {
        public static readonly string map_walkabledata = Path.Combine( Application.persistentDataPath,"map_walkable.bin");
        public static readonly string tree_data = Path.Combine(Application.persistentDataPath, "tree_data.bin");
        public static readonly string world_matrix = Path.Combine(Application.persistentDataPath, "Maps/map.bin");
        public static readonly string world_basematrix = Path.Combine(Application.persistentDataPath, "Maps/mapbase.bin");
        public static readonly string world_groundmatrix = Path.Combine(Application.persistentDataPath, "Maps/mapground.bin");
    }
    public static class ConfigAdress
    {
        public static readonly string tree_config_key = ""; 
    }
}
