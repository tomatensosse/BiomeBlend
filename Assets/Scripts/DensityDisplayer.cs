using UnityEngine;

public class DensityDisplayer : MonoBehaviour
{
    public static DensityDisplayer Instance { get; private set; }

    public GameObject densityPointPrefab;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (densityPointPrefab == null)
        {
            Debug.LogError("DensityDisplayer | densityPointPrefab is not assigned.");
        }
    }

    public void DisplayDensities(Chunk chunk, float[,,] densities)
    {
        Vector3 basePosition = chunk.transform.position - (Vector3.one * World.WorldGenSettings.chunkSize / 2f);
        float spacing = World.WorldGenSettings.pointSpacing;
        int numPointsPerAxis = World.WorldGenSettings.numPointsPerAxis;

        for (int x = 0; x < numPointsPerAxis; x++)
        {
            for (int y = 0; y < numPointsPerAxis; y++)
            {
                for (int z = 0; z < numPointsPerAxis; z++)
                {
                    float density = densities[x, y, z];

                    Vector3 pointPosition = basePosition + new Vector3(x, y, z) * spacing;

                    GameObject densityPointObject = Instantiate(densityPointPrefab, pointPosition, Quaternion.identity, chunk.transform);
                    DensityPoint densityPoint = densityPointObject.GetComponent<DensityPoint>();

                    densityPoint.SetDensity(new Vector3Int(x, y, z), density);
                }
            }
        }
    }
}
