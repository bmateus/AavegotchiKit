using Cysharp.Threading.Tasks;
using Nethereum.Hex.HexTypes;
using Nethereum.Signer.EIP712;
using Nethereum.Web3;
using System;
using UnityEngine;


namespace PortalDefender.AavegotchiKit.Examples.MetaMask
{
    public class SendSignRequest
    {
        string selectedAddress_;
        string signingMessage_;

        public SendSignRequest(string selectedAddress_, string signingMessage_)
        {
            this.selectedAddress_ = selectedAddress_;
            this.signingMessage_ = signingMessage_;
        }

        public async UniTask<bool> Send(IWeb3 web3)
        {
            Debug.Log("SendSignRequest: " + selectedAddress_ + " " + signingMessage_);

            var response = await web3.Eth.AccountSigning.SignTypedDataV4.SendRequestAsync(new HexUTF8String(signingMessage_));

            Debug.Log("Got response: " + response);

            try
            {
                var success = ValidateSignTransaction(signingMessage_, response.ToString());
                if (!success)
                {
                    Debug.LogError("Unable to validate sign transaction message");
                }
                return success;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            return false;
        }

        private bool ValidateSignTransaction(string message, string signature)
        {
            var addressRecovered = Eip712TypedDataSigner.Current.RecoverFromSignatureV4(message, signature).ToLower();

            if (addressRecovered == selectedAddress_)
            {
                return true;
            }

            Debug.LogError($"The message was signed by the wrong person.\n" +
                           $"Expected wallet: {selectedAddress_}\n" +
                           $"Recovered: {addressRecovered}");
            return false;
        }

    }
}