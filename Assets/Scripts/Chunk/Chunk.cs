using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Chunk : MonoBehaviour
{
    [Header("Chunk Parameters")]
    public Vector3Int chunkPosition;

    public float[,,] DensityValues => densityValues;
    protected float[,,] densityValues;

    protected ComputeBuffer densityBuffer;

    protected bool isDensityGenerated = false;
    protected bool isMeshGenerated = false;

    protected MeshFilter meshFilter;
    protected MeshRenderer meshRenderer;
    protected MeshCollider meshCollider;

    //private int numPointsPerAxis => World.Data.worldGenSettings.numPointsPerAxis;

    private int numPointsPerAxis = DemoGenerator.Instance.numPointsPerAxis;

    void OnDestroy()
    {
        if (densityBuffer != null) densityBuffer.Release();
    }

    public virtual void Initialize()
    {
        meshFilter = this.AddComponent<MeshFilter>();
        meshRenderer = this.AddComponent<MeshRenderer>();
        meshCollider = this.AddComponent<MeshCollider>();
    }

    public abstract void GenerateDensity(bool showDensities = false);
    /* // Dont forget to release the density/points buffer for memory leaks
    {
        densityBuffer = biome.biomeShader.GenerateDensity(transform.position);

        SaveDensities(densityBuffer);

        if (showDensities)
        {
            DensityDisplayer.Instance.DisplayDensities(this, densityValues);
        }

        isDensityGenerated = true;
    }
    */

    public abstract void GenerateMesh();
    /*
    {
        Mesh mesh;

        mesh = MeshGenerator.Instance.GenerateMesh(densityBuffer, 1);

        meshFilter.mesh = mesh;
        meshFilter.sharedMesh = mesh;
        meshRenderer.material = biome.material;
        meshRenderer.sharedMaterial = biome.material;

        // Only set collider if mesh is valid
        if (mesh.vertexCount >= 3)
        {
            meshCollider.sharedMesh = mesh;
        }

        densityBuffer.Release();

        isMeshGenerated = true;
    }
    */

    protected void SaveDensities(ComputeBuffer densityBuffer)
    {
        int n = numPointsPerAxis;
        densityValues = new float[n, n, n];

        Vector4[] flat = new Vector4[n * n * n];

        Debug.Log("Count: " + densityBuffer.count);
        Debug.Log("Stride: " + densityBuffer.stride);
        Debug.Log("Flat Length: " + flat.Length);

        densityBuffer.GetData(flat);

        for (int x = 0; x < n; x++)
        {
            for (int y = 0; y < n; y++)
            {
                for (int z = 0; z < n; z++)
                {
                    densityValues[x, y, z] = flat[x + y * n + z * n * n].w; // Assuming the density is stored in the w component
                }
            }
        }
    }
}