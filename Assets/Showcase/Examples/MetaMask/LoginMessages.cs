using TMPro;
using UnityEngine;

namespace PortalDefender.AavegotchiKit.Examples.MetaMask
{
    public class LoginMessages : MonoBehaviour
    {
        [SerializeField]
        TMP_Text messages;

        private void Awake()
        {
            messages.text = "";
            LoginManager.LoginError += (msg) => {
                messages.text = msg;
            };
        }

    }
}