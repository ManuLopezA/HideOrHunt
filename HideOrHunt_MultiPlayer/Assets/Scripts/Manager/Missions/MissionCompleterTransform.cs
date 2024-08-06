using UnityEngine;

public class MissionCompleterTransform : MonoBehaviour, MissionCompleterInterface<SOMissionTransform>
{
    public SOMissionTransform mission { get; private set; }

    public void Init(SOMissionTransform newMission)
    {
        mission = newMission;
    }

    public void OnAfterTransform(GameObject prop)
    {
        if (mission == null) return;
        if (prop.name != mission.target.name) return;
        GetComponent<PlayerMissionController>().MissionCompleted();
    }

    public void FinishMission()
    {
        mission = default;
    }
}
