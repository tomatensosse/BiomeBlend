using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    [Header("Chunk Parameters")]
    public Vector3Int chunkPosition;
    public Biome biome;

    public float[,,] DensityValues => densityValues;
    protected float[,,] densityValues;

    protected ComputeBuffer densityBuffer;

    public bool isDensityGenerated = false;
    public bool isMeshGenerated = false;

    [Header("Components")]
    [HideInInspector] public MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider meshCollider;

    void OnDestroy()
    {
        if (densityBuffer != null) densityBuffer.Release();
    }

    public void Initialize()
    {
        meshFilter = this.AddComponent<MeshFilter>();
        meshRenderer = this.AddComponent<MeshRenderer>();
        meshCollider = this.AddComponent<MeshCollider>();
    }

    public virtual void GenerateDensity(bool saveDensities = true, bool showDensities = false) // Dont forget to release the density/points buffer for memory leaks
    {
        densityBuffer = biome.biomeShader.GenerateDensity(transform.position);

        if (saveDensities)
        {
            SaveDensities(densityBuffer);
        }

        if (saveDensities && showDensities)
        {
            DensityDisplayer.Instance.DisplayDensities(this, densityValues);
        }

        isDensityGenerated = true;
    }

    public void GenerateMesh()
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

    [Button("Log Density Values")]
    public void LogDensities()
    {
        if (densityValues == null || densityValues.Length == 0)
        {
            Debug.LogWarning("Density values are not generated or empty.");
            return;
        }

        string log = "Density Values for Chunk at " + chunkPosition + ":\n";

        for (int x = 0; x < World.Settings.numPointsPerAxis; x++)
        {
            for (int y = 0; y < World.Settings.numPointsPerAxis; y++)
            {
                for (int z = 0; z < World.Settings.numPointsPerAxis; z++)
                {
                    float densityRounded = Mathf.Round(densityValues[x, y, z] * 10f) / 10f; // Round to 2 decimal places
                    log += $"({densityRounded}) ";
                }
            }
        }

        Debug.Log(log);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (biome != null)
        {
            Gizmos.color = biome.biomeColor;
        }

        Gizmos.DrawWireCube(transform.position, new Vector3(World.Settings.chunkSize, World.Settings.chunkSize, World.Settings.chunkSize));
    }
    
    private void SaveDensities(ComputeBuffer densityBuffer)
    {
        int n = World.Settings.numPointsPerAxis;
        densityValues = new float[n, n, n];

        Vector4[] flat = new Vector4[n * n * n];
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