using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class RewardExecutorHp : MonoBehaviour, RewardExecutorInterface
{
    public Dictionary<long, SOReward> rewards { get; private set; }

    public void Awake()
    {
        rewards = new Dictionary<long, SOReward>();
    }

    public void Init(SOReward reward, long key)
    {
        Assert.IsTrue(reward is SORewardHp);
        GetComponent<HealthController>().ChangeHealth(((SORewardHp)reward).hp);
        rewards.Add(key, reward);
    }

    public void FinishReward(long key)
    {
        if (rewards.ContainsKey(key)) rewards.Remove(key);
    }

    public void FinishAllRewards()
    {
    }
}