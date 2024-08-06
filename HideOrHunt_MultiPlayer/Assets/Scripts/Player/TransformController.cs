using Unity.Netcode;
using UnityEngine;

public class TransformController : NetworkBehaviour
{
    public PropSize Size = PropSize.MEDIUM;
    public string PropName;
    [SerializeField] private GameObject originalSprite;
    [SerializeField] private GameObject propSprite;
    public bool isTransformed => !originalSprite.activeSelf;

    [SerializeField] private BoxCollider propCollider;
    [SerializeField] private CapsuleCollider hiderCollider;

    [SerializeField] private GameObject animationSM;

     [Rpc(SendTo.Everyone)]
    public void TransformRpc(string propName, PropSize propSize)
    {
        if (animationSM != null)
        {
            animationSM.GetComponent<AnimationStateMachine>().ResetAnimator();
        }
        PropName = propName;
        Size = propSize;
        NoSprite();
        GameObject prop = MatchManager.Instance.GetProp(propName).gameObject;
        if (!prop) return;
        propSprite = Instantiate(prop.GetComponent<PropController>().Sprite, transform);
        CopyPropInfo(prop);
    }

    [Rpc(SendTo.Everyone)]
    public void DestransformRpc()
    {
        Size = PropSize.MEDIUM;
        Destroy(propSprite);
        propCollider.enabled = false;
        hiderCollider.enabled = true;
        originalSprite.SetActive(true);
        if (animationSM != null)
        {
            animationSM.GetComponent<AnimationStateMachine>().ResetAnimator();
        }
        GetComponent<OnMouseOverSprite>().DeleteSprite();
    }

    public void CloneTransformSpriteRpc()
    {
        GetComponentInChildren<SpriteSelector>().ChangeArchetype(GetComponent<PlayerController>()._characterSelected.Value);
    }

    public void NoSprite()
    {
        Destroy(propSprite);
        originalSprite.SetActive(false);
        hiderCollider.enabled = false;
    }

    public void CopyPropInfo(GameObject prop)
    {
        BoxCollider propBoxCollider = prop.GetComponent<BoxCollider>();
        propCollider.enabled = true;
        propCollider.center = propBoxCollider.center;
        propCollider.size = propBoxCollider.size;
        Size = prop.GetComponent<PropController>().Size;
        GetComponent<OnMouseOverSprite>().AssignSprite(propSprite);
    }
}