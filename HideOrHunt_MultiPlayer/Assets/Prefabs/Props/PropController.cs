using UnityEngine;

public class PropController : MonoBehaviour
{
    [SerializeField] private PropSize size;
    [SerializeField] private GameObject sprite;

    public PropSize Size => size;
    public GameObject Sprite => sprite;
}