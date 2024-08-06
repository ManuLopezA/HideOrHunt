using System;
using UnityEngine;

[Serializable]
public struct Biome
{
    public string name;
    public GameObject cube;
    [Range(0, 1)] public float floor;
    public HeightProp[] props;

    public GameObject getProp(float value)
    {
        foreach (var prop in props)
        {
            if (value >= prop.min && value <= prop.max)
            {
                return prop.prop;
            }
        }

        return null;
    }
}

[Serializable]
public struct HeightProp
{
    public string name;
    public GameObject prop;
    [Range(0, 1)] public float min;
    [Range(0, 1)] public float max;
}
