using UnityEngine;

public abstract class MegaBiome : ScriptableObject
{
    public string uid;
    public string megaBiomeName;

    public abstract int[,,] GenerateMap(Vector3Int megaBiomePosition);
}