using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MenuDummyAgent : MonoBehaviour
{

    private GameObject m_Target;

    [Header("Waypoints")]
    [SerializeField] private List<GameObject> m_Targets;


    private NavMeshAgent m_Agent;

    private void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        UpdateDestination();
    }

    private void Update()
    {
        CheckIfTouchingWaypoint();
    }

    private void CheckIfTouchingWaypoint()
    {
        if (Vector3.Distance(transform.position, m_Target.transform.position) < 1)
            UpdateDestination();
    }

    private void UpdateDestination()
    {
        m_Target = m_Targets[RandomWaypointIndex()];
        m_Agent.SetDestination(m_Target.transform.position);
    }

    private int RandomWaypointIndex()
    {
        return Random.Range(0, m_Targets.Count);
    }

}
