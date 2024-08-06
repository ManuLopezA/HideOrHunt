using UnityEngine;

public class SeguirJugador : MonoBehaviour
{
    public string namePlayer;
    public Transform jugador;
    public float suavizado = 0.1f;
    public float offsetY = 0f;

    private Vector3 velocidad = Vector3.zero;

    private void Awake()
    {
        jugador = GameObject.Find(namePlayer).transform;
    }

    void FixedUpdate()
    {
        if (jugador != null)
        {
            // Obtener la posición actual de la cámara
            Vector3 posicionDeseada = new Vector3(jugador.position.x, transform.position.y + offsetY, transform.position.z);

            // Suavizar el movimiento de la cámara hacia la posición deseada
            transform.position = Vector3.SmoothDamp(transform.position, posicionDeseada, ref velocidad, suavizado);
        }
    }
}
