using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LobbyRoleManager : NetworkBehaviour
{
    [SerializeField] private int huntersCount;

    public List<PlayerController> hiders = new();
    public List<PlayerController> hunters = new();
    public List<PlayerController> randoms = new();

    public void AssignRolesToPlayers()
    {
        huntersCount = GetHuntersCount();
        AssignChosenRole(Role.WANTS_HUNTER, Role.HUNTER, hunters);
        AssignChosenRole(Role.WANTS_HIDER, Role.HIDER, hiders);
        AssignChosenRole(Role.WANTS_RANDOM, Role.WANTS_RANDOM, randoms);
        BalanceHunters();
        BalanceHiders();
    }

    private int GetHuntersCount()
    {
        int playerCount = GameManager.Instance.All.Count;
        return GameManager.Instance.difficulty switch
        {
            Difficulty.EASY => Mathf.CeilToInt(playerCount / 5f),
            Difficulty.MEDIUM => Mathf.CeilToInt(playerCount / 3f),
            Difficulty.HARD => Mathf.CeilToInt(playerCount / 2f),
            _ => 0
        };
    }

    private void AssignChosenRole(Role wantsToBe, Role willBe, ICollection<PlayerController> roleList)
    {
        foreach (PlayerController player in GameManager.Instance.All)
        {
            RoleController roleController = player.GetComponent<RoleController>();
            if (roleController.role.Value != wantsToBe) continue;
            roleController.role.Value = willBe;
            roleList.Add(player);
        }
    }

    private void BalanceHunters()
    {
        while (hunters.Count < huntersCount)
        {
            if (randoms.Count > 0)
            {
                MovePlayer(randoms, hunters, Role.HUNTER);
            }
            else if (hiders.Count > 0)
            {
                MovePlayer(hiders, hunters, Role.HUNTER);
            }
            else break;
        }

        while (hunters.Count > huntersCount)
        {
            MovePlayer(hunters, hiders, Role.HIDER);
        }
    }

    private void BalanceHiders()
    {
        while (randoms.Count > 0)
        {
            MovePlayer(randoms, hiders, Role.HIDER);
        }
    }

    private void MovePlayer(List<PlayerController> from, List<PlayerController> to, Role role)
    {
        PlayerController player = from[0];
        player.GetComponent<RoleController>().role.Value = role;
        to.Add(player);
        from.RemoveAt(0);
    }
}