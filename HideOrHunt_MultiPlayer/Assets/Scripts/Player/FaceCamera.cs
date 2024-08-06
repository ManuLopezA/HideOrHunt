using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private void Update()
    {
        if (Camera.main == null) return;
        transform.LookAt(Camera.main.transform);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }
}