using System.Collections.Generic;
using UnityEngine;

public class RandomMaterialCube : MonoBehaviour
{
  [SerializeField] private List<Material> materials;
  private void Awake()
  {
    int random = Random.Range(0, materials.Count);
    GetComponent<MeshRenderer>().material = materials[random];
  }
}
