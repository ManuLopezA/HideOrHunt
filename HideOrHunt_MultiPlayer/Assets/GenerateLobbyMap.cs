using UnityEngine;

public class GenerateLobbyMap : MonoBehaviour
{
    public GameObject cubePrefab; 

    [SerializeField] private int width = 40;  
    [SerializeField] private int depth = 25;  

    void Start()
    {
        GenerateMap();
        GenerateBorder();
        GenerateOuterBorder();
    }

    void GenerateMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                Vector3 position = new Vector3(x, 0, z);
                Instantiate(cubePrefab, position, Quaternion.identity);
            }
        }
    }

    void GenerateBorder()
    {
        // Borde superior e inferior
        for (int x = -1; x <= width; x++)
        {
            Vector3 topPosition = new Vector3(x, -1, -1);
            Vector3 bottomPosition = new Vector3(x, -1, depth);
            Instantiate(cubePrefab, topPosition, Quaternion.identity);
            Instantiate(cubePrefab, bottomPosition, Quaternion.identity);
        }

        // Borde izquierdo y derecho
        for (int z = 0; z < depth; z++)
        {
            Vector3 leftPosition = new Vector3(-1, -1, z);
            Vector3 rightPosition = new Vector3(width, -1, z);
            Instantiate(cubePrefab, leftPosition, Quaternion.identity);
            Instantiate(cubePrefab, rightPosition, Quaternion.identity);
        }
    }

    void GenerateOuterBorder()
    {
        // Borde superior e inferior del borde exterior
        for (int x = -2; x <= width + 1; x++)
        {
            Vector3 topPosition = new Vector3(x, -2, -2);
            Vector3 bottomPosition = new Vector3(x, -2, depth + 1);
            Instantiate(cubePrefab, topPosition, Quaternion.identity);
            Instantiate(cubePrefab, bottomPosition, Quaternion.identity);
        }

        // Borde izquierdo y derecho del borde exterior
        for (int z = -1; z <= depth; z++)
        {
            Vector3 leftPosition = new Vector3(-2, -2, z);
            Vector3 rightPosition = new Vector3(width + 1, -2, z);
            Instantiate(cubePrefab, leftPosition, Quaternion.identity);
            Instantiate(cubePrefab, rightPosition, Quaternion.identity);
        }
    }
}