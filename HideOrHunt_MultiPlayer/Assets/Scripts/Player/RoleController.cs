using Unity.Netcode;
using UnityEngine;

public class RoleController : NetworkBehaviour
{
    public NetworkVariable<Role> role = new();

    public override void OnNetworkSpawn()
    {
        role.OnValueChanged += OnRoleChange;
        if (!IsServer) return;
        if (GetComponent<PlayerController>().isDummy) return;
        if (!DevTools.Instance.SkipLobby) return;
        role.Value = DevTools.Instance.Role;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GetComponent<PlayerController>().isDummy) return;
        if (!IsServer) return;
        if (other.CompareTag(HOH_Tags.LobbyHunterArea))
        {
            role.Value = Role.WANTS_HUNTER;
            LobbyManager.Instance.huntCount.Value++;
        }
        else if (other.CompareTag(HOH_Tags.LobbyHiderArea))
        {
            role.Value = Role.WANTS_HIDER;
            LobbyManager.Instance.hideCount.Value++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (GetComponent<PlayerController>().isDummy) return;
        if (!IsServer) return;
        if (!other.CompareTag(HOH_Tags.LobbyHunterArea) && !other.CompareTag(HOH_Tags.LobbyHiderArea)) return;
        if (other.CompareTag(HOH_Tags.LobbyHunterArea))
        {
            LobbyManager.Instance.huntCount.Value--;
        }
        else if (other.CompareTag(HOH_Tags.LobbyHiderArea))
        {
            LobbyManager.Instance.hideCount.Value--;
        }

        role.Value = Role.WANTS_RANDOM;
    }

    private void OnRoleChange(Role old, Role newRole)
    {
        GetComponent<PlayerController>().nick.color = GetRoleColor();
        if (!GetComponent<PlayerController>().isDummy && IsOwner) return;
        GetComponent<PlayerController>().minimapDot.GetComponent<MeshRenderer>().material.color = GetRoleColor();
    }

    public Color GetRoleColor()
    {
        return role.Value switch
        {
            Role.HUNTER => Color.red,
            Role.HIDER => Color.blue,
            _ => Color.white
        };
    }
}
