using UnityEngine;

[CreateAssetMenu(fileName = "NewDataSprites", menuName = "DataSprites")]
public class TransformDataSO : ScriptableObject
{
    [Header("GAME SPRITES CONFIG")]
    [Header("Props & PLayers")]
    public float inclination;
    
    [Header("Props size increase")]
    [Range(0.01f, 0.10f)]
    public float increase;
}
