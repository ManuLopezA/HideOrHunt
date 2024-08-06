using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(RewardExecutorSpeed))]
[RequireComponent(typeof(RewardExecutorHp))]
[RequireComponent(typeof(RewardExecutorDestransform))]
[RequireComponent(typeof(RewardExecutorFreeze))]
[RequireComponent(typeof(RewardExecutorShake))]
[RequireComponent(typeof(RewardExecutorWeakness))]
[RequireComponent(typeof(RewardExecutorClone))]
[RequireComponent(typeof(RewardExecutorReveal))]
public class PlayerRewardController : NetworkBehaviour
{
    [SerializeField] private GameObject rewardGuiPrefab;
    private RewardExecutorSpeed rewardExecutorSpeed;
    private RewardExecutorHp rewardExecutorHp;
    private RewardExecutorDestransform rewardExecutorDestransform;
    private RewardExecutorFreeze rewardExecutorFreeze;
    private RewardExecutorShake rewardExecutorShake;
    private RewardExecutorWeakness rewardExecutorWeakness;
    private RewardExecutorClone rewardExecutorClone;
    private RewardExecutorReveal rewardExecutorReveal;

    private Dictionary<long, (SOReward, TextMeshProUGUI)> rewards = new();

    private void Awake()
    {
        rewardExecutorSpeed = GetComponent<RewardExecutorSpeed>();
        rewardExecutorHp = GetComponent<RewardExecutorHp>();
        rewardExecutorDestransform = GetComponent<RewardExecutorDestransform>();
        rewardExecutorFreeze = GetComponent<RewardExecutorFreeze>();
        rewardExecutorShake = GetComponent<RewardExecutorShake>();
        rewardExecutorReveal = GetComponent<RewardExecutorReveal>();
        rewardExecutorWeakness = GetComponent<RewardExecutorWeakness>();
        rewardExecutorClone = GetComponent<RewardExecutorClone>();
    }

    public void EndRewards()
    {
        StopAllCoroutines();
        rewards.Clear();
        rewardExecutorHp.rewards.Clear();
        rewardExecutorHp.StopAllCoroutines();
        rewardExecutorSpeed.rewards.Clear();
        rewardExecutorSpeed.StopAllCoroutines();
        rewardExecutorDestransform.rewards.Clear();
        rewardExecutorDestransform.StopAllCoroutines();
        rewardExecutorFreeze.rewards.Clear();
        rewardExecutorFreeze.StopAllCoroutines();
        rewardExecutorShake.rewards.Clear();
        rewardExecutorShake.StopAllCoroutines();
        rewardExecutorReveal.rewards.Clear();
        rewardExecutorReveal.StopAllCoroutines();
        EndRewardsClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void EndRewardsClientRpc()
    {
        try
        {
            foreach (var reward in rewards)
            {
                Destroy(reward.Value.Item2.gameObject);
            }
        }
        catch (Exception)
        {
            // ignored
        }

        rewards.Clear();
    }

    public void AskForCooldown()
    {
        var (newCooldown, index) = MissionManager.Instance.GetNewCooldown();
        AssignReward(newCooldown, index, false);
    }

    public void AskForPowerup()
    {
        var (newPowerup, index) = MissionManager.Instance.GetNewPowerup();
        AssignReward(newPowerup, index, true);
    }

    public void AssignReward(SOReward reward, int index, bool good)
    {
        if (GetComponent<HealthController>().isDead) return;
        long key = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        rewards.TryAdd(key, (reward, null));
        AddSOClientRpc(key, index, good);
        switch (reward)
        {
            case SORewardSpeed soRewardSpeed:
                rewardExecutorSpeed.Init(soRewardSpeed, key);
                StartCoroutine(Countdown(soRewardSpeed, key, rewardExecutorSpeed));
                break;
            case SORewardHp soRewardHp:
                rewardExecutorHp.Init(soRewardHp, key);
                StartCoroutine(Countdown(soRewardHp, key, rewardExecutorHp));
                break;
            case SORewardDestransform soRewardDestransform:
                rewardExecutorDestransform.Init(soRewardDestransform, key);
                StartCoroutine(Countdown(soRewardDestransform, key, rewardExecutorDestransform));
                break;
            case SORewardFreeze soRewardFreeze:
                rewardExecutorFreeze.Init(soRewardFreeze, key);
                StartCoroutine(Countdown(soRewardFreeze, key, rewardExecutorFreeze));
                break;
            case SORewardShake soRewardShake:
                rewardExecutorShake.Init(soRewardShake, key);
                StartCoroutine(Countdown(soRewardShake, key, rewardExecutorShake));
                break;
            case SORewardReveal soRewardReveal:
                rewardExecutorReveal.Init(soRewardReveal, key);
                StartCoroutine(Countdown(soRewardReveal, key, rewardExecutorReveal));
                break;
            case SORewardWeakness soRewardWeakness:
                rewardExecutorWeakness.Init(soRewardWeakness, key);
                StartCoroutine(Countdown(soRewardWeakness, key, rewardExecutorWeakness));
                break;
            case SORewardClone soRewardClone:
                rewardExecutorClone.Init(soRewardClone, key);
                StartCoroutine(Countdown(soRewardClone, key, rewardExecutorClone));
                break;
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void AddSOClientRpc(long key, int index, bool good)
    {
        if (!IsOwner) return;
        if (GetComponent<PlayerController>().isDummy) return;
        SOReward reward = good switch
        {
            true => MissionManager.Instance.GetPowerup(index),
            false => MissionManager.Instance.GetCooldown(index)
        };

        GameObject rewardGui = Instantiate(rewardGuiPrefab, GUIManager.Instance.rewardList.transform);
        if (rewards.TryAdd(key, (reward, rewardGui.GetComponent<TextMeshProUGUI>()))) return;
        rewards.Remove(key);
        rewards.Add(key, (reward, rewardGui.GetComponent<TextMeshProUGUI>()));
    }

    private IEnumerator Countdown(SOReward reward, long key, RewardExecutorInterface executor)
    {
        int elapsed = 0;
        while (elapsed < reward.duration)
        {
            elapsed++;
            UpdateTimeClientRpc(key, reward.duration - elapsed);
            yield return new WaitForSeconds(1);
        }

        RemoveRewardServer(key, executor);
        RemoveRewardClientRpc(key);
    }


    [Rpc(SendTo.ClientsAndHost)]
    private void UpdateTimeClientRpc(long key, int time)
    {
        if (!rewards.TryGetValue(key, out var reward)) return;
        var (so, rewardGui) = reward;
        if (rewardGui == null) return;
        int min = time / 60;
        int sec = time % 60;
        rewardGui.text = $"{min:00}:{sec:00} - {so.title}";
    }

    private void RemoveRewardServer(long key, RewardExecutorInterface executor)
    {
        var (_, rewardGui) = rewards[key];
        if (rewardGui != null) Destroy(rewardGui.gameObject);
        rewards.Remove(key);
        executor.FinishReward(key);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void RemoveRewardClientRpc(long key)
    {
        if (!rewards.TryGetValue(key, out var reward)) return;
        var (_, rewardGui) = reward;
        if (rewardGui != null) Destroy(rewardGui.gameObject);
        rewards.Remove(key);
    }
}
