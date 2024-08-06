using UnityEngine;

public class SelectionMapManager : MonoBehaviour
{
    [SerializeField] private GameObject[] maps;
    [SerializeField] private Transform[] points;
    private int index = 0;

    public int Index => index;
    private int moveCompleteCount = 0;
    private bool isActive = false;

    private void Awake()
    {
        foreach (var map in maps)
        {
            map.GetComponent<MapMiniature>().OnMoveComplete += AllMapsStopped;
        }
    }

    public Transform[] Points()
    {
        return points;
    }

    public void NextSelection()
    {
        if (isActive) return;
        isActive = true;
        index++;
        if (index > maps.Length - 1) index = 0;
        foreach (var map in maps)
        {
            map.GetComponent<MapMiniature>().GoNextPosition();
        }
    }

    public void PrevSelection()
    {
        if (isActive) return;
        isActive = true;
        index--;
        if (index < 0) index = maps.Length - 1;
        foreach (var map in maps)
        {
            map.GetComponent<MapMiniature>().GoPrevPosition();
        }
    }
    private void AllMapsStopped()
    {
        moveCompleteCount++;
        if (moveCompleteCount >= maps.Length)
        {
            moveCompleteCount = 0;
            isActive = false;
        }
    }

}