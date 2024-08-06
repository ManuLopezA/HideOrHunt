using UnityEngine;

public class OnMouseOverSprite : MonoBehaviour
{
    [SerializeField] private GameObject sprite;
    private Vector3 originalScale;
    private float increaseScale;
    private float scale = 1f;

    private void Awake()
    {
        Init();
        increaseScale = 0.1f;
        increaseScale += scale;
    }

    private void Init()
    {
        if (sprite == null) return;
        originalScale = sprite.transform.localScale;
    }

    private void OnMouseOver()
    {
        if (sprite == null) return;
        sprite.transform.localScale = originalScale * increaseScale;
        if (GameManager.Instance.owner == null) return;
        if (Vector3.Distance(transform.position, GameManager.Instance.owner.transform.position) > Constants.Instance.ActionRange)
        {
            Constants.Instance.SetCursor(Constants.Instance.cursorCantShoot);
        }
        else
        {
            Constants.Instance.SetCursor(Constants.Instance.cursorCanShoot);
        }
    }

    private void OnMouseExit()
    {
        if (sprite == null) return;
        sprite.transform.localScale = originalScale;
        Constants.Instance.SetCursor(Constants.Instance.cursorDefault);
    }
    public void AssignSprite(GameObject spriteToPut)
    {
        sprite = spriteToPut;
        Init();
    }

    public void DeleteSprite()
    {
        sprite = null;
    }

}
