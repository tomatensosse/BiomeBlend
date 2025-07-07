using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World Instance { get; private set; }
    public static WorldGenSettings WorldGenSettings => Instance._worldGenSettings;
    public static int Seed => Instance._seed;
    public static bool IsReady => Instance._isReady;

    private bool _isReady = false;

    public List<Biome> biomes = new List<Biome>();
    public float biomeNoiseScale = 0.1f;

    [Header("World Settings")]
    public int setSeed = 0;
    private int _seed;

    public Vector3Int worldSize = new Vector3Int(3, 1, 3);

    public int chunkSize = 8;
    public int numPointsPerAxis = 4;

    public const int threadGroupSize = 8;

    private WorldGenSettings _worldGenSettings;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        GenerateConstants();
    }

    public void GenerateConstants()
    {
        _isReady = false;

        _seed = setSeed == 0 ? Random.Range(0, int.MaxValue) : setSeed;

        int numVoxelsPerAxis = numPointsPerAxis - 1;
        int numVoxels = numPointsPerAxis * numPointsPerAxis * numPointsPerAxis;
        int maxTriangleCount = numVoxels * 5;

        _worldGenSettings = new WorldGenSettings
        {
            chunkSize = chunkSize,
            numPoints = numPointsPerAxis * numPointsPerAxis * numPointsPerAxis,
            numPointsPerAxis = numPointsPerAxis,
            numVoxelsPerAxis = numVoxelsPerAxis,
            numVoxels = numVoxels,
            maxTriangleCount = maxTriangleCount,
            numThreadsPerAxis = Mathf.CeilToInt((float)numVoxelsPerAxis / threadGroupSize),
            boundsSize = chunkSize,
            pointSpacing = chunkSize / ((float)numPointsPerAxis - 1),
            worldSize = worldSize,

            threadGroupSize = threadGroupSize,
        };

        _isReady = true;
    }

    public Biome SampleBiomeForChunk(Vector3Int chunkPosition)
    {
        float treshold = 1.0f / biomes.Count;
        float noise = Mathf.PerlinNoise(_seed + chunkPosition.x * biomeNoiseScale, _seed + chunkPosition.z * biomeNoiseScale);

        for (int i = 0; i < biomes.Count; i++)
        {
            if (noise < treshold * (i + 1))
            {
                return biomes[i];
            }
        }

        Debug.LogWarning("No biome found for chunk position: " + chunkPosition + ". Returning null.");

        return null;
    }
}