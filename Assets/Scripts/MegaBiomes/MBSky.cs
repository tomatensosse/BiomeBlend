using UnityEngine;

[CreateAssetMenu(fileName = "MBSky", menuName = "MegaBiomes/Sky")]
public class MBSky : MegaBiome
{
    public Biome skyBiome;

    public override int[,,] GenerateMap(Vector3Int megaBiomePosition)
    {
        int skyLevel = World.Data.worldSettings.ResolutionToSkyLevel;
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
                        map[x, y, z] = World.Instance.GetBiomeIndex(skyBiome);
                    }
                }
            }
        }

        return map;
    }
}