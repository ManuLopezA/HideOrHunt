using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class RewardExecutorSpeed : MonoBehaviour, RewardExecutorInterface
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


    public float OnBeforeMove(float speed)
    {
        foreach (var so in rewards.Select(reward => reward.Value))
        {
            Assert.IsTrue(so is SORewardSpeed);
            speed *= ((SORewardSpeed)so).speedMultiplier;
        }
        return speed;
    }

    public void FinishReward(long key)
    {
        if (rewards.ContainsKey(key)) rewards.Remove(key);
        GetComponent<PlayerController>().RecalculateSpeed();
    }

    public void FinishAllRewards()
    {
        GetComponent<PlayerController>().RecalculateSpeed();
    }
}
