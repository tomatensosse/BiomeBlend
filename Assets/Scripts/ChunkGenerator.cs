using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    public static ChunkGenerator Instance { get; private set; }
    public static Dictionary<Vector3Int, Chunk> Chunks => Instance.chunks;

    private Dictionary<Vector3Int, Chunk> chunks = new Dictionary<Vector3Int, Chunk>();
    protected Dictionary<Vector3Int, Chunk> dirtyChunks = new Dictionary<Vector3Int, Chunk>();

    public bool generateMesh = true;

    public bool showDensities = false;

    private int staticSizeState = -1;
    private bool isBusy = false;

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
        StartCoroutine(WaitAndGenerateStatic());
    }

    void Update()
    {
        if (isBusy) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateStatic();
        }
    }

    private IEnumerator WaitAndGenerateStatic()
    {
        yield return new WaitUntil(() => World.IsReady);

        GenerateStatic();
    }

    private void GenerateStatic()
    {
        isBusy = true;

        FindAndDestroyExistingChunks();

        World.Instance.GenerateConstants();

        Vector3Int worldSize = World.WorldGenSettings.worldSize;

        for (int x = 0; x < worldSize.x; x++)
        {
            for (int y = 0; y < worldSize.y; y++)
            {
                for (int z = 0; z < worldSize.z; z++)
                {
                    GenerateChunk(new Vector3Int(x, y, z));
                }
            }
        }

        FindNeighbors();

        isBusy = false;
    }

    private void GenerateChunk(Vector3Int position)
    {
        if (chunks.ContainsKey(position)) return;

        GameObject chunk = new GameObject($"Chunk - ( {position.x} |  {position.y} | {position.z} )");
        chunk.transform.position = World.WorldGenSettings.chunkSize * position;
        chunk.transform.SetParent(transform);

        Chunk chunkComponent = chunk.AddComponent<Chunk>();

        chunkComponent.biome = World.Instance.SampleBiomeForChunk(position);

        chunkComponent.chunkPosition = position;
        chunkComponent.Initialize();

        chunkComponent.GenerateDensity(showDensities);

        if (generateMesh)
        {
            chunkComponent.GenerateMesh();
        }

        chunks.Add(position, chunkComponent);
    }

    private void FindAndDestroyExistingChunks()
    {
        foreach (var chunk in chunks.Values)
        {
            if (chunk != null)
            {
                Destroy(chunk.gameObject);
            }
        }

        chunks.Clear();
    }

    private void FindNeighbors()
    {
        foreach (var chunk in chunks.Values)
        {
            chunk.FindNeighbors();
        }
    }
}