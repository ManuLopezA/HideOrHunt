using Unity.Mathematics;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private float velocity = 0.05f;
    void Update()
    {
        transform.rotation *= quaternion.RotateZ(velocity * Time.deltaTime);
    }
}
