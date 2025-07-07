using UnityEngine;

[System.Serializable]
public struct WorldGenSettings
{
    public int chunkSize;

    // Values that will be used by generators (MeshGenerator, DensityNode, etc...)

    // Storing them in a struct so they can be accessed from anywhere +
    // multiplayer structure will be MUCH easier to implement

    public int numPoints;
    public int numPointsPerAxis;
    public int numVoxelsPerAxis;
    public int numVoxels;
    public int numThreadsPerAxis;
    public int maxTriangleCount;
    public int boundsSize;
    public float pointSpacing;
    public Vector3Int worldSize;

    public int threadGroupSize;
}