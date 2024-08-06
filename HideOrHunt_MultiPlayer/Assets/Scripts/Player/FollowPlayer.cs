using Unity.Netcode;
using UnityEngine;

public class FollowPlayer : NetworkBehaviour
{
    public Transform jugador;
    [SerializeField] private float profundidad = 15;
    [SerializeField] private float altura = 9;

    private void Update()
    {
        if (!IsOwner) return;
        if (jugador == null) return;
        Vector3 posicionJugador = jugador.position;
        Vector3 nuevaPosicion = new Vector3(
            posicionJugador.x,
            posicionJugador.y + altura,
            posicionJugador.z - profundidad
        );
        transform.position = nuevaPosicion;
    }
}