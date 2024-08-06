using Unity.Netcode;
using UnityEngine;

public class PlayerGUIController : NetworkBehaviour
{
    [SerializeField] private GameObject infoBoxPrefab;
    private InfoBoxController infoBox;
    private PlayerController playerController;
    private RoleController roleController;
    private HealthController healthController;


    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        roleController = GetComponent<RoleController>();
        healthController = GetComponent<HealthController>();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void DeactivateMissionGuiRpc()
    {
        if (!IsOwner) return;
        GUIManager.Instance.missionGui.SetActive(false);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void CreateClientInfoBoxRpc()
    {
        infoBox = (roleController.role.Value switch
        {
            Role.HIDER => Instantiate(infoBoxPrefab, GUIManager.Instance.hidersLifeGrid.transform),
            Role.HUNTER => Instantiate(infoBoxPrefab, GUIManager.Instance.huntersLifeGrid.transform),
            _ => Instantiate(infoBoxPrefab)
        }).GetComponent<InfoBoxController>();
        infoBox.StartInfoBox(playerController, roleController.GetRoleColor());
        infoBox.Subscribe(healthController);

        if (playerController.isDummy) return;
        if (!IsOwner) return;

        GUIManager.Instance.ownInfo.StartInfoBoxDetail(playerController, roleController.GetRoleColor());
        GUIManager.Instance.ownInfo.Subscribe(healthController);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void DestroyClientInfoBoxRpc()
    {
        if (infoBox == null) return;
        infoBox.Unsubscribe(healthController);
        Destroy(infoBox.gameObject);
        if (playerController.isDummy) return;
        if (!IsOwner) return;
        GUIManager.Instance.ownInfo.Unsubscribe(healthController);
    }
}
