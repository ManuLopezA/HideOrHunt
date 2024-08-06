using UnityEngine;

[CreateAssetMenu(fileName = "New Mission", menuName = "Missions/Default")]
public class SOMission : ScriptableObject
{
    public string title;
    public string description;
    public int availableTimeSec;
    public int waitTimeSec;
    public Sprite image;
    public int progress = 1;
    public Difficulty difficulty;
}
