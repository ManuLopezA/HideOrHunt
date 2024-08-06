using TMPro;
using UnityEngine;

namespace m17
{
    public class ShowClientID : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI m_Text;
        public void OnIDChanged(string ID)
        {
            m_Text.text = ID;
        }
    }
}

