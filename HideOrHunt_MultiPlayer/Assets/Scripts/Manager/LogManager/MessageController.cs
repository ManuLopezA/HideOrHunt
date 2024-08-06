using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MessageController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image icon;


    public void SetMessage(string msg, LogType type)
    {
        text.text = msg;
        icon.color = type switch
        {
            LogType.INFO => Color.blue,
            LogType.WARNING => Color.yellow,
            LogType.SUCCESS => Color.green,
            LogType.DANGER => Color.red,
            _ => Color.white
        };
        StartCoroutine(DeleteMessage());
    }

    private IEnumerator DeleteMessage()
    {
        yield return new WaitForSeconds(Constants.Instance.MessageDuration);
        Destroy(gameObject);
    }

}
