using Unity.Netcode;
using UnityEngine;

namespace m17
{
    public class PlayerBehaviourSimple : NetworkBehaviour
    {
        //Variable senzilla, la pot updatejar el client -> Això no s'hauria de fer "que fan trampes"
        NetworkVariable<Color> m_Color = new NetworkVariable<Color>(Color.white, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [SerializeField] private GameEvent<string> m_OnIDChangedEvent;

        // No es recomana fer servir perquè estem en el món de la xarxa.
        // Utilitzeu OnNetworkSpawn millor
        // Recordeu que si no el feu servir, s'ha d'esborrar (el deixo per a que llegiu això)
        void Start()
        {
        }

        // Això sí que seria el nou awake
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            m_Color.OnValueChanged += OnColorChanged;
            GetComponent<SpriteRenderer>().color = m_Color.Value;

            //Codi que només s'executarà al servidor
            if (IsServer)
            {
                transform.position = new Vector3(Random.Range(-7f, 7f), Random.Range(-5f, 5f), 0);
            }

            //Codi que s'executarà només si ets owner
            if (!IsOwner)
                return;

            m_OnIDChangedEvent.Raise("ID: " + OwnerClientId);
        }

        // Això podriem dir que seria el nou ondestroy (però només en si implica xarxa)
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            m_Color.OnValueChanged -= OnColorChanged;
        }

        //Sempre tindran aquest format amb oldValue i newValue
        private void OnColorChanged(Color previousValue, Color newValue)
        {
            GetComponent<SpriteRenderer>().color = newValue;
        }

        void Update()
        {
            //Aquest update només per a qui li pertany
            if (!IsOwner)
                return;

            if (Input.GetKeyDown(KeyCode.Space))
                m_Color.Value = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        }
    }
}
