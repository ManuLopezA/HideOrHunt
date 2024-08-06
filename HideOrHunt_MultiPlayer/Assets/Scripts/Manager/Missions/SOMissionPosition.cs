using UnityEngine;

[CreateAssetMenu(fileName = "New Mission", menuName = "Missions/Position")]
public class SOMissionPosition : SOMission
{
    public Vector3 position;
    public float radius;
}