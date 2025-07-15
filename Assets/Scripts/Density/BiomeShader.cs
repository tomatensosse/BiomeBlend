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

    /*
    private int seed => World.Data.seed;
    private int numPoints => World.Data.worldGenSettings.numPoints;
    private int boundsSize => World.Data.worldGenSettings.boundsSize;
    private float pointSpacing => World.Data.worldGenSettings.pointSpacing;
    private Vector3 worldSize => (Vector3)World.Data.worldGenSettings.worldSize;
    private int numPointsPerAxis = World.Data.worldGenSettings.numPointsPerAxis;
    private int numThreadsPerAxis => World.Data.worldGenSettings.numThreadsPerAxis;
    */

    private int seed => DemoGenerator.Instance.seed;
    private int numPoints => DemoGenerator.Instance.numPoints;
    private int boundsSize => DemoGenerator.Instance.chunkSize;
    private float pointSpacing => DemoGenerator.Instance.pointSpacing;
    private Vector3 worldSize => (Vector3)DemoGenerator.Instance.worldSize;
    private int numPointsPerAxis => DemoGenerator.Instance.numPointsPerAxis;
    private int numThreadsPerAxis => DemoGenerator.Instance.numThreadsPerAxisP;

    public void SetBaseParameters()
    {
        shader.SetFloat("boundsSize", boundsSize);
        shader.SetFloat("spacing", pointSpacing);
        shader.SetVector("worldSize", worldSize);

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
        var prng = new System.Random(seed);
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
        pointsBuffer = new ComputeBuffer(numPoints, sizeof(float) * 4);
    }

    public void Dispatch(Vector3 worldPositionOfChunk)
    {
        shader.SetVector("centre", worldPositionOfChunk);

        shader.Dispatch(0, numThreadsPerAxis, numThreadsPerAxis, numThreadsPerAxis);
    }

    public abstract ComputeBuffer GenerateDensity(Vector3 worldPositionOfChunk);
}