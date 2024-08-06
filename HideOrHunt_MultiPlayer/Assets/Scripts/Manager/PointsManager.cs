using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PointsManager : NetworkBehaviour
{
    public static PointsManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(this);
    }

    // <username, score>
    private Dictionary<string, int> PlayerPoints = new();

    public void SubmitPoints()
    {
        var body = new UsersSubmit
        {
            password = GameManager.Instance.db_password,
            data = new UserScore[PlayerPoints.Count]
        };

        int i = 0;
        foreach (var entry in PlayerPoints)
        {
            var user = new UserScore
            {
                username = entry.Key,
                score = entry.Value
            };
            body.data[i++] = user;
        }
        DBHelper<UsersSubmit, UsersSubmitResponse> helper = new("submit", body);
        var res = helper.GetResponse();
        if (!res.ok)
        {
            Debug.LogWarning("There was an error reaching the db");
            return;
        }
        if (!res.data.success)
        {
            Debug.LogWarning("There was an error writing data into the db");
            return;
        }
        Debug.Log("Data written successfully!");
    }

    public void ClearPoints()
    {
        List<string> keys = new List<string>(PlayerPoints.Keys);
        foreach (string key in keys)
        {
            PlayerPoints[key] = 0;
        }
    }

    public int GetPoints(string username)
    {
        if (PlayerPoints.ContainsKey(username)) return PlayerPoints[username];
        else return 1;
    }

    [Rpc(SendTo.Server)]
    public void AddPlayerRpc(string username)
    {
        PlayerPoints.TryAdd(username, 0);
    }

    public void RemovePlayer(string username)
    {
        if (!PlayerPoints.ContainsKey(username)) return;
        PlayerPoints.Remove(username);
    }

    public void KilledPlayer(ulong uid, string username)
    {
        if (!PlayerPoints.ContainsKey(username)) return;

        PlayerPoints[username] += Constants.Instance.PointsKill;
        PointsLog(uid, username, Constants.Instance.PointsKill);
    }

    public void CompleteMission(ulong uid, string username, Difficulty diff)
    {
        if (!PlayerPoints.ContainsKey(username)) return;

        int points = diff switch
        {
            Difficulty.EASY => Constants.Instance.PointsMissionEasy,
            Difficulty.MEDIUM => Constants.Instance.PointsMissionMedium,
            Difficulty.HARD => Constants.Instance.PointsMissionHard,
            _ => 0
        };

        PlayerPoints[username] += points;
        PointsLog(uid, username, points);
    }

    private void PointsLog(ulong uid, string username, int points)
    {
        Debug.Log($"{username} has won {points} points");
        LogManager.Instance.LogSingleRpc($"{username} has won {points} points", LogType.SUCCESS, uid);
    }

}
