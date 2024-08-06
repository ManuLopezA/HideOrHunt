using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIMissionManager : MonoBehaviour
{
    public static GUIMissionManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public GameObject missionGui;
    [SerializeField] private TextMeshProUGUI missionTitle;
    [SerializeField] private TextMeshProUGUI missionDescription;
    [SerializeField] private TextMeshProUGUI missionProgress;
    [SerializeField] private TextMeshProUGUI missionTimer;
    public GameObject imageGrande;
    public GameObject missionImage;
    public GameObject missionCameraImage;
    public GameObject missionCamera;

    private void Start()
    {
        try
        {
            //if (GameManager.Instance.owner.GetComponent<RoleController>().role.Value == Role.HIDER) return;
            imageGrande.SetActive(false);
            missionGui.SetActive(false);
            missionImage.SetActive(false);
        }
        catch { }
    }
    private int maxProgress;

    public void NewMissionData(string title, string description, Sprite image, int maxProgress)
    {
        missionTitle.SetText(title);
        missionDescription.SetText(description);
        missionProgress.SetText($"0/{maxProgress}");
        missionImage.GetComponent<Image>().sprite = image;
        if (maxProgress == 0) return;
        this.maxProgress = maxProgress;
    }

    public void UpdateMissionProgress(int _, int curr)
    {
        Debug.Log($"GUIMISSIONMAN: UpdateMissionProgress{curr}/{maxProgress}");
        missionProgress.SetText($"{curr}/{maxProgress}");
    }

    public void UpdateMissionTimer(int _, int curr)
    {
        int min = curr / 60;
        int sec = curr % 60;
        missionTimer.text = $"{min:00}:{sec:00}";
    }
}
