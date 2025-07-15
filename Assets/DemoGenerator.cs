using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoGenerator : MonoBehaviour
{
    public static DemoGenerator Instance { get; private set; }
    public static bool IsReady => Instance._isReady;

    private bool _isReady = false;

    protected int[,,] map = new int[5, 5, 5]
    {
        {
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 }
        },
        {
            { 0, 0, 0, 0, 0 },
            { 0, -1, -1, -1, 0 },
            { 0, -1, -1, -1, 0 },
            { 0, -1, -1, -1, 0 },
            { 0, 0, 0, 0, 0 }
        },
        {
            { 0, 0, 0, 0, 0 },
            { 0, -1, -1, -1, 0 },
            { 0, -1, 1, -1, 0 },
            { 0, -1, -1, -1, 0 },
            { 0, 0, 0, 0, 0 }
        },
        {
            { 0, 0, 0, 0, 0 },
            { 0, -1, -1, -1, 0 },
            { 0, -1, -1, -1, 0 },
            { 0, -1, -1, -1, 0 },
            { 0, 0, 0, 0, 0 }
        },
        {
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 }
        }
    };

    public Biome biome1;
    public Biome biome2;

    [Header("Parameters")]
    public int chunkSize = 8;
    public int numPointsPerAxis = 4;
    public Vector3Int generateSize = new Vector3Int(5, 5, 5);

    public int seed = 0;

    [HideInInspector] public int numPoints;
    [HideInInspector] public int numVoxelsPerAxis;
    [HideInInspector] public int numVoxels;
    [HideInInspector] public int maxTriangleCount;
    [HideInInspector] public int numThreadsPerAxisP;
    [HideInInspector] public int numThreadsPerAxisV;
    [HideInInspector] public int boundsSize;
    [HideInInspector] public Vector3 worldSize;
    [HideInInspector] public float pointSpacing;
    [HideInInspector] public int threadGroupSize = 8;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        Debug.Log("World size: " + map.Length);

        GenerateConstants();

        StartCoroutine(WaitAndGenerateWorld());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            DeleteAllChunks();
            GenerateWorld();
        }
    }

    IEnumerator WaitAndGenerateWorld()
    {
        yield return new WaitUntil(() => MeshGenerator.IsReady);

        GenerateWorld();
    }

    private void GenerateConstants()
    {
        numPoints = numPointsPerAxis * numPointsPerAxis * numPointsPerAxis;
        numVoxelsPerAxis = numPointsPerAxis - 1;
        numVoxels = numPointsPerAxis * numPointsPerAxis * numPointsPerAxis;
        maxTriangleCount = numVoxels * 5;
        numThreadsPerAxisP = Mathf.CeilToInt((float)numPointsPerAxis / threadGroupSize);
        numThreadsPerAxisV = Mathf.CeilToInt((float)numVoxelsPerAxis / threadGroupSize);
        boundsSize = chunkSize;
        pointSpacing = chunkSize / ((float)numPointsPerAxis - 1);
        worldSize = generateSize * chunkSize;

        _isReady = true;
    }

    private void GenerateWorld()
    {
        for (int x = 0; x < generateSize.x; x++)
        {
            for (int y = 0; y < generateSize.y; y++)
            {
                for (int z = 0; z < generateSize.z; z++)
                {
                    Vector3Int chunkPosition = new Vector3Int(x, y, z);

                    Debug.Log("Generating chunk at position: " + chunkPosition);

                    int biomeIndex = map[x, y, z];

                    if (biomeIndex == -1)
                    {
                        GenerateBlendChunk(chunkPosition);
                    }

                    if (biomeIndex >= 0)
                    {
                        GenerateBiomeChunk(chunkPosition, biomeIndex);
                    }
                }
            }
        }
    }

    private void GenerateBiomeChunk(Vector3Int chunkPosition, int biomeIndex)
    {
        GameObject chunkObject = new GameObject($"BiomeChunk_{chunkPosition.x}_{chunkPosition.y}_{chunkPosition.z}");
        chunkObject.transform.position = chunkPosition * chunkSize;

        BiomeChunk biomeChunk = chunkObject.AddComponent<BiomeChunk>();
        biomeChunk.chunkPosition = chunkPosition;
        biomeChunk.Initialize();

        biomeChunk.biome = (biomeIndex == 0) ? biome1 : biome2; // proper biome retrieve on deploy

        biomeChunk.GenerateDensity();
        biomeChunk.GenerateMesh();
    }

    private void GenerateBlendChunk(Vector3Int chunkPosition)
    {
        GameObject chunkObject = new GameObject($"BlendChunk_{chunkPosition.x}_{chunkPosition.y}_{chunkPosition.z}");
        chunkObject.transform.position = chunkPosition * chunkSize;

        BlendChunk blendChunk = chunkObject.AddComponent<BlendChunk>();
        blendChunk.chunkPosition = chunkPosition;
        blendChunk.Initialize();
    }

    private void DeleteAllChunks()
    {
        foreach (var chunk in FindObjectsOfType<Chunk>())
        {
            Destroy(chunk.gameObject);
        }
    }
}
