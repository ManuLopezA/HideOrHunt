using TMPro;
using UnityEngine;

public class InfoBoxDetailController : InfoBoxController
{
    [SerializeField] private TextMeshProUGUI nickName;
    [SerializeField] private TextMeshProUGUI hp;

    public void StartInfoBoxDetail(PlayerController playerController, Color color)
    {
        StartInfoBox(playerController, color);
        nickName.SetText(playerController._name.Value.ToString());
        hp.SetText($"{playerController.GetComponent<HealthController>().m_Health.Value}/100");
    }

    private void UpdateLifeDetail(int prev, int current)
    {
        UpdateInfoBoxLife(prev, current);
        hp.SetText($"HP: {current}/100");
    }

    public new void Subscribe(HealthController healthController)
    {
        healthController.m_Health.OnValueChanged += UpdateLifeDetail;
    }

    public new void Unsubscribe(HealthController healthController)
    {
        healthController.m_Health.OnValueChanged -= UpdateLifeDetail;
    }
}