using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class RewardExecutorShake : MonoBehaviour, RewardExecutorInterface
{
    public Dictionary<long, SOReward> rewards { get; private set; }
    // public Coroutine coroutine = null;
    // public IEnumerator ienumerator;
    
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
        Assert.IsTrue(reward is SORewardShake);
        StartCoroutine(ShakePlayer((SORewardShake)reward));
    }

    IEnumerator ShakePlayer(SORewardShake reward)
    {
        float elapsedTime = 0;
        while (elapsedTime <= reward.duration)
        {
            float xShake = Random.Range(-1, 2) * Random.Range(1, reward.intensity);
            float zShake = Random.Range(-1, 2) * Random.Range(1, reward.intensity);

            GetComponent<Rigidbody>().velocity += new Vector3(xShake, 1, zShake);
            elapsedTime += reward.interval;
            yield return (new WaitForSeconds(reward.interval));
        }
    }
    public void FinishAllRewards()
    {
      
    }

    public void FinishReward(long key)
    {
        if (rewards.ContainsKey(key)) rewards.Remove(key);
    }
}
