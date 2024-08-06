using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private GameObject m_Quad;

    [SerializeField]
    [Min(0f)]
    private float m_MovementSpeed = 40f;
    [SerializeField]
    [Range(1f, 5f)]
    private float m_SprintMultiplier = 2f;
    [SerializeField]
    private float m_RotationSpeed = 360f;
    [SerializeField]
    private bool m_InvertMouseY = false;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //Show/Hide texture plane
        if (Input.GetKeyDown(KeyCode.T))
            m_Quad.SetActive(!m_Quad.activeSelf);

        bool sprint = Input.GetKey(KeyCode.LeftShift);
        //Move Camera
        Vector3 movement = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            movement += transform.forward;
        if (Input.GetKey(KeyCode.S))
            movement -= transform.forward;
        if (Input.GetKey(KeyCode.A))
            movement -= transform.right;
        if (Input.GetKey(KeyCode.D))
            movement += transform.right;

        transform.position += movement.normalized * m_MovementSpeed * (sprint ? m_SprintMultiplier : 1) * Time.deltaTime;

        //Rotate Camera
        if (Input.GetMouseButton(1))
            transform.localEulerAngles = transform.localEulerAngles
                                        + Vector3.right * Input.GetAxis("Mouse Y") * Time.deltaTime * m_RotationSpeed * (m_InvertMouseY ? 1 : -1)
                                        + Vector3.up * Input.GetAxis("Mouse X") * Time.deltaTime * m_RotationSpeed;
    }
}
