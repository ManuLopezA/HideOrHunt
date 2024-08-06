using Unity.Netcode;
using UnityEngine;

public class PlayerMatchController : NetworkBehaviour
{
    private PlayerController playerController;
    private ShootController shootController;
    private RoleController roleController;
    private HealthController healthController;
    private RewardExecutorClone rewardExecutorClone;

    private void Awake()
    {
        rewardExecutorClone = GetComponent<RewardExecutorClone>();
        playerController = GetComponent<PlayerController>();
        shootController = GetComponent<ShootController>();
        roleController = GetComponent<RoleController>();
        healthController = GetComponent<HealthController>();
    }

    public void StartGameServer()
    {
        if (playerController.isDummy) return;
        shootController.m_Action = roleController.role.Value switch
        {
            Role.HIDER => shootController.HandleHiderTransformAction,
            Role.HUNTER => shootController.HandleHunterShootAction,
            _ => null
        };
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void StartGameClientRpc()
    {
        healthController.Activate();
        if (!IsOwner) return;
        if (playerController.isDummy) return;
        if (GetComponent<RoleController>().role.Value == Role.HIDER)
        {
            playerController.shoot.started += shootController.Shoot;
        }
        playerController.anchor.started += playerController.Anchor;
        if (roleController.role.Value != Role.HIDER) return;
        Debug.Log("PlayerMatchController Clone");
        playerController.clone.performed += rewardExecutorClone.SpawnClone;
    }

    public void OnHidingEnd()
    {
        if (IsServer)
        {
            if (playerController.GetComponent<RoleController>().role.Value == Role.HUNTER)
                playerController.speed = Constants.Instance.MovementSpeed;
        }
        StartHuntersGameClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void StartHuntersGameClientRpc()
    {
        if (GetComponent<RoleController>().role.Value != Role.HUNTER) return;
        playerController.shoot.started += shootController.Shoot;
    }

    public void EndMatchServer()
    {
        shootController.m_Action = null;
        roleController.role.Value = Role.WANTS_RANDOM;
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void EndMatchClientRpc()
    {
        if (!IsOwner) return;
        playerController.shoot.started -= shootController.Shoot;
        playerController.anchor.started -= playerController.Anchor;
        playerController.clone.performed -= rewardExecutorClone.SpawnClone;
    }
}
