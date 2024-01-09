using TMPro;
using UnityEngine;

namespace PortalDefender.AavegotchiKit.Examples.MetaMask
{
    public class LoginScreen : MonoBehaviour
    {
        [SerializeField]
        TMP_Text step1; // 1. Scan the QR Code
        
        [SerializeField]
        TMP_Text step2; // 2. Click Connect

        [SerializeField]
        TMP_Text step3; // 3. Click Sign

        // Start is called before the first frame update
        void Start()
        {
            LoginManager.LoginStateChanged += (state) =>
            {
                if (state == LoginManager.LoginState.LoggedIn)
                {
                    //hide this once we're logged in
                    gameObject.SetActive(false);
                }
                else
                {
                    step1.color = state == LoginManager.LoginState.Offline ? Color.white : Color.gray;
                    step2.color = state == LoginManager.LoginState.Connecting ? Color.white : Color.gray;
                    step3.color = state == LoginManager.LoginState.Signing ? Color.white : Color.gray;
                }
            };

            //hide on startup
            gameObject.SetActive(false);
        }
        
    }

}