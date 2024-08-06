using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace m17
{
    public class HandleConnectionUI : MonoBehaviour
    {
        [SerializeField]
        private Button m_ServerButton;
        [SerializeField]
        private Button m_ClientButton;
        [SerializeField]
        private Button m_HostButton;


        void Awake()
        {
            m_ServerButton.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartServer();
            });

            m_ClientButton.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartClient();
            });

            m_HostButton.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartHost();
            });
        }
    }
}

