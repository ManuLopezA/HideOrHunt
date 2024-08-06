using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class RewardExecutorWeakness : MonoBehaviour, RewardExecutorInterface
{
    public Dictionary<long, SOReward> rewards { get; private set; }

    public void Awake()
    {
        rewards = new Dictionary<long, SOReward>();
    }

    public void Init(SOReward reward, long key)
    {
        rewards.Add(key, reward);
        Debug.Log("Init");
        // ienumerator = ShakePlayer(reward.duration, ((SORewardShake)reward).interval);
        // coroutine = 
        Assert.IsTrue(reward is SORewardWeakness);
        //StartCoroutine(ApplyWeakness((SORewardWeakness)reward));
    }

    public float OnBeforeReceiveDamage(float damage)
    {
        foreach (var so in rewards.Select(reward => reward.Value))
        {
            Assert.IsTrue(so is SORewardWeakness);
            damage *= ((SORewardWeakness)so).multiplier;
        }
        return damage;
    }

    public void FinishReward(long key)
    {
        if (rewards.ContainsKey(key)) rewards.Remove(key);
    }

    public void FinishAllRewards()
    {

    }

}
