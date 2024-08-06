using System;
using System.ComponentModel;
using System.Diagnostics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

// [Serializable]
// internal class MapVote
// {
//     public Terrain terrain;
//     public string name;
//     public Color color;
//     public Image image;
// }

public class LobbyVoteManager : NetworkBehaviour
{
    public NetworkVariable<int> votes_mountain = new();
    public NetworkVariable<int> votes_desert = new();
    public NetworkVariable<int> votes_river = new();
    public Button button_vote;
    public Button button_next;
    public Button button_prev;

    // [SerializeField] private GameObject buttonPrefab;
    // [SerializeField] private GameObject mapArray;
    // [SerializeField] private MapVote[] mapButtons;
    // [SerializeField] private List<GameObject> buttons;

    public override void OnNetworkSpawn()
    {
        // foreach (MapVote map in mapButtons)
        // {
        //     GameObject button = Instantiate(buttonPrefab, mapArray.transform);
        //     button.name = map.name;
        //     button.GetComponentInChildren<TextMeshProUGUI>().text = map.name;
        //     button.GetComponent<Button>().onClick.AddListener(() => Vote(map.terrain, button));
        //     buttons.Add(button);
        // }
    }

    public Terrain GetSelectedTerrain()
    {
        if (votes_river.Value > votes_desert.Value && votes_river.Value > votes_mountain.Value)
        {
            return Terrain.RIVER;
        }

        if (votes_desert.Value > votes_river.Value && votes_desert.Value > votes_mountain.Value)
        {
            return Terrain.DESERT;
        }

        if (votes_mountain.Value > votes_river.Value && votes_mountain.Value > votes_desert.Value)
        {
            return Terrain.MOUNTAIN;
        }

        return Terrain.MOUNTAIN;
    }

    public void Vote()
    {
        button_vote.interactable = false;
        // button_vote.GetComponent<Image>().color = Color.grey;
        button_next.interactable = false;
        button_next.GetComponent<Image>().color = Color.gray;
        button_prev.interactable = false;
        button_prev.GetComponent<Image>().color = Color.gray;

        AddVoteRpc(LobbyManager.Instance.voteGui.GetComponent<SelectionMapManager>().Index);
    }

    [Rpc(SendTo.Server)]
    public void AddVoteRpc(int indexSelected)
    {
        switch (indexSelected)
        {
            case 0:
                print("mountain");
                votes_mountain.Value++;
                break;
            case 1:
                print("desert");
                votes_desert.Value++;
                break;
            case 2:
                print("river)");
                votes_river.Value++;
                break;
            default:
                votes_mountain.Value++;
                break;
        }
        // check if all players have voted
        int playerCount = LobbyManager.Instance.playerCount.Value;
        if (votes_river.Value + votes_desert.Value + votes_mountain.Value >= playerCount)
        {
            LobbyManager.Instance.StartGame();
        }
    }
}