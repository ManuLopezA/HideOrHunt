using System.Collections.Generic;

public interface RewardExecutorInterface
{
    public Dictionary<long, SOReward> rewards { get; }
    public void Init(SOReward reward, long key);
    public void FinishAllRewards();
    public void FinishReward(long key);
}