using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class RewardExecutorFreeze : MonoBehaviour, RewardExecutorInterface
{
    public Dictionary<long, SOReward> rewards { get; private set; }

    public void Awake()
    {
        rewards = new Dictionary<long, SOReward>();
    }

    public void Init(SOReward reward, long key)
    {
        rewards.Add(key, reward);
    }

    public bool HasFreeze(PlayerRewardController shooter)
    {
        foreach (var so in rewards.Select(reward => reward.Value))
        {
            Assert.IsTrue(so is SORewardFreeze);
            SOReward freezeReward = null;
            int freezeIndex = 0;
            bool isGood = false;
            while (!isGood)
            {
                (freezeReward, freezeIndex) = MissionManager.Instance.GetNewCooldown();
                if (freezeReward is not SORewardSpeed speedSO) continue;
                if (speedSO.speedMultiplier != 0) continue;
                isGood = true;
            }

            shooter.AssignReward(freezeReward, freezeIndex, false);
            return true;
        }

        return false;
    }

    public void FinishReward(long key)
    {
        if (rewards.ContainsKey(key)) rewards.Remove(key);
    }

    public void FinishAllRewards()
    {
    }
}
