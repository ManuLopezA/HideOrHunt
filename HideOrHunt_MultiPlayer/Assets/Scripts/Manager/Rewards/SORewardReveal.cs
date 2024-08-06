using UnityEngine;

[CreateAssetMenu(fileName = "New Reward", menuName = "Rewards/Reveal")]
public class SORewardReveal : SOReward
{
    public Role teamToReveal;
    public bool wholeTeam;
}