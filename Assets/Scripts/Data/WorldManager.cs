using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance { get; private set; }

    const string worldDataPath = "/Worlds/";

    private readonly List<string> randWorldName1 = new List<string>
    {
        "Glorious",
        "Legendary",
        "Turbo",
        "Magnificent",
        "Corrupted",
        "Dark"
    };

    private readonly List<string> randWorldName2 = new List<string>
    {
        "World",
        "Realm",
        "Dimension",
        "Universe",
        "Kingdom",
        "Empire"
    };

    private readonly List<string> randWorldName3 = new List<string>
    {
        "of Wonders",
        "of Chaos",
        "of Dreams",
        "of Shadows",
        "of Light",
        "of Adventure"
    };

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void OnApplicationQuit()
    {
        if (World.Data != null)
        {
            SaveWorld(World.Data);
        }
    }

    [Button("Generate Random World")]
    public void GenerateWorld()
    {
        string worldName = GenerateRandomWorldName();
        string uid = worldName.Replace(" ", "_").ToLowerInvariant();
        string version = Game.Version;
        int seed = Random.Range(0, int.MaxValue);

        WorldBiomes worldBiomes = new WorldBiomes
        {
            biomes = new List<string>(),
            megaBiomes = new List<string>()
        };

        foreach (Biome biome in Game.Biomes)
        {
            worldBiomes.biomes.Add(biome.uid);
        }

        foreach (MegaBiome megaBiome in Game.MegaBiomes)
        {
            worldBiomes.megaBiomes.Add(megaBiome.uid);
        }

        WorldData data = new WorldData
        {
            worldName = worldName,
            uid = uid,
            version = version,
            seed = seed,
            worldSettings = new WorldSettings
            {
                worldResolution = WorldSettings.WorldResolution.Low
            },
            worldBiomes = worldBiomes
        };

        string json = JsonUtility.ToJson(data, true);
        
        string fullDirectoryPath = Application.persistentDataPath + worldDataPath;

        if (!System.IO.Directory.Exists(fullDirectoryPath))
        {
            System.IO.Directory.CreateDirectory(fullDirectoryPath);
            Debug.Log($"Created directory: {fullDirectoryPath}");
        }

        System.IO.File.WriteAllText(Application.persistentDataPath + worldDataPath + $"{uid}.json", json);
    }

    [Button("Load All World UIDs")]
    public void LoadAllWorldUIDs()
    {
        string fullDirectoryPath = Application.persistentDataPath + worldDataPath;

        if (!System.IO.Directory.Exists(fullDirectoryPath))
        {
            System.IO.Directory.CreateDirectory(fullDirectoryPath);
            Debug.Log($"No worlds found. Created directory: {fullDirectoryPath}");
            return;
        }

        string[] files = System.IO.Directory.GetFiles(Application.persistentDataPath + worldDataPath, "*.json");

        foreach (string file in files)
        {
            // Get the file name without the extension
            string uid = System.IO.Path.GetFileNameWithoutExtension(file);

            Debug.Log($"Found world UID: {uid}");
        }
    }

    [Button("Load World Data")]
    public void LoadWorld(string uid)
    {
        string filePath = Application.persistentDataPath + worldDataPath + $"{uid}.json";

        if (!System.IO.File.Exists(filePath))
        {
            Debug.LogError($"World data file not found: {filePath}");
            return;
        }

        string json = System.IO.File.ReadAllText(filePath);
        WorldData data = JsonUtility.FromJson<WorldData>(json);

        if (data == null)
        {
            Debug.LogError("Failed to load world data.");
            return;
        }

        if (data.version != Game.Version)
        {
            Debug.LogWarning($"World version {data.version} does not match game version {Game.Version}. Some features may not work as expected.");
        }

        World.Instance.LoadWorld(data);
    }

    public void SaveWorld(WorldData data)
    {
        string json = JsonUtility.ToJson(data, true);

        System.IO.File.WriteAllText(Application.persistentDataPath + worldDataPath + $"{data.uid}.json", json);

        Debug.Log("Saved World!");
    }

    private string GenerateRandomWorldName()
    {
        string namePart1 = randWorldName1[Random.Range(0, randWorldName1.Count)];
        string namePart2 = randWorldName2[Random.Range(0, randWorldName2.Count)];
        string namePart3 = randWorldName3[Random.Range(0, randWorldName3.Count)];

        return $"{namePart1} {namePart2} {namePart3}";
    }
}