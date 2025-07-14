using UnityEngine;

public class Player : MonoBehaviour
{
    public Vector3Int GetRelativeChunkPosition()
    {
        return World.Instance.WorldToRelativeChunkPosition(transform.position);
    }

    public Vector3Int GetMegaChunkPosition()
    {
        return World.Instance.WorldToMegaChunkPosition(transform.position);
    }
}