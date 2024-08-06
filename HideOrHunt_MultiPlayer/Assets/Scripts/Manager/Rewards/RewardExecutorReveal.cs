using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;

public class RewardExecutorReveal : NetworkBehaviour, RewardExecutorInterface
{
    public Dictionary<long, SOReward> rewards { get; private set; }

    public void Awake()
    {
        rewards = new Dictionary<long, SOReward>();
    }

    public void Init(SOReward reward, long key)
    {
        Assert.IsTrue(reward is SORewardReveal);
        rewards.Add(key, reward);
        SORewardReveal reveal = (SORewardReveal)reward;

        string teamName = reveal.teamToReveal switch
        {
            Role.HUNTER => "Hunters",
            Role.HIDER => "Hiders",
            _ => throw new System.NotImplementedException()
        };

        if (reveal.wholeTeam)
        {

            LogManager.Instance.LogAllRpc("All " + teamName + " have been revealed", LogType.INFO);
            List<PlayerController> team = reveal.teamToReveal switch
            {
                Role.HIDER => MatchManager.Instance.hiders,
                Role.HUNTER => MatchManager.Instance.hunters,
                _ => throw new System.NotImplementedException()
            };
            foreach (PlayerController player in team)
            {
                player.GetComponent<RewardExecutorReveal>().ShowMinimapDotRpc();
            }
        }
        else
        {
            LogManager.Instance.LogAllRpc("All " + teamName + " have been revealed", LogType.INFO);
            PlayerController player = reveal.teamToReveal switch
            {
                Role.HIDER => GetComponent<PlayerController>(),
                Role.HUNTER => MatchManager.Instance.hunters[Random.Range(0, MatchManager.Instance.hunters.Count)],
                _ => throw new System.NotImplementedException()
            };
            player.GetComponent<RewardExecutorReveal>().ShowMinimapDotRpc();
        }
    }

    private int activeRewards;

    [Rpc(SendTo.Everyone)]
    private void ShowMinimapDotRpc()
    {
        if (GetComponent<PlayerController>().isDummy || IsOwner) return;
        activeRewards++;
        GetComponent<PlayerController>().minimapDot.SetActive(true);
    }

    [Rpc(SendTo.Everyone)]
    public void HideMinimapDotRpc()
    {
        if (GetComponent<PlayerController>().isDummy || IsOwner) return;
        activeRewards--;
        if (activeRewards > 0) return;
        activeRewards = 0;
        GetComponent<PlayerController>().minimapDot.SetActive(false);
    }

    public void FinishAllRewards()
    {
    }

    public void FinishReward(long key)
    {
        SORewardReveal reveal = (SORewardReveal)rewards[key];
        if (reveal.wholeTeam)
        {
            List<PlayerController> team = reveal.teamToReveal switch
            {
                Role.HIDER => MatchManager.Instance.hiders,
                Role.HUNTER => MatchManager.Instance.hunters,
                _ => throw new System.NotImplementedException()
            };
            foreach (PlayerController player in team)
            {
                player.GetComponent<RewardExecutorReveal>().HideMinimapDotRpc();
            }
        }
        else
        {
            PlayerController player = reveal.teamToReveal switch
            {
                Role.HIDER => GetComponent<PlayerController>(),
                Role.HUNTER => MatchManager.Instance.hunters[Random.Range(0, MatchManager.Instance.hunters.Count)],
                _ => throw new System.NotImplementedException()
            };
            player.GetComponent<RewardExecutorReveal>().HideMinimapDotRpc();
        }

        if (rewards.ContainsKey(key)) rewards.Remove(key);
    }
}
