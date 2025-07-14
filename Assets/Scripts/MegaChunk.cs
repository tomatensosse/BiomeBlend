using UnityEngine;

public class MegaChunk : MonoBehaviour
{
    [Header("Mega Chunk Parameters")]
    public Vector3Int megaChunkPosition;
    public MegaBiome megaBiome;

    public int[,,] map;

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        Gizmos.color = Color.white;

        int megaChunkSize = World.Data.worldGenSettings.megaChunkSize * World.Data.worldGenSettings.chunkSize;
        Gizmos.DrawWireCube(transform.position, Vector3.one * megaChunkSize);
    }
}
