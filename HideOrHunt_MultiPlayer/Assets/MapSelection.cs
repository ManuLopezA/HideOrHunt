using UnityEngine;

public class MapSelection : MonoBehaviour
{
    [SerializeField] private GameObject[] agents;
    private int index = 0;

    public void NextSelection()
    {
        index++;
        if (index > agents.Length - 1) index = 0;
        foreach (var agent in agents)
        {
            agent.GetComponent<SelectionMove>().GoNextPosition();
        }
    }

    public void PrevSelection()
    {
        index--;
        if (index < 0) index = agents.Length - 1;
        foreach (var agent in agents)
        {
            agent.GetComponent<SelectionMove>().GoPrevPosition();
        }
    }

    // private void ChangeMapSelection(operand op)
    // {
    //     switch (op)
    //     {
    //         case operand.PLUS:
    //             for (int i = 0; i < maps.Length; i++)
    //             {
    //                 int newPos = index + i >= waypoints.Count ? 0 : index + 1;
    //                 Debug.Log($"{maps[i].name} to position[{newPos}]");
    //                 maps[i].SetDestination(waypoints[newPos].position);
    //             }
    //
    //             break;
    //         case operand.MINUS:
    //             for (int i = 0; i < maps.Length; i++)
    //             {
    //                 int newPos = i - 1 <= 0 ? 2 : i - 1;
    //                 Debug.Log(maps[i].name);
    //                 maps[i].SetDestination(waypoints[newPos].position);
    //             }
    //             break;
    //     }
    //
    //     // MAS              MENOS
    //     //----------------------------
    //     // 0 -> 1           0 -> 2
    //     // 1 -> 2           1 -> 0
    //     // 2 -> 0           2 -> 1
    // }
}