using UnityEngine;

public class Centro : MonoBehaviour
{
    private Vector3 escalaOriginal = new Vector3(1, 1, 1);
    [SerializeField] private SpriteRenderer sprite;

    // Start is called before the first frame update
    private void OnMouseOver()
    {
        sprite.transform.localScale = escalaOriginal * 1.5f;
    }

    private void OnMouseExit()
    {
        sprite.transform.localScale = escalaOriginal;
    }
}
