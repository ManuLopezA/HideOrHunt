using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f; // Velocidad de movimiento del personaje

    void Update()
    {
        float horizontalInput = 0f;
        float verticalInput = 0f;
        
        if (Input.GetKey(KeyCode.A))
        {
            horizontalInput = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            horizontalInput = 1f;
        }
        if (Input.GetKey(KeyCode.W))
        {
            verticalInput = 1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            verticalInput = -1f;
        }

        // Calcular la dirección del movimiento basado en las teclas presionadas
        Vector3 movementDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        // Aplicar el movimiento al personaje
        transform.Translate(movementDirection * speed * Time.deltaTime);
    }
}