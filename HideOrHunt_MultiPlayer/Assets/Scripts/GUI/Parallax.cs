using UnityEngine;

public class Parallax : MonoBehaviour
{
    Vector2 StartPosition;
    [SerializeField] int force;

    private void Start()
    {
        StartPosition = transform.position;
    }

    private void Update()
    {
        Vector2 pz = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        float posX = Mathf.Lerp(transform.position.x, StartPosition.x + (pz.x * force), 2f * Time.deltaTime);
        float posY = Mathf.Lerp(transform.position.y, StartPosition.y + (pz.y * force), 2f * Time.deltaTime);

        transform.position = new Vector3(posX, posY, 0);
    }
}
