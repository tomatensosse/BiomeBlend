using System.Collections.Generic;
using UnityEngine;

public class BlendChunk : Chunk
{
    protected List<Vector3Int> directions = new List<Vector3Int>
    {
        new Vector3Int(1, 0, 0), // Right
        new Vector3Int(-1, 0, 0), // Left
        new Vector3Int(0, 1, 0), // Up
        new Vector3Int(0, -1, 0), // Down
        new Vector3Int(0, 0, 1), // Forward
        new Vector3Int(0, 0, -1), // Backward

        new Vector3Int(1, 1, 0), // Right Up
        new Vector3Int(-1, 1, 0), // Left Up
        new Vector3Int(1, -1, 0), // Right Down
        new Vector3Int(-1, -1, 0), // Left Down

        new Vector3Int(1, 0, 1), // Right Forward
        new Vector3Int(-1, 0, 1), // Left Forward
        new Vector3Int(1, 0, -1), // Right Backward
        new Vector3Int(-1, 0, -1), // Left Backward
        new Vector3Int(0, 1, 1), // Up Forward
        new Vector3Int(0, -1, 1), // Down Forward
        new Vector3Int(0, 1, -1), // Up Backward
        new Vector3Int(0, -1, -1), // Down Backward

        new Vector3Int(1, 1, 1), // Right Up Forward
        new Vector3Int(-1, 1, 1), // Left Up Forward
        new Vector3Int(1, -1, 1), // Right Down Forward
        new Vector3Int(-1, -1, 1), // Left Down Forward

        new Vector3Int(1, 1, -1), // Right Up Backward
        new Vector3Int(-1, 1, -1), // Left Up Backward
        new Vector3Int(1, -1, -1), // Right Down Backward
        new Vector3Int(-1, -1, -1) // Left Down Backward
    };

    protected Dictionary<Vector3Int, Chunk> neighborChunks = new Dictionary<Vector3Int, Chunk>();

    protected bool neighborsAvailable = false;
    protected bool blendComplete = false;

    public override void GenerateDensity(bool showDensities = false)
    {
        throw new System.NotImplementedException();
    }

    public override void GenerateMesh()
    {
        throw new System.NotImplementedException();
    }

    public void CheckBlendAvailability()
    {
        if (blendComplete) { return; }

        FindNeighbors();

        if (neighborChunks.Count == 26)
        {
            neighborsAvailable = true;
        }

        if (neighborsAvailable)
        {
            // blend the chunk
        }
    }

    public void FindNeighbors()
    {
        neighborChunks.Clear();

        foreach (var direction in directions)
        {
            Vector3Int neighborPosition = chunkPosition + direction;
            if (ChunkGenerator.Chunks.TryGetValue(neighborPosition, out Chunk neighborChunk))
            {
                neighborChunks[neighborPosition] = neighborChunk;
            }
        }
    }
}