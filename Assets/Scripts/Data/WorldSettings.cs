using UnityEditor.PackageManager;

[System.Serializable]
public class WorldSettings
{
    public WorldResolution worldResolution;

    public enum WorldResolution
    {
        Low, // ChunkSize | numPointsPerAxis = 64 | 32
        Medium, // ChunkSize = 64 | 64
        High,  // ChunkSize = 128 | numPointsPerAxis = 86
        Ultra  // ChunkSize = 128 | numPointsPerAxis = 128 might be unstable
    }

    public int chunkSize
    {
        get
        {
            return worldResolution switch
            {
                WorldResolution.Low => 64,
                WorldResolution.Medium => 64,
                WorldResolution.High => 64,
                WorldResolution.Ultra => 64,
                _ => -1 // tell the user that something is wrong
            };
        }
    }

    public int numPointsPerAxis
    {
        get
        {
            return worldResolution switch
            {
                WorldResolution.Low => 32,
                WorldResolution.Medium => 64,
                WorldResolution.High => 86,
                WorldResolution.Ultra => 128,
                _ => -1 // tell the user that something is wrong
            };
        }
    }
}