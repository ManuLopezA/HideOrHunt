using System;

[Serializable]
public struct PerlinSettings
{
    public float offsetX;
    public float offsetY;
    public int frequency;
    public int amplitude;
    public int octaves;
    public int lacunarity;
    public float persistence;
    public bool carve;
}
