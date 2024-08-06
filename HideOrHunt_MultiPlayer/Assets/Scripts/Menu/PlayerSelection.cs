using Menu;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelection : MonoBehaviour
{
    private int index;
    public int Index => index;
    [SerializeField] private Image image;


    private void Start()
    {
        ChangeImage();
    }

    public void NextCharacter()
    {
        if (index < GameManager.Instance.characterSprites.Length - 1) index++;
        else index = 0;
        GetComponent<MenuManager>().UpdateCharacterSelected(index);
        ChangeImage();
    }

    public void PreviousCharacter()
    {
        if (index > 0) index--;
        else index = GameManager.Instance.characterSprites.Length - 1;
        GetComponent<MenuManager>().UpdateCharacterSelected(index);
        ChangeImage();
    }

    private void ChangeImage()
    {
        GameManager.Instance.gm_selectedCharacter = index;
        image.GetComponent<Image>().sprite = GameManager.Instance.characterSprites[index];
    }
}
