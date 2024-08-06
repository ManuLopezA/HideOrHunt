using UnityEngine;
using UnityEngine.UI;

public class InfoBoxController : MonoBehaviour
{
    [SerializeField] protected Color color;
    [SerializeField] protected Image lifeSlice;
    [SerializeField] protected Image background;
    [SerializeField] protected Image character;

    public void StartInfoBox(PlayerController playerController, Color color)
    {
        character.sprite = GameManager.Instance.characterSprites[playerController._characterSelected.Value];
        background.color = color;
    }

    protected void UpdateInfoBoxLife(int prev, int current)
    {
        float life = current / 100f;
        lifeSlice.transform.localScale = new Vector3(life, 1, 1);
    }

    public void Subscribe(HealthController healthController)
    {
        healthController.m_Health.OnValueChanged += UpdateInfoBoxLife;
    }

    public void Unsubscribe(HealthController healthController)
    {
        healthController.m_Health.OnValueChanged -= UpdateInfoBoxLife;
    }
}