using UnityEngine;

[System.Serializable]
public class TerrainObjectRegion
{
    public string regionName;
    public int amountOfObj;

    [Range(0, 1)]
    public float minSpawnHeight;
    [Range(0, 1)]
    public float maxSpawnHeight;

    public GameObject terraingObject;
}
