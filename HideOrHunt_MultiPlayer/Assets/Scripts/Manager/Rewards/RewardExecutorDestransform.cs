using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class RewardExecutorDestransform : MonoBehaviour, RewardExecutorInterface
{
    public Dictionary<long, SOReward> rewards { get; private set; }

    public void Awake()
    {
        rewards = new Dictionary<long, SOReward>();
    }

    public void Init(SOReward reward, long key)
    {
        rewards.Add(key, reward);
        GetComponent<TransformController>().DestransformRpc();
    }


    public bool CanTransform()
    {
        foreach (var reward in rewards)
        {
            var so = reward.Value;
            Assert.IsTrue(so is SORewardDestransform);
            return false;
        }

        return true;
    }

    public void FinishReward(long key)
    {
        if (rewards.ContainsKey(key)) rewards.Remove(key);
    }

    public void FinishAllRewards()
    {
    }
}