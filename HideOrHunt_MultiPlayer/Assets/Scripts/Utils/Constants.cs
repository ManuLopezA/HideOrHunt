using UnityEngine;

public class Constants : MonoBehaviour
{
    [Header("Scenes")] public string MenuPrincipal = "MenuPrincipal";
    public string LobbyScene = "LobbyScene";
    public string GameScene = "GameScene";

    [Header("Cursors")] public Texture2D cursorDefault;
    public Texture2D cursorCanShoot;
    public Texture2D cursorCantShoot;
    public Vector2 cursorSize;

    public void SetCursor(Texture2D texture)
    {
        Cursor.SetCursor(texture, Constants.Instance.cursorSize, CursorMode.Auto);
    }

    [Header("Points")] public int PointsMissionEasy = 20;
    public int PointsMissionMedium = 50;
    public int PointsMissionHard = 100;
    public int PointsKill = 75;

    public int PointsWinAlive = 200;
    public int PointsWinDead = 50;

    [Header("Missions")]
    public int InitialWaitTimeRange = 10;

    [Header("Player")] public float ActionCooldownSec = 2;
    public int ActionRange = 8;
    public float MovementSpeed = 4;

    [Header("Hider")] public float HiderHurtSpeedMultiplier = 3.0f;
    public int HiderHurtSpeedMultiplierDuration = 2;

    public int HiderSmallDamage = 10;
    public int HiderMediumDamage = 10;
    public int HiderLargeDamage = 10;

    [Header("Hunter")]
    public int HunterMissDamage = 1;

    [Header("Misc")]
    public float MessageDuration = 5f;
    public string ip = "localhost";
    public string DBUri()
    {
        return $"http://{NetManager.Instance.ip}:3000";
    }

    public static Constants Instance { get; private set; }

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
        SetCursor(cursorDefault);
    }
}
