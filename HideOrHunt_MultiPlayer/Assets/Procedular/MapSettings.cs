using System;
using UnityEngine.UI;

[Serializable]
public struct MapSettings
{
    public string name;
    public Terrain terrain;
    public Image wall;
    public int width;
    public int height;
    public PerlinSettings biomePerlin;
    public PerlinSettings propsPerlin;
    public Biome[] biomes;

    public Biome getBiome(float value)
    {
        foreach (var biome in biomes)
        {
            if (value <= biome.floor)
            {
                return biome;
            }
        }

        return biomes[^1];
    }
}
