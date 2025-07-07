using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Biome", menuName = "BiomeBlend/Biome", order = 1)]
public class Biome : ScriptableObject
{
    [Header("Biome Settings")]
    public string uid;
    public string biomeName;
    public Color biomeColor = Color.white;
    public Material material;

    [SerializeReference]
    public BiomeShader biomeShader;
}
