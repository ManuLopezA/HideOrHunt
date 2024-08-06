using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    private new Camera camera;
    private void Awake()
    {
        camera = GameObject.Find("CAMAROTE").GetComponent<Camera>();
    }

    void Update()
    {
        transform.rotation = Quaternion.Euler(0f, camera.transform.rotation.eulerAngles.y, 0f);
    }
}
