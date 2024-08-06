using UnityEngine;

public class InclinacionGO : FindSOData
{
    protected override void Awake()
    {
        base.Awake();
        transform.eulerAngles = new Vector3(data.inclination, 0f, 0f);
    }
}
