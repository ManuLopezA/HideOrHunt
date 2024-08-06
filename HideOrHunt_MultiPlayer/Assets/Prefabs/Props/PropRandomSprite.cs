using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PropRandomSprite : MonoBehaviour
{
    [SerializeField] private List<GameObject> sprites;

    private void Awake()
    {
        if (sprites.Count != 0)
        {
            foreach (var sprite in sprites)
            {
                sprite.SetActive(false);
            }
            int randomIndex = Random.Range(0, sprites.Count);
            sprites[randomIndex].SetActive(true);
        }
    }
}
