using UnityEngine;
public enum ObjectType
{
    none,
    crop,
    tree,
}
public static class ObjectInfo
{
    public static ObjectType GetTypeById(int id)
    {
        if(id>=201&&id<=300) return ObjectType.crop;
        return ObjectType.none;
    }

}
