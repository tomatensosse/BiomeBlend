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

    public int ResolutionToChunkSize
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

    public int ResolutionToNumPointsPerAxis
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

    public int ResolutionToMegaChunkSize
    {
        get
        {
            return worldResolution switch
            {
                WorldResolution.Low => 64,
                WorldResolution.Medium => 64,
                WorldResolution.High => 128,
                WorldResolution.Ultra => 128,
                _ => -1
            };
        }
    }

    public int ResolutionToCavernLevel
    {
        get
        {
            return worldResolution switch
            {
                WorldResolution.Low => 8,
                WorldResolution.Medium => 8,
                WorldResolution.High => 16,
                WorldResolution.Ultra => 16,
                _ => -1
            };
        }
    }

    public int ResolutionToSeaLevel
    {
        get
        {
            return worldResolution switch
            {
                WorldResolution.Low => 32,
                WorldResolution.Medium => 32,
                WorldResolution.High => 64,
                WorldResolution.Ultra => 64,
                _ => -1
            };
        }
    }

    public int ResolutionToSkyLevel
    {
        get
        {
            return worldResolution switch
            {
                WorldResolution.Low => 40,
                WorldResolution.Medium => 40,
                WorldResolution.High => 80,
                WorldResolution.Ultra => 80,
                _ => -1
            };
        }
    }
}