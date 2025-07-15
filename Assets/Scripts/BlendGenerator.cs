using System.Collections.Generic;
using UnityEngine;

public class BlendGenerator : MonoBehaviour
{
    public static BlendGenerator Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public float[,,] BlendChunk(BlendChunk blendChunk, Dictionary<Vector3Int, Chunk> neighborChunks)
    {
        return null;
    }
}