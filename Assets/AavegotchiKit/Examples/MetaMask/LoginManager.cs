using System;
using System.Numerics;
using System.Threading.Tasks;
using MetaMask;
using MetaMask.Models;
using MetaMask.Transports.Unity;
using MetaMask.Unity;
using Nethereum.Signer.EIP712;
using PortalDefender.AavegotchiKit.Utils;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Nethereum.Web3;
using MetaMask.NEthereum;
using Nethereum.Hex.HexTypes;

namespace PortalDefender.AavegotchiKit.Examples.MetaMask
{
    // Your one stop shop for logging in with MetaMask!
    // Handles logging in with MetaMask Extension on WebGL
    // And falls back to loggin in with Deeplink on Android and iOS (if avaiable)
    // And finally logging in using a QR code WallectConnect-style

    // The Login Flow can be activated by calling LoginManager.StartLogin()

    public class LoginManager : SingletonBehaviour<LoginManager>, IMetaMaskUnityTransportListener
    {
        #region Events

        /// <summary>Raised when the wallet is connected.</summary>
        public event EventHandler onWalletConnected;
        /// <summary>Raised when the wallet is disconnected.</summary>
        public event EventHandler onWalletDisconnected;
        /// <summary>Raised when the wallet is ready.</summary>
        public event EventHandler onWalletReady;
        /// <summary>Raised when the wallet is paused.</summary>
        public event EventHandler onWalletPaused;
        /// <summary>Raised when the wallet is connected.</summary>
        public event EventHandler onWalletAuthorized;
        /// <summary>Raised when the user signs and sends the document.</summary>
        public event EventHandler onSignSend;
        /// <summary>Occurs when a transaction is sent.</summary>
        public event EventHandler onTransactionSent;
        /// <summary>Raised when the transaction result is received.</summary>
        /// <param name="e">The event arguments.</param>
        public event EventHandler<MetaMaskEthereumRequestResultEventArgs> onTransactionResult;

        #endregion



        // Keep track of the current login step
        public enum LoginState
        {
            Offline,
            Connecting,
            Signing,
            LoggedIn
        }

        private LoginState state_ = LoginState.Offline;

        public static event Action<LoginState> LoginStateChanged;

        // Only used for Android and iOS
#pragma warning disable CS0067
        public static event Action ShowConnectMobile; //TODO: additional flow for mobile
#pragma warning restore CS0067

        //Login Manager events to listen for
        public static event Action<string> WalletReceived;
        public static event Action<string> BalanceReceived;
        public static event Action LoggedIn;
        public static event Action LoggedOut;
        public static event Action<string> LoginError;

        [Header("MetaMask")]
        [SerializeField]
        private MetaMaskUnity MetaMaskObj;
                
        [SerializeField]
        GameObject LoginScreen;

        //true if using the browser extension flow for metamask
        private bool usingExtensionFlow_ = false;

        private LoginData loginData_;

        private string ChainId = "0x89"; //Polygon Mainnet

        void Awake()
        {
            // Only keep 1 instance of LoginManager alive
            var objs = FindObjectsOfType<LoginManager>();

            if (objs.Length > 1)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(MetaMaskObj);

            loginData_ = Resources.Load<LoginData>("LoginData");
            if (loginData_ == null)
            {
                Debug.LogError("LoginData not found!");
            }

            // Sign up for wallet events
            MetaMaskObj.Initialize();
            MetaMaskObj.Wallet.WalletConnected += WalletConnected;
            MetaMaskObj.Wallet.WalletDisconnected += WalletDisconnected;
            MetaMaskObj.Wallet.WalletReady += WalletReady;
            MetaMaskObj.Wallet.WalletPaused += WalletPaused;    
            MetaMaskObj.Wallet.WalletAuthorized += WalletAuthorized;
            MetaMaskObj.Wallet.WalletUnauthorized += WalletUnauthorized;
            MetaMaskObj.Wallet.EthereumRequestResultReceived += EthereumRequestResultReceived;
            MetaMaskObj.Wallet.EthereumRequestFailed += EthereumRequestFailed;
            MetaMaskObj.Wallet.ChainIdChanged += ChainIdChanged;
            MetaMaskObj.Wallet.AccountChanged += AccountChanged;
        }

        private void OnDestroy()
        {
            MetaMaskObj.Wallet.WalletConnected -= WalletConnected;
            MetaMaskObj.Wallet.WalletDisconnected -= WalletDisconnected;
            MetaMaskObj.Wallet.WalletReady -= WalletReady;
            MetaMaskObj.Wallet.WalletPaused -= WalletPaused;
            MetaMaskObj.Wallet.WalletAuthorized -= WalletAuthorized;
            MetaMaskObj.Wallet.WalletUnauthorized -= WalletUnauthorized;
            MetaMaskObj.Wallet.EthereumRequestResultReceived -= EthereumRequestResultReceived;
            MetaMaskObj.Wallet.EthereumRequestFailed -= EthereumRequestFailed;
            MetaMaskObj.Wallet.ChainIdChanged -= ChainIdChanged;
            MetaMaskObj.Wallet.AccountChanged -= AccountChanged;
        }


        public void LogIn()
        {
            Debug.Log("LoginManager::LogIn");

#if UNITY_WEBGL && !UNITY_EDITOR
            
            //first check to see if we can use the web extension flow

            if (MetamaskWebglInterop.IsMetamaskAvailable()) //is the browser extension installed?
            {
                usingExtensionFlow_ = true;
                MetamaskWebglInterop.EnableEthereum(gameObject.name, nameof(EthereumEnabled), nameof(DisplayError));
            }
#endif
            if (!usingExtensionFlow_)
            {
                InitializeLoginScreen();
            }
        }

        public void LogOut()
        {
            Debug.Log("LoginManager::LogOut");

#if UNITY_WEBGL && !UNITY_EDITOR
            if (usingExtensionFlow_)
            {
                MetamaskWebglInterop.DisableEthereum(gameObject.name, nameof(DisplayError));
            }
#endif
            if (!usingExtensionFlow_)
            {
                MetaMaskObj.Wallet.Disconnect();
            }               
        }

        #region WebGL Extension Methods

#if UNITY_WEBGL && !UNITY_EDITOR

        public void DisplayError(string errorMessage)
        {
            LoginError?.Invoke(errorMessage);
        }

        public void EthereumEnabled(string addressSelected)
        {
            if (!_isMetamaskInitialised)
            {
                MetamaskWebglInterop.EthereumInit(gameObject.name, nameof(NewAccountSelected), nameof(ChainChanged));
                MetamaskWebglInterop.GetChainId(gameObject.name, nameof(ChainChanged), nameof(DisplayError));
                _isMetamaskInitialised = true;
            }
            NewAccountSelected(addressSelected);
        }

#endif

#endregion WebGL Extension Methods

        private void InitializeLoginScreen()
        {       
            state_ = LoginState.Offline;
            LoginStateChanged?.Invoke(state_);

            MetaMaskObj.Wallet.Connect();

            ShowLoginScreen();

        }

        #region Handle MetaMask Unity Wallet Events

        //Happens after user scans the QR code
        private void WalletConnected(object sender, EventArgs e)
        {
            Debug.Log($">>>>> LoginManager::WalletConnected");
            
            state_ = LoginState.Connecting;
            LoginStateChanged?.Invoke(state_);

            onWalletConnected?.Invoke(this, EventArgs.Empty);
        }
        
        private void WalletDisconnected(object sender, EventArgs e)
        {
            Debug.Log($">>>>> LoginManager::WalletDisconnected");
            LoggedOut?.Invoke();

            // Can't just scan the same QR code to reconnect;
            // need to call MetamaskObj.Wallet.Connect() to set things up again
            HideLoginScreen();
            
            onWalletDisconnected?.Invoke(this, EventArgs.Empty);
        }

        // Address and Chain Id should be valid here
        private void WalletReady(object sender, EventArgs e)
        {
            Debug.Log($">>>>> LoginManager::WalletReady: " + MetaMaskObj.Wallet.SelectedAddress + " on " + MetaMaskObj.Wallet.SelectedChainId);

            if (state_ == LoginState.Signing && ChainId == MetaMaskObj.Wallet.SelectedChainId)
            {
                SendBalanceRequest().Forget();
                SendSignRequest().Forget();
            }
            
            onWalletReady?.Invoke(this, EventArgs.Empty);
        }

        private void WalletPaused(object sender, EventArgs e)
        {
            Debug.Log($"LoginManager::WalletPaused");

            onWalletPaused?.Invoke(this, EventArgs.Empty);
        }

        // User has allowed wallet to connect to the app
        private void WalletAuthorized(object sender, EventArgs e)
        {
            Debug.Log($">>>>> LoginManager::WalletAuthorized");
            
            state_ = LoginState.Signing;
            LoginStateChanged?.Invoke(state_);
            
            if (MetaMaskObj.Wallet.SelectedChainId != ChainId)
            {
                //TODO: show a popup or something
                LoginError?.Invoke($"Wrong chain id! Please switch to Polygon ({ChainId})");
                Debug.LogError($"LoginManager::WalletAuthorized - Wrong chain id! Expected {ChainId}, got {MetaMaskObj.Wallet.SelectedChainId}");
                return;
            }

            onWalletAuthorized?.Invoke(this, EventArgs.Empty);

        }

        private void WalletUnauthorized(object sender, EventArgs e)
        {
            Debug.Log($">>>>> LoginManager::WalletUnauthorized");
        }

        private void ChainIdChanged(object sender, EventArgs e)
        {
            Debug.Log($">>>>> LoginManager::ChainIdChanged: " + MetaMaskObj.Wallet.SelectedChainId);

            if (MetaMaskObj.Wallet.SelectedChainId == ChainId)
            {
                if (state_ == LoginState.Signing)
                {
                    SendSignRequest().Forget();
                    SendBalanceRequest().Forget();
                }
            }
            else
            {
                //wrong chain id!
                //TODO: show a popup or something
                LoginError?.Invoke($"Wrong chain id! Please switch to Polygon ({ChainId})");
                Debug.LogError($"LoginManager::WalletAuthorized - Wrong chain id! Expected {ChainId}, got {MetaMaskObj.Wallet.SelectedChainId}");
            }
        }

        private void AccountChanged(object sender, EventArgs e)
        {
            Debug.Log($">>>>> LoginManager::AccountChanged: " + MetaMaskObj.Wallet.SelectedAddress);

            WalletReceived?.Invoke(MetaMaskObj.Wallet.SelectedAddress);
            //TODO: re-sign?
        }   


        #endregion Handle MetaMask Unity Wallet Events

        private async UniTaskVoid SendSignRequest()
        {
            // Wait until the MetaMask SDK will set the wallet address 
            // TODO: why!? it should be ready!!
            while (string.IsNullOrEmpty(MetaMaskObj.Wallet.SelectedAddress))
            {
                Debug.Log("Waiting for MetaMask SDK to set the wallet address");
                await Task.Delay(100);
            }

            Debug.Log("LoginManager::SendSignRequest");

            // Getting the EIP-712: Typed structured data for the signing message
            string signingMessage = loginData_.GetSignMessageJson(MetaMaskObj.Wallet.SelectedAddress, ChainId);

            object paramsArray = new[] { MetaMaskObj.Wallet.SelectedAddress, signingMessage };

            var request = new MetaMaskEthereumRequest
            {
                Method = "eth_signTypedData_v4",
                Parameters = paramsArray,
            };

            var response = await MetaMaskObj.Wallet.Request(request);

            try
            {
                var success = ValidateSignTransaction(signingMessage, response.ToString());
                if (!success)
                {
                    Debug.LogError("Unable to validate sign transaction message");
                    return;
                }

                state_ = LoginState.LoggedIn;
                LoginStateChanged?.Invoke(state_);

                LoggedIn?.Invoke();

            }
            catch (Exception ex) 
            { 
                Debug.LogException(ex);
            }
        }

        private bool ValidateSignTransaction(string message, string signature)
        {
            var addressRecovered = Eip712TypedDataSigner.Current.RecoverFromSignatureV4(message, signature).ToLower();

            if (addressRecovered == MetaMaskObj.Wallet.SelectedAddress)
            {
                return true;
            }

            Debug.LogError($"The message was signed by the wrong person.\n" +
                           $"Expected wallet: {MetaMaskObj.Wallet.SelectedAddress}\n" +
                           $"Recovered: {addressRecovered}");
            return false;
        }

        private async UniTaskVoid SendBalanceRequest()
        {
            // Wait until the MetaMask SDK will set the wallet address 
            // TODO: why!? it should be ready!!
            while (string.IsNullOrEmpty(MetaMaskObj.Wallet.SelectedAddress))
            {
                Debug.Log("Waiting for MetaMask SDK to set the wallet address");
                await Task.Delay(100);
            }

            try
            {
                //Try doing it with Nethereum

                Web3 web3 = MetaMaskObj.Wallet.CreateWeb3();
                Debug.Log("Try to get balance of " + MetaMaskObj.Wallet.SelectedAddress);
                var result = await web3.Eth.GetBalance.SendRequestAsync(MetaMaskObj.Wallet.SelectedAddress);
                Debug.Log("Got result: " + result.ToString());
                var balance = Web3.Convert.FromWei(result);
                Debug.Log($"LoginManager::WalletReady - Balance: {balance}");
                BalanceReceived?.Invoke(balance.ToString());
                
            }
            catch(Exception ex)
            {
                Debug.LogError($"LoginManager::WalletReady - Error getting balance: {ex.Message}");
                Debug.LogException(ex);
            }

            await UniTask.Delay(1000);

            try
            {
                //Try it with Wallet directly

                var request = new MetaMaskEthereumRequest
                {
                    Method = "eth_getBalance",
                    Parameters = new object[] { MetaMaskObj.Wallet.SelectedAddress, "latest" }
                };

                var response = await MetaMaskUnity.Instance.Wallet.Request(request);

                // Convert balance response to string and shift it by 18 decimal points to the left
                // to get the real value
                Debug.Log("Got response: " + response.ToString());
                var balance = Web3.Convert.FromWei(new HexBigInteger(response.ToString())); 
                BalanceReceived?.Invoke(balance.ToString());
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception was caught while getting balance response.\n{ex.Message}");
                BalanceReceived?.Invoke("0");
            }
        }
        
        private void EthereumRequestResultReceived(object sender, MetaMaskEthereumRequestResultEventArgs e)
        {
            Debug.Log($">>>>> LoginManager::EthereumRequestResultReceived Request:{e.Request} Result:{e.Result}");
        }

        private void EthereumRequestFailed(object sender, MetaMaskEthereumRequestFailedEventArgs e)
        {
            Debug.LogError($">>>>> LoginManager::EthereumRequestFailed Request:{e.Request} Error:{e.Error}");
        }   
      
        #region Implement IMetaMaskUnityTransportListener

        public void OnMetaMaskConnectRequest(string url)
        {
            Debug.Log($"=====> LoginManager::OnMetaMaskConnectRequest {url}");
            //Happens when you call MetaMaskObject.Connect()
        }
        public void OnMetaMaskRequest(string id, MetaMaskEthereumRequest request) { 
            Debug.Log($"=====> LoginManager::OnMetaMaskRequest {id} {request.Method}");
        }
        public void OnMetaMaskFailure(Exception error) { 
            Debug.Log($"=====> LoginManager::OnMetaMaskFailure {error.Message}");

            //the error message is in json format?

            LoginError?.Invoke(error.Message);
        }
        public void OnMetaMaskSuccess() {
            Debug.Log("======> LoginManager::OnMetaMaskSuccess");
        }

        #endregion Implement IMetaMaskUnityTransportListener

        private void ShowLoginScreen()
        {
            // TODO: do some cool transition 
            LoginScreen.SetActive(true);
        }

        private void HideLoginScreen()
        {
            // TODO: do some cool transition
            LoginScreen.SetActive(false);

            LoginError?.Invoke("");
        }


    }


}