using UnityEngine;

[CreateAssetMenu(fileName = "MBCavern", menuName = "MegaBiomes/Cavern")]
public class MBCavern : MegaBiome
{
    public Biome cavernBiome;

    public override int[,,] GenerateMap(Vector3Int megaBiomePosition)
    {
        int cavernLevel = World.Data.worldSettings.ResolutionToCavernLevel;

        int megaChunkSize = World.Data.worldSettings.ResolutionToChunkSize;

        int[,,] map = new int[megaChunkSize, megaChunkSize, megaChunkSize];

        for (int x = 0; x < megaChunkSize; x++)
        {
            for (int y = 0; y < megaChunkSize; y++)
            {
                for (int z = 0; z < megaChunkSize; z++)
                {
                    int worldX = megaBiomePosition.x * megaChunkSize + x;
                    int worldY = megaBiomePosition.y * megaChunkSize + y;
                    int worldZ = megaBiomePosition.z * megaChunkSize + z;

                    if (worldY >= cavernLevel)
                    {
                        map[x, y, z] = World.Instance.GetIndexOfBiome(cavernBiome);
                    }
                }
            }
        }

        return map;
    }
}