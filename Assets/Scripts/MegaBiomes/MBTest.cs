using UnityEngine;

[CreateAssetMenu(fileName = "MBTest", menuName = "MegaBiomes/Test")]
public class MBTest : MegaBiome
{
    public Biome snow;
    public Biome plains;

    public float noiseScale = 0.1f;

    public override int[,,] GenerateMap(Vector3Int megaBiomePosition)
    {
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

                    float noiseValue = Mathf.PerlinNoise(worldX * noiseScale, worldZ * noiseScale);

                    if (noiseValue > 0.5f)
                    {
                        map[x, y, z] = World.Instance.GetIndexOfBiome(snow);
                    }
                    else if (worldY < 0)
                    {
                        map[x, y, z] = World.Instance.GetIndexOfBiome(snow);
                    }
                }
            }
        }

        // Make different neighbor chunks -1

        for (int x = 0; x < megaChunkSize; x++)
        {
            for (int y = 0; y < megaChunkSize; y++)
            {
                for (int z = 0; z < megaChunkSize; z++)
                {
                    if (map[x, y, z] == World.Instance.GetIndexOfBiome(snow))
                    {
                        if (x > 0 && map[x - 1, y, z] != World.Instance.GetIndexOfBiome(snow))
                        {
                            map[x - 1, y, z] = -1;
                        }
                        if (y > 0 && map[x, y - 1, z] != World.Instance.GetIndexOfBiome(snow))
                        {
                            map[x, y - 1, z] = -1;
                        }
                        if (z > 0 && map[x, y, z - 1] != World.Instance.GetIndexOfBiome(snow))
                        {
                            map[x, y, z - 1] = -1;
                        }
                    }
                }
            }
        }

        return map;
    }
}