using System.Collections.Generic;

[System.Serializable]
public class WorldBiomes
{
    public List<string> biomes = new List<string>(); // -2 is empty, -1 is blend chunk
    public List<string> megaBiomes = new List<string>();
}