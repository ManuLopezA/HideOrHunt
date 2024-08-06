using TMPro;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GUIResultManager : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI points;
    [SerializeField] private TextMeshProUGUI web;

    [SerializeField] private AudioClip losers;
    [SerializeField] private AudioClip winners;

    [Rpc(SendTo.ClientsAndHost)]
    public void SetWinningTeamRpc(Role teamRole)
    {
        title.text = teamRole switch
        {
            Role.HUNTER => "Hunters won!",
            Role.HIDER => "Hiders won!",
            _ => "?"
        };

        title.color = teamRole switch
        {
            Role.HUNTER => Color.red,
            Role.HIDER => Color.blue,
            _ => Color.white
        };

        web.text = "Check the scores on the website";

        if (teamRole == GameManager.Instance.owner.GetComponent<RoleController>().role.Value)
            GetComponent<AudioSource>().clip = winners;
        else
            GetComponent<AudioSource>().clip = losers;
        GetComponent<AudioSource>().Play();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void SetScoreRpc(string user, int score)
    {
        if (user != GameManager.Instance.owner._name.Value) return;
        points.enabled = true;
        points.text = $"Score: {score}";
    }
}
