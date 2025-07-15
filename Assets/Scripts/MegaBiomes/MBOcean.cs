using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MBOcean", menuName = "MegaBiomes/Ocean")]
public class MBOcean : MegaBiome
{
    public Biome skyBiome;
    public Biome seaBiome;
    public Biome cavernBiome;

    public override int[,,] GenerateMap(Vector3Int megaBiomePosition)
    {
        int skyLevel = World.Data.worldSettings.ResolutionToSkyLevel;
        int seaLevel = World.Data.worldSettings.ResolutionToSeaLevel;
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

                    if (worldY >= skyLevel)
                    {
                        map[x, y, z] = World.Instance.GetIndexOfBiome(skyBiome);
                    }
                    else if (worldY >= seaLevel)
                    {
                        map[x, y, z] = World.Instance.GetIndexOfBiome(seaBiome);
                    }
                    else if (worldY >= cavernLevel)
                    {
                        map[x, y, z] = World.Instance.GetIndexOfBiome(cavernBiome);
                    }
                }
            }
        }

        return map;
    }
}