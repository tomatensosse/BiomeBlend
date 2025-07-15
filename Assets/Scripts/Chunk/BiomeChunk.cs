using UnityEngine;

public class BiomeChunk : Chunk
{
    public Biome biome;

    //private int chunkSize => World.Data.worldGenSettings.chunkSize;

    private int chunkSize => DemoGenerator.Instance.chunkSize;

    public override void GenerateDensity(bool showDensities = false)
    {
        densityBuffer = biome.biomeShader.GenerateDensity(transform.position);

        SaveDensities(densityBuffer);

        if (showDensities)
        {
            DensityDisplayer.Instance.DisplayDensities(this, densityValues);
        }

        isDensityGenerated = true;
    }

    public override void GenerateMesh()
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

        Gizmos.DrawCube(transform.position, Vector3.one * (chunkSize - 2));
    }
}