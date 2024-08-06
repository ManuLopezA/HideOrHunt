using System;
using UnityEngine;
using UnityEngine.AI;

public class SelectionMove : MonoBehaviour
{
    [SerializeField] private Transform[] positions;
    [SerializeField] private int index;


    public void GoNextPosition()
    {
        index++;
        if (index > positions.Length-1) index = 0;
        GetComponent<NavMeshAgent>().SetDestination(positions[index].position);
    }

    public void GoPrevPosition()
    {
        index--;
        if (index < 0) index = positions.Length - 1;
        GetComponent<NavMeshAgent>().SetDestination(positions[index].position);
    }
}