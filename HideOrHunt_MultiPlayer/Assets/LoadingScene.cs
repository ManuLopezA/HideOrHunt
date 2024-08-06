using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private Image hideOrHuntLogo;
    private string[] text = new string[4];
    [SerializeField] private bool logoOpacityChange;
    [SerializeField] private bool textOpacityChange;
    
    [Range(0.01f, 0.1f)]
    [SerializeField] private float fadeInterval = 0.05f; 
    private float opacityStep = 0.05f;

    void Awake()
    {
        text[0] = "loading";
        text[1] = "loading\n.";
        text[2] = "loading\n.  .";
        text[3] = "loading\n.  .  .";
    }

    private void Start()
    {
        StartCoroutine(UpdateLoadingText());
        StartCoroutine(LogoAndTextOpacity());
    }

    private IEnumerator UpdateLoadingText()
    {
        int index = 0;
        while (true)
        {
            loadingText.text = text[index];
            index = (index + 1) % text.Length;
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator LogoAndTextOpacity()
    {
        bool fadingOut = true;
        float sharedAlpha = 1.0f; // Variable de opacidad compartida

        while (true)
        {
            if (fadingOut)
            {
                sharedAlpha -= opacityStep;
                if (sharedAlpha <= 0.05)
                {
                    sharedAlpha = 0.05f;
                    fadingOut = false;
                }
            }
            else
            {
                sharedAlpha += opacityStep;
                if (sharedAlpha >= 1)
                {
                    sharedAlpha = 1;
                    fadingOut = true;
                }
            }

            if (logoOpacityChange)
            {
                Color logoColor = hideOrHuntLogo.color;
                logoColor.a = sharedAlpha;
                hideOrHuntLogo.color = logoColor;
            }

            if (textOpacityChange)
            {
                Color textColor = loadingText.color;
                textColor.a = sharedAlpha;
                loadingText.color = textColor;
            }

            yield return new WaitForSeconds(fadeInterval);
        }
    }
}