using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    public static ChunkGenerator Instance { get; private set; }
    public static Dictionary<Vector3Int, Chunk> Chunks => Instance.chunks;

    private Dictionary<Vector3Int, MegaChunk> megaChunks = new Dictionary<Vector3Int, MegaChunk>();
    protected List<MegaChunk> dirtyMegaChunks = new List<MegaChunk>();

    private Dictionary<Vector3Int, Chunk> chunks = new Dictionary<Vector3Int, Chunk>();
    protected List<Chunk> dirtyChunks = new List<Chunk>();

    public bool generateDensities = true;
    [ShowIf("generateDensities")] public bool generateMesh = true;
    [ShowIf("generateDensities")] public bool showDensities = false;

    private int staticSizeState = -1;
    private bool isBusy = false;

    private Dictionary<Player, Vector3Int> playerChunkPositions = new Dictionary<Player, Vector3Int>();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Update()
    {
        if (!World.IsReady)
        {
            return;
        }

        foreach (Player player in World.Players)
        {
            if (player == null)
            {
                Debug.LogWarning("Player is null, it should have been removed.");
                continue;
            }

            Vector3Int playerMegaChunkPosition = player.GetMegaChunkPosition();

            if (!megaChunks.ContainsKey(playerMegaChunkPosition))
            {
                GenerateMegaChunk(playerMegaChunkPosition);
            }

            Vector3Int playerChunkPosition = player.GetRelativeChunkPosition();

            if (!playerChunkPositions.ContainsKey(player))
            {
                LoadPlayer(player);
            }

            if (playerChunkPositions[player] != playerChunkPosition)
            {
                playerChunkPositions[player] = playerChunkPosition;

                GenerateChunksAroundPlayer(player);
            }
        }
    }

    private void LoadPlayer(Player player)
    {
        if (playerChunkPositions.ContainsKey(player))
        {
            Debug.LogWarning("Player already exists in chunk generator.");
            return;
        }

        playerChunkPositions.Add(player, player.GetRelativeChunkPosition());
    }

    private void GenerateChunksAroundPlayer(Player player)
    {
        Vector3Int playerChunkPosition = player.GetRelativeChunkPosition();
        Vector3Int megaChunkPosition = player.GetMegaChunkPosition();

        Vector3Int renderDistance = World.RenderDistance;

        for (int x = -renderDistance.x; x <= renderDistance.x; x++)
        {
            for (int y = -renderDistance.y; y <= renderDistance.y; y++)
            {
                for (int z = -renderDistance.z; z <= renderDistance.z; z++)
                {
                    Vector3Int position = playerChunkPosition + new Vector3Int(x, y, z);

                    if (ChunkOutOfBounds(position))
                    {
                        continue; // Skip chunks that are out of bounds
                    }

                    if (ChunkExistsAt(position))
                    {
                        continue; // Skip if chunk already exists
                    }

                    GenerateChunk(megaChunkPosition, position);
                }
            }
        }

        HandleDirtyChunks();

        GenerateBlendChunks();
    }

    private void GenerateChunk(Vector3Int megaChunkPosition, Vector3Int chunkPosition)
    {
        if (chunks.ContainsKey(chunkPosition)) return;

        if (!megaChunks.TryGetValue(megaChunkPosition, out MegaChunk megaChunk))
        {
            Debug.LogError($"MegaChunk at {megaChunkPosition} does not exist. Try generating it first.");
            return;
        }

        int biomeIndex = megaChunk.map[chunkPosition.x, chunkPosition.y, chunkPosition.z];

        if (biomeIndex == -2)
        {
            Debug.LogError("Empty chunk detected at " + chunkPosition + ". Skipping generation.");
            return;
        }

        GameObject chunk = new GameObject($"Chunk - ( {chunkPosition.x} |  {chunkPosition.y} | {chunkPosition.z} )");

        int chunkSize = World.Data.worldGenSettings.chunkSize;
        int megaChunkSize = World.Data.worldGenSettings.megaChunkSize * chunkSize;

        Vector3 relativeChunkOffset = new Vector3(
            megaChunkPosition.x * megaChunkSize,
            megaChunkPosition.y * megaChunkSize,
            megaChunkPosition.z * megaChunkSize
        );

        Vector3 megaChunkOffset = Vector3.one * (megaChunkSize / 2f);

        Vector3 chunkOffset = Vector3.one * (chunkSize / 2f);

        chunk.transform.position = (chunkSize * chunkPosition) + relativeChunkOffset + chunkOffset - megaChunkOffset;
        chunk.transform.SetParent(megaChunk.transform);

        Chunk chunkComponent;

        if (biomeIndex == -1)
        {
            // Generate blend chunk
            chunkComponent = chunk.AddComponent<BlendChunk>();
            chunkComponent.chunkPosition = chunkPosition;

            chunkComponent.Initialize();
        }
        else
        {
            // Generate biome chunk
            chunkComponent = chunk.AddComponent<BiomeChunk>();
            chunkComponent.chunkPosition = chunkPosition;

            GenerateBiomeChunk(chunkComponent as BiomeChunk, biomeIndex);
        }
    }

    private void GenerateBiomeChunk(BiomeChunk biomeChunk, int biomeIndex)
    {
        biomeChunk.biome = World.Instance.GetBiomeOfIndex(biomeIndex);

        biomeChunk.Initialize();

        if (generateDensities)
        {
            biomeChunk.GenerateDensity(showDensities);
        }

        if (generateDensities && generateMesh)
        {
            biomeChunk.GenerateMesh();
        }

        chunks.Add(biomeChunk.chunkPosition, biomeChunk);
    }

    private void GenerateMegaChunk(Vector3Int megaChunkPosition)
    {
        if (megaChunks.ContainsKey(megaChunkPosition)) return;

        GameObject megaChunk = new GameObject($"MegaChunk - ( {megaChunkPosition.x} |  {megaChunkPosition.y} | {megaChunkPosition.z} )");

        int megaChunkSize = World.Data.worldGenSettings.megaChunkSize * World.Data.worldGenSettings.chunkSize;

        megaChunk.transform.position = megaChunkSize * megaChunkPosition;
        megaChunk.transform.SetParent(transform);

        MegaChunk megaChunkComponent = megaChunk.AddComponent<MegaChunk>();

        megaChunkComponent.megaChunkPosition = megaChunkPosition;
        megaChunkComponent.megaBiome = World.Instance.GetMegaBiome(megaChunkPosition);
        megaChunkComponent.map = megaChunkComponent.megaBiome.GenerateMap(megaChunkPosition);

        megaChunks.Add(megaChunkPosition, megaChunkComponent);

        HandleDirtyMegaChunks();
    }

    private void HandleDirtyChunks()
    {
        dirtyChunks.Clear(); // Optional safety if dirtyChunks was not reset before

        foreach (Chunk chunk in chunks.Values)
        {
            if (chunk == null) continue; // Prevent accessing destroyed chunks

            if (!ChunkInRenderDistance(chunk.chunkPosition))
            {
                dirtyChunks.Add(chunk);
            }
        }

        for (int i = dirtyChunks.Count - 1; i >= 0; i--)
        {
            Chunk chunk = dirtyChunks[i];
            chunks.Remove(chunk.chunkPosition);

            if (chunk == null) // Already destroyed, skip
            {
                dirtyChunks.RemoveAt(i);
                continue;
            }

            GameObject chunkGameObject = chunk.gameObject;
            dirtyChunks.RemoveAt(i);

            Destroy(chunkGameObject);
        }
    }

    private void GenerateBlendChunks()
    {
        // LOL
    }

    private void HandleDirtyMegaChunks()
    {
        dirtyMegaChunks.Clear(); // Optional safety if dirtyMegaChunks was not reset before

        foreach (Player player in World.Players)
        {
            Vector3Int playerMegaChunkPosition = player.GetMegaChunkPosition();

            foreach (MegaChunk megaChunk in megaChunks.Values)
            {
                if (megaChunk.megaChunkPosition != playerMegaChunkPosition)
                {
                    dirtyMegaChunks.Add(megaChunk);
                }
            }
        }

        for (int i = dirtyMegaChunks.Count - 1; i >= 0; i--)
        {
            MegaChunk megaChunk = dirtyMegaChunks[i];
            megaChunks.Remove(megaChunk.megaChunkPosition);

            if (megaChunk == null) // Already destroyed, skip
            {
                dirtyMegaChunks.RemoveAt(i);
                continue;
            }

            GameObject megaChunkGameObject = megaChunk.gameObject;
            dirtyMegaChunks.RemoveAt(i);

            Destroy(megaChunkGameObject);
        }
    }

    private bool ChunkInRenderDistance(Vector3Int chunkPosition)
    {
        foreach (Player player in World.Players)
        {
            Vector3Int playerChunkPosition = player.GetRelativeChunkPosition();

            Vector3Int renderDistance = World.RenderDistance;


            for (int x = -renderDistance.x; x <= renderDistance.x; x++)
            {
                for (int y = -renderDistance.y; y <= renderDistance.y; y++)
                {
                    for (int z = -renderDistance.z; z <= renderDistance.z; z++)
                    {
                        Vector3Int position = playerChunkPosition + new Vector3Int(x, y, z);

                        if (chunkPosition == position)
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    private bool ChunkOutOfBounds(Vector3Int chunkPosition)
    {
        int megaChunkSize = World.Data.worldGenSettings.megaChunkSize;

        return chunkPosition.x < 0 || chunkPosition.x >= megaChunkSize ||
               chunkPosition.y < 0 || chunkPosition.y >= megaChunkSize ||
               chunkPosition.z < 0 || chunkPosition.z >= megaChunkSize;
    }

    private bool ChunkExistsAt(Vector3Int chunkPosition)
    {
        if (chunks.ContainsKey(chunkPosition))
        {
            return true;
        }

        return false;
    }
}