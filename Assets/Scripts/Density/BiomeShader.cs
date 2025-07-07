using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class BiomeShader
{
    public ComputeShader shader;
    public ComputeBuffer pointsBuffer;

    [Header("Base Parameters")]
    public Vector3 offset = Vector3.zero;
    public Vector4 parameters = new Vector4(1, 0, 0, 0);
    public int numOctaves = 2;

    protected List<ComputeBuffer> buffersToRelease = new List<ComputeBuffer>();

    protected ComputeBuffer offsetsBuffer;
    protected int numPointsPerAxis;
    protected int numThreadsPerAxis;
    protected int threadGroupSize;

    public void SetBaseParameters()
    {
        shader.SetFloat("boundsSize", World.WorldGenSettings.boundsSize);
        shader.SetFloat("spacing", World.WorldGenSettings.pointSpacing);
        shader.SetVector("worldSize", (Vector3)World.WorldGenSettings.worldSize);

        shader.SetVector("offset", offset);

        shader.SetVector("params", parameters);

        shader.SetInt("octaves", Mathf.Max(1, numOctaves));
    }

    public void SetDynamicParameters()
    {
        shader.SetBuffer(0, "points", pointsBuffer);
        shader.SetBuffer(0, "offsets", offsetsBuffer);
        shader.SetInt("numPointsPerAxis", numPointsPerAxis);
        shader.SetInt("numThreadsPerAxis", numThreadsPerAxis);
    }

    public void GenerateDynamicParameters()
    {
        numPointsPerAxis = World.WorldGenSettings.numPointsPerAxis;
        threadGroupSize = World.WorldGenSettings.threadGroupSize;
        numThreadsPerAxis = Mathf.CeilToInt(numPointsPerAxis / (float)threadGroupSize);

        var prng = new System.Random(World.Seed);
        var offsets = new Vector3[numOctaves];
        float offsetRange = 1000;
        for (int i = 0; i < numOctaves; i++)
        {
            offsets[i] = new Vector3((float)prng.NextDouble() * 2 - 1, (float)prng.NextDouble() * 2 - 1, (float)prng.NextDouble() * 2 - 1) * offsetRange;
        }

        var offsetsBuffer = new ComputeBuffer(offsets.Length, sizeof(float) * 3);
        offsetsBuffer.SetData(offsets);

        buffersToRelease.Add(offsetsBuffer);

        this.offsetsBuffer = offsetsBuffer;

        // POINTS BUFFER IS IMPORTANT ; RETURNS THE DENSITIES
        pointsBuffer = new ComputeBuffer(World.WorldGenSettings.numPointsPerAxis * World.WorldGenSettings.numPointsPerAxis * World.WorldGenSettings.numPointsPerAxis, sizeof(float) * 4);
    }

    public void Dispatch(Vector3 worldPositionOfChunk)
    {
        shader.SetVector("centre", worldPositionOfChunk);

        shader.Dispatch(0, numThreadsPerAxis, numThreadsPerAxis, numThreadsPerAxis);
    }

    public abstract ComputeBuffer GenerateDensity(Vector3 worldPositionOfChunk);
}