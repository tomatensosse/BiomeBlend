using UnityEngine;

[System.Serializable]
public struct WorldGenSettings
{
    // Values that will be used by generators (MeshGenerator, DensityNode, etc...)

    // Storing them in a struct so they can be accessed from anywhere +
    // multiplayer structure will be MUCH easier to implement

    public int chunkSize; // used for chunk generation and basically everything

    // used for the compute buffers in order to generate densities and meshes
    public int numPoints;
    public int numPointsPerAxis;
    public int numVoxelsPerAxis;
    public int numVoxels;
    public int numThreadsPerAxis;
    public int maxTriangleCount;
    public int boundsSize;
    public float pointSpacing;
    public Vector3Int worldSize;

    public int threadGroupSize; // used to determine compute buffer group size

    public int megaChunkSize; // used for mega biome generation
    public int cavernLevel;
    public int seaLevel; // i.e. ground level
    public int skyLevel;
}