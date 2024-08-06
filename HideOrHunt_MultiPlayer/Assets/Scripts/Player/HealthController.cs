using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class HealthController : NetworkBehaviour, IDamageable
{
    [Header("Components")] private TransformController m_TransformController;
    private PlayerController m_PlayerController;

    [SerializeField] public NetworkVariable<int> m_Health = new(100);
    [SerializeField] private GameEvent OnPlayerDie;
    public bool isDead;

    public bool hasBeenRecentlyShot = false;

    private Coroutine GoFasterRoutine;

    [Header("Executors")]
    private RewardExecutorWeakness rewardExecutorWeakness;
    private RewardExecutorClone rewardExecutorClone;

    private void Awake()
    {
        isDead = false;
        rewardExecutorWeakness = GetComponent<RewardExecutorWeakness>();
        rewardExecutorClone = GetComponent<RewardExecutorClone>();
        m_TransformController = GetComponent<TransformController>();
        m_PlayerController = GetComponent<PlayerController>();
    }

    //aqui se mira si es grande, pequenho, se lo gestiona el mismo...
    public bool DamageHider()
    {
        if (!IsServer) return false;

        if (m_PlayerController.isClone)
        {
            rewardExecutorClone.KMS();
            return false;
        }

        if (GetComponent<PlayerController>().isDummy)
        {
            GetComponent<NetworkObject>().Despawn();
            Destroy(gameObject);
            return false;
        }
        float damage = m_TransformController.Size switch
        {
            PropSize.SMALL => Constants.Instance.HiderSmallDamage,
            PropSize.LARGE => Constants.Instance.HiderLargeDamage,
            _ => Constants.Instance.HiderMediumDamage
        };
        damage = rewardExecutorWeakness.OnBeforeReceiveDamage(damage);
        m_Health.Value -= (int)damage;
        GetComponent<AudioController>().PlayAudioRpc(0);
        if (GoFasterRoutine != null)
        {
            print("GoFasterCoroutine Dentro IF");
            StopCoroutine(GoFasterRoutine);
        }

        print("GoFasterCoroutine After IF");
        GoFasterRoutine = StartCoroutine(GoFaster());
        if (m_Health.Value > 0) return false;
        Die();
        GetComponent<AudioController>().PlayAudioRpc(1);
        return true;
    }

    private IEnumerator GoFaster()
    {
        print("GoFaster");
        hasBeenRecentlyShot = true;
        yield return new WaitForSeconds(Constants.Instance.HiderHurtSpeedMultiplierDuration);
        hasBeenRecentlyShot = false;
        print("EndGoFaster");
    }

    public bool DamageHunter()
    {
        if (!IsServer) return false;
        if (Random.Range(0, 2) == 0) return false;
        m_Health.Value -= Constants.Instance.HunterMissDamage;
        if (m_Health.Value > 0) return false;
        Die();
        return true;
    }

    public void Activate()
    {
        if (!IsServer) return;
        m_Health.Value = 100;
    }

    private void Die()
    {
        isDead = true;
        GetComponent<PlayerGUIController>().DeactivateMissionGuiRpc();
        MatchManager.Instance.Kill(m_PlayerController);
    }

    [Rpc(SendTo.Everyone)]
    public void DieRpc()
    {
        if (IsServer) GetComponent<RoleController>().role.Value = Role.SPECTATOR;
        // esto tendra que cambiar cuando hagamos los props buenos 
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().detectCollisions = false;
        //GetComponentInChildren<TextMeshPro>().enabled = false;
        GetComponent<TransformController>().NoSprite();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void RebornRpc()
    {
        if (IsServer) GetComponent<RoleController>().role.Value = Role.WANTS_RANDOM;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().detectCollisions = true;
        GetComponentInChildren<TextMeshPro>().enabled = true;
        //GetComponentInChildren<FaceCamera>().GetComponentInChildren<MeshRenderer>().enabled = true;
    }

    public void ChangeHealth(int value)
    {
        m_Health.Value = Mathf.Clamp(value + m_Health.Value, 0, 100);
        if (m_Health.Value > 0) return;
        Die();
    }
}
