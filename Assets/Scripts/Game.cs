using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Instance { get; private set; }
    
    public static string Version => Instance.version;
    public static List<Biome> Biomes => Instance._biomes;
    public static List<MegaBiome> MegaBiomes => Instance._megaBiomes;

    [SerializeField] private string version = "0.a"; // alpha
    [SerializeField] private List<Biome> _biomes = new List<Biome>();
    [SerializeField] private List<MegaBiome> _megaBiomes = new List<MegaBiome>();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
}