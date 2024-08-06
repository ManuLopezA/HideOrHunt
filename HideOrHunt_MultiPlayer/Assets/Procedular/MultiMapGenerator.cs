using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class MultiGenerateMap : NetworkBehaviour
{
    public Terrain terrain = Terrain.MOUNTAIN;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject floor;
    [SerializeField] private GameObject walls;
    [SerializeField] private GameObject decorations;
    [SerializeField] private GameObject props;
    [SerializeField] private GameObject quadFloor;

    [FormerlySerializedAs("MapsSettings")] [SerializeField]
    private MapSettings[] mapsSettings;

    public static MultiGenerateMap Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (IsServer || IsClient) return;
        Debug.Log("???");
        GenerateMap();
    }

    public override void OnNetworkSpawn()
    {
        terrain = GameManager.Instance.selectedTerrain.Value;
        if (!IsServer) return;
        GenerateMap();
    }

    public void GenerateMap()
    {
        foreach (Transform child in floor.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in walls.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in decorations.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in props.transform)
        {
            Destroy(child.gameObject);
        }

        MapSettings chosen = default;
        for (var i = 0; i < mapsSettings.Length; i++)
        {
            if (mapsSettings[i].terrain != terrain) continue;
            chosen = mapsSettings[i];
            break;
        }

        GenerateQuadFloor(chosen.height, chosen.width);

        float[,] biomePerlin = NewPerlinMap(chosen.width, chosen.height, chosen.biomePerlin);
        float[,] propsPerlin = NewPerlinMap(chosen.width, chosen.height, chosen.propsPerlin);

        GenerateBiomes(chosen, biomePerlin);
        GenerateProps(chosen, biomePerlin, propsPerlin);
        GenerateWalls(chosen);
    }

    private void GenerateQuadFloor(float height, float width)
    {
        float quadHeight = height / 2;
        float quadWidth = width / 2;
        GameObject quad = Instantiate(quadFloor);
        Vector3 quadCenter = new Vector3(quadHeight - 1, 0f, quadWidth - 1);
        
        quadFloor.transform.position = quadCenter;
        quadFloor.transform.localScale = new Vector3(height*2, width*2, 1f);
        
        quad.GetComponent<NetworkObject>().Spawn();
    }

    private void GenerateBiomes(MapSettings conf, float[,] biomePerlin)
    {
        for (int x = 0; x < conf.width; x++)
        {
            for (int y = 0; y < conf.height; y++)
            {
                Biome biome = conf.getBiome(biomePerlin[x, y]);
                if (!biome.cube) continue;
                GameObject cube = Instantiate(biome.cube);
                cube.name = $"Cube {x}:{y}";
                cube.transform.position = new Vector3(x, -0.5f, y);
                if (!IsServer) continue;
                cube.GetComponent<NetworkObject>().Spawn();
                cube.GetComponent<NetworkObject>().ActiveSceneSynchronization = true;
                cube.GetComponent<NetworkObject>().DestroyWithScene= true;
            }
        }
    }

    private void GenerateProps(MapSettings conf, float[,] biomePerlin, float[,] propsPerlin)
    {
        for (int x = 0; x < conf.width; x++)
        {
            for (int y = 0; y < conf.height; y++)
            {
                Biome biome = conf.getBiome(biomePerlin[x, y]);
                GameObject item = biome.getProp(propsPerlin[x, y]);
                if (item == null) continue;
                item = Instantiate(item);
                float x_offset = Random.Range(-0.5f, 0.5f);
                float y_offset = Random.Range(-0.5f, 0.5f);
                item.name = biome.getProp(propsPerlin[x, y]).name;
                item.transform.position = new Vector3(x + x_offset, 0, y + y_offset);
                if (!IsServer) continue;
                item.GetComponent<NetworkObject>().Spawn();
                item.GetComponent<NetworkObject>().ActiveSceneSynchronization = true;
                item.GetComponent<NetworkObject>().DestroyWithScene= true;
            }
        }
    }


    private void GenerateWalls(MapSettings map)
    {
        const int wall_height = 25;
        PlaceWall("Wall 1", new Vector3(map.width / 2f - 0.5f, wall_height / 2, -0.5f),
            new Vector3(map.width, wall_height, map.width), 180);
        PlaceWall("Wall 2", new Vector3(map.width / 2f - 0.5f, wall_height / 2, map.height - 0.5f),
            new Vector3(map.width, wall_height, map.width), 0);
        PlaceWall("Wall 3", new Vector3(-0.5f, wall_height / 2, map.height / 2f - 0.5f),
            new Vector3(map.height, wall_height, map.height), -90);
        PlaceWall("Wall 4", new Vector3(map.width - 0.5f, wall_height / 2, map.height / 2f - 0.5f),
            new Vector3(map.height, wall_height, map.height), 90);
    }

    private void PlaceWall(string wallName, Vector3 position, Vector3 scale, float rotation)
    {
        GameObject wall = Instantiate(wallPrefab, walls.transform);
        wall.name = wallName;
        wall.transform.localScale = scale;
        wall.transform.position = position;
        wall.transform.rotation = Quaternion.Euler(0, rotation, 0);
    }


    private float[,] NewPerlinMap(int width, int height, PerlinSettings conf)
    {
        float[,] perlin = new float[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float perlinNoise = PerlinUtils.CalculatePerlinNoise(x, y, conf.frequency, conf.offsetX, conf.offsetY,
                    conf.octaves, conf.lacunarity, conf.persistence, conf.carve, false, true);
                perlinNoise = Mathf.Clamp01(perlinNoise);
                perlin[x, y] = perlinNoise;
            }
        }

        return perlin;
    }
}

// do this only if in unity editor
#if UNITY_EDITOR
[CustomEditor(typeof(MultiGenerateMap))]
public class MapSettingsButton : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        MultiGenerateMap map = (MultiGenerateMap)target;
        if (GUILayout.Button("Update Map"))
        {
            map.GenerateMap();
        }
    }
}
#endif