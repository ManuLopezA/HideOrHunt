using Unity.Mathematics;
using UnityEngine;

public class Diamond : MonoBehaviour
{
      [SerializeField] private float velocity = 0.05f;
      void Update()
      {
          transform.rotation *= quaternion.RotateY(velocity * Time.deltaTime);
      }
}
