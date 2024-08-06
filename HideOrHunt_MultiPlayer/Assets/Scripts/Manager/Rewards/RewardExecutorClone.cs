using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class RewardExecutorClone : NetworkBehaviour, RewardExecutorInterface
{
    public Dictionary<long, SOReward> rewards { get; private set; }
    public GameObject playerPrefab;
    public PlayerController pc;
    public Rigidbody rb;

    public NetworkVariable<bool> isClone = new();
    public NetworkVariable<int> cloneDuration = new();
    public NetworkVariable<Vector3> cloneVector = new();

    private void Awake()
    {
        rewards = new Dictionary<long, SOReward>();
        pc = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
    }

    public void Init(SOReward reward, long key)
    {
        rewards.Add(key, reward);
    }

    public void SpawnClone(InputAction.CallbackContext _)
    {
        SpawnCloneRpc();
    }

    [Rpc(SendTo.Server)]
    public void SpawnCloneRpc()
    {
        var (canSpawn, duration) = CanSpawnClone();
        if (!canSpawn) return;

        float cloneSpeed = pc.speed;
        Vector3 cloneDirection = new Vector3(Random.Range(-1f, 2f), 0, Random.Range(-1f, 2f) * cloneSpeed);

        GameObject clone = Instantiate(playerPrefab);
        clone.name = "Clone";

        var clonePC = clone.GetComponent<PlayerController>();
        var cloneTC = clone.GetComponent<TransformController>();
        var cloneRE = clone.GetComponent<RewardExecutorClone>();
        var cloneRC = clone.GetComponent<RoleController>();
        var cloneNO = clone.GetComponent<NetworkObject>();

        clonePC.isDummy = true;
        clonePC.isClone = true;
        cloneRE.isClone.Value = true;

        cloneNO.Spawn();
        clonePC.SetNameRpc(pc._name.Value.ToString());
        clonePC._characterSelected.Value = pc._characterSelected.Value;

        cloneRE.cloneVector.Value = cloneDirection;
        cloneRE.cloneDuration.Value = duration;
        cloneRC.role.Value = Role.HIDER;

        if (cloneTC.isTransformed)
        {
            cloneTC.TransformRpc(cloneTC.PropName, cloneTC.Size);
        }
        else
        {
            cloneTC.CloneTransformSpriteRpc();
        }

        clone.transform.position = transform.position;
        Debug.Log("kys!!!!!!!!!!!! ;)");
        cloneRE.KMSRoutine();
        Debug.Log("now!!!!!!!!!!!! ;)");

    }

    private void Update()
    {
        if (!IsServer) return;
        if (!isClone.Value) return;
        rb.velocity = cloneVector.Value;
    }

    public void KMSRoutine()
    {
        StartCoroutine(KMSLater());
    }

    private IEnumerator KMSLater()
    {
        Debug.Log("kys plz! ;)");
        yield return new WaitForSeconds(cloneDuration.Value);
        KMS();
    }

    public void KMS()
    {
        GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }

    private (bool, int) CanSpawnClone()
    {
        long toRemove = 0;
        foreach (var reward in rewards)
        {
            var so = reward.Value;
            Assert.IsTrue(so is SORewardClone);
            toRemove = reward.Key;
        }
        if (toRemove == 0) return (false, 0);
        int duration = rewards[toRemove].duration;
        if (rewards.ContainsKey(toRemove)) rewards.Remove(toRemove);
        return (true, duration);
    }

    public void FinishReward(long key)
    {
        if (rewards.ContainsKey(key)) rewards.Remove(key);
    }

    public void FinishAllRewards()
    {
        rewards.Clear();
        StopAllCoroutines();
    }
}
