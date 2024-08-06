using UnityEngine;

[CreateAssetMenu(fileName = "New Mission", menuName = "Missions/Collect")]
public class SOMissionCollect : SOMission
{
    public GameObject prefabToCollect;
    public Vector2[] positions;
}
