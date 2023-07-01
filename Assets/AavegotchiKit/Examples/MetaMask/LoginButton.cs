using TMPro;
using UnityEngine;

namespace PortalDefender.AavegotchiKit.Examples.MetaMask
{
    public class LoginButton : MonoBehaviour
    {
        [SerializeField]
        GameObject loggedOutView;

        [SerializeField]
        GameObject loggedInView;

        [SerializeField]
        TMP_Text address_;

        [SerializeField]
        TMP_Text balance_;

        [SerializeField]
        LoginManager loginManager;

        private bool loggedIn_ = false;

        private void Awake()
        {
            LoginManager.LoggedIn += () => SetLoggedIn(true);
            LoginManager.LoggedOut += () => SetLoggedIn(false);

            LoginManager.WalletReceived += (wallet) =>
            {
                address_.text = wallet.Substring(2,6) + "..." + wallet.Substring(wallet.Length-4);
            };

            LoginManager.BalanceReceived += (balance) =>
            {
                //display balance to two decimals
                balance_.text = balance.Substring(0, balance.IndexOf(".") == -1 ? balance.Length : balance.IndexOf(".") + 3);
            };
        }

        public void SetLoggedIn(bool loggedIn)
        {
            loggedIn_ = loggedIn;
            loggedOutView.SetActive(!loggedIn);
            loggedInView.SetActive(loggedIn);
        }

        public void OnClick()
        {
            if (loggedIn_)
            {
                loginManager.LogOut();
            }
            else
            {
                loginManager.LogIn();
            }
        }

    }
}