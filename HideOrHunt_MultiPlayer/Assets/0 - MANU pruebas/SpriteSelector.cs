using System.Collections.Generic;
using UnityEngine;

public class SpriteSelector : MonoBehaviour
{
    [SerializeField] private List<GameObject> characters = new();
    public enum characterArchetype
    {
        DRAGON, BARBARIAN, XENACLAMP, KENNY
    }
    [SerializeField] private int selector = 0;

    private void Awake()
    {
        foreach (var archetype in characters)
        {
            archetype.SetActive(false);
        }

        ChangeArchetype(selector);
    }

    public void ChangeArchetype(int selector)
    {
        // selector = (selector + 1) % 4;
        foreach (var archetype in characters)
        {
            archetype.SetActive(false);
        }

        switch ((characterArchetype)selector)
        {
            case characterArchetype.DRAGON:
                characters[0].SetActive(true);
                break;
            case characterArchetype.BARBARIAN:
                characters[1].SetActive(true);
                break;
            case characterArchetype.XENACLAMP:
                characters[2].SetActive(true);
                break;
            case characterArchetype.KENNY:
                characters[3].SetActive(true);
                break;
        }
    }
}
