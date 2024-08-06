using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class MenuManager : NetworkBehaviour
    {
        [SerializeField] private Slider maxPlayersSlider;
        [SerializeField] private TextMeshProUGUI maxPlayersText;
        [SerializeField] private TextMeshProUGUI dbPasswordMsg;
        [SerializeField] private TextMeshProUGUI loginMsg;

        public int maxPlayers;
        public int characterSelected;

        public bool ipHost;
        public string ipAddress;
        [SerializeField] private GameObject loadingGui;

        
        private void ActivateLoadingGui()
        {
            loadingGui.SetActive(true);
        }
        
        private void Start()
        {
            maxPlayersText.text = "10";
            maxPlayersSlider.value = 10;
            maxPlayers = 10;
            if (DevTools.Instance.SinglePlayer)
            {
                NetManager.Instance.StartHost();
            }
        }

        public void UpdateDifficultyDropdown(int value)
        {
            GameManager.Instance.difficulty = value switch
            {
                0 => Difficulty.EASY,
                1 => Difficulty.MEDIUM,
                2 => Difficulty.HARD,
                _ => Difficulty.EASY
            };
        }

        public void UpdateSliderValue(float value)
        {
            maxPlayersText.text = " " + value;
            maxPlayers = (int)value;
        }

        public void UpdateCharacterSelected(int ch)
        {
            characterSelected = ch;
        }

        public void ChangeIpAddress(string ip)
        {
            if (ip == "")
            {
                NetManager.Instance.ip = "localhost";
                return;
            }
            if (!ValidIP(ip)) return;
            ipAddress = ip;
            NetManager.Instance.ip = ip;
            ipHost = true;
        }

        private bool ValidIP(string ip)
        {
            const string pattern =
                @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
            if (System.Text.RegularExpressions.Regex.IsMatch(ip, pattern)) return true;
            ipHost = false;
            return false;
        }

        [SerializeField] private GameObject initial;
        [SerializeField] private GameObject join;
        [SerializeField] private GameObject create;
        [SerializeField] private GameObject character;

        public void GoToInitalCanvas()
        {
            join.gameObject.SetActive(false);
            create.gameObject.SetActive(false);
            character.gameObject.SetActive(false);
            initial.gameObject.SetActive(true);
        }

        private bool isHost;

        public void GoToJoinCanvas()
        {
            isHost = false;
            create.gameObject.SetActive(false);
            character.gameObject.SetActive(false);
            initial.gameObject.SetActive(false);
            join.gameObject.SetActive(true);
        }

        public void GoToCreateCanvas()
        {
            isHost = true;
            character.gameObject.SetActive(false);
            initial.gameObject.SetActive(false);
            join.gameObject.SetActive(false);
            create.gameObject.SetActive(true);
        }


        public void GoToCharacterCanvas()
        {
            if (isHost)
            {
                var body = new ValidatePassword
                {
                    password = GameManager.Instance.db_password
                };
                DBHelper<ValidatePassword, ValidatePasswordResponse> helper = new("validate", body);
                var response = helper.GetResponse();
                if (response.ok)
                {
                    if (!response.data.isValid)
                    {
                        dbPasswordMsg.text = "Password is incorrect!";
                        dbPasswordMsg.color = Color.yellow;
                        return;
                    }
                }
                else
                {
                    dbPasswordMsg.text = "Unexpected Error!";
                    dbPasswordMsg.color = Color.red;
                    return;
                }
            }

            initial.gameObject.SetActive(false);
            join.gameObject.SetActive(false);
            create.gameObject.SetActive(false);
            character.gameObject.SetActive(true);
        }

        public void GoBack()
        {
            join.gameObject.SetActive(false);
            create.gameObject.SetActive(false);
            character.gameObject.SetActive(false);
            initial.gameObject.SetActive(true);
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        public void StartGame()
        {
            var body = new UserLogin
            {
                username = GameManager.Instance.nickname,
                password = GameManager.Instance.password
            };

            DBHelper<UserLogin, UserLoginResponse> helper = new("login", body);
            var response = helper.GetResponse();
            if (response.ok)
            {
                loginMsg.text = response.data.msg;
                loginMsg.color = response.data.status switch
                {
                    "error" => Color.red,
                    "warning" => Color.yellow,
                    "success" => Color.green,
                    "info" => Color.blue,
                    _ => Color.white
                };
                if (!response.data.isValid)
                {
                    return;
                }   
                ActivateLoadingGui();
            }
            else
            {
                return;
            }
            if (isHost)
            {
                NetManager.Instance.StartHost();
            }
            else
            {
                NetManager.Instance.StartClient();
            }
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
