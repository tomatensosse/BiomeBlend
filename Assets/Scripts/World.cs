using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    // Singleton Instance
    public static World Instance { get; private set; }

    // Private field accessors
    public static WorldData Data => Instance._worldData;
    public static bool DataLoaded => Instance._dataLoaded;
    public static bool IsReady => Instance._isReady;
    public static List<Player> Players => Instance._players;
    public static Vector3Int RenderDistance => Instance._renderDistance;

    // Private fields
    private WorldData _worldData;
    private bool _dataLoaded = false;
    private bool _isReady = false;
    [SerializeField] private List<Player> _players = new List<Player>();
    [SerializeField] private Vector3Int _renderDistance = new Vector3Int(4, 4, 4);

    private List<Biome> _biomes = new List<Biome>();

    public MegaBiome air;
    public MegaBiome sea;
    public MegaBiome cavern;

    // For compute shaders
    public const int threadGroupSize = 8;

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
        StartCoroutine(WaitForData());
    }

    private IEnumerator WaitForData()
    {
        yield return new WaitUntil(() => _dataLoaded);

        // Any initialization

        _isReady = true;
    }

    public void LoadWorld(WorldData worldData)
    {
        _worldData = worldData;

        if (_worldData == null)
        {
            Debug.LogError("World data is null. Cannot load world.");
            return;
        }

        if (!_worldData.worldGenSettingsGenerated)
        {
            WorldGenSettings worldGenSettings = GenerateWorldGenSettings();
            _worldData.worldGenSettings = worldGenSettings;
            _worldData.worldGenSettingsGenerated = true;
        }

        Debug.Log($"Loaded world: {_worldData.worldName} (UID: {_worldData.uid}, Seed: {_worldData.seed})");

        _dataLoaded = true;
    }

    public WorldGenSettings GenerateWorldGenSettings()
    {
        int numPointsPerAxis = _worldData.worldSettings.ResolutionToNumPointsPerAxis;
        int chunkSize = _worldData.worldSettings.ResolutionToChunkSize;

        int megaChunkSize = _worldData.worldSettings.ResolutionToMegaChunkSize;
        int cavernLevel = _worldData.worldSettings.ResolutionToCavernLevel;
        int seaLevel = _worldData.worldSettings.ResolutionToSeaLevel;
        int skyLevel = _worldData.worldSettings.ResolutionToSkyLevel;

        int numVoxelsPerAxis = numPointsPerAxis - 1;
        int numVoxels = numPointsPerAxis * numPointsPerAxis * numPointsPerAxis;
        int maxTriangleCount = numVoxels * 5;

        WorldGenSettings worldGenSettings = new WorldGenSettings
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
            worldSize = _renderDistance * chunkSize,

            threadGroupSize = threadGroupSize,

            megaChunkSize = megaChunkSize,
            cavernLevel = cavernLevel,
            seaLevel = seaLevel,
            skyLevel = skyLevel
        };

        return worldGenSettings;
    }

    public Vector3Int WorldToRelativeChunkPosition(Vector3 worldPosition)
    {
        int chunkSize = Data.worldGenSettings.chunkSize;
        int megaChunkSize = Data.worldGenSettings.megaChunkSize * chunkSize;

        Vector3Int megaChunkPosition = WorldToMegaChunkPosition(worldPosition);
        Vector3 megaChunkOffset = new Vector3(
            megaChunkPosition.x * megaChunkSize,
            megaChunkPosition.y * megaChunkSize,
            megaChunkPosition.z * megaChunkSize
        );

        float offset = chunkSize / 2f;
        return new Vector3Int(
            Mathf.FloorToInt((worldPosition.x - megaChunkOffset.x) / chunkSize),
            Mathf.FloorToInt((worldPosition.y - megaChunkOffset.y) / chunkSize),
            Mathf.FloorToInt((worldPosition.z - megaChunkOffset.z) / chunkSize)
        );
    }

    public Vector3Int WorldToMegaChunkPosition(Vector3 worldPosition)
    {
        int megaChunkSize = Data.worldGenSettings.megaChunkSize * Data.worldGenSettings.chunkSize;
        float offset = megaChunkSize / 2f;
        return new Vector3Int(
            Mathf.FloorToInt((worldPosition.x + offset) / megaChunkSize),
            Mathf.FloorToInt((worldPosition.y + offset) / megaChunkSize),
            Mathf.FloorToInt((worldPosition.z + offset) / megaChunkSize)
        );
    }

    public int GetBiomeIndex(Biome biome)
    {
        return _biomes.IndexOf(biome);
    }

    public MegaBiome GetMegaBiome(Vector3Int megaBiomePosition)
    {
        if (megaBiomePosition.y > 0)
        {
            return air;
        }
        if (megaBiomePosition.y == 0)
        {
            return sea;
        }
        if (megaBiomePosition.y < 0)
        {
            return cavern;
        }

        Debug.LogError($"Invalid megaBiomePosition: {megaBiomePosition}. Cannot determine MegaBiome.");

        return null;
    }
}