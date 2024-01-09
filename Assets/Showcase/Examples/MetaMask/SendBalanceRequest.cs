using Cysharp.Threading.Tasks;
using Nethereum.Web3;
using System;
using UnityEngine;

namespace PortalDefender.AavegotchiKit.Examples.MetaMask
{
    public class SendBalanceRequest
    {
        string address_;

        public SendBalanceRequest(string address)
        {
            address_ = address;
        }

        public async UniTask<float> Send(IWeb3 web3)
        {
            try
            {
                Debug.Log("Try to get balance of " + address_);
                var result = await web3.Eth.GetBalance.SendRequestAsync(address_);
                Debug.Log("Got result: " + result.ToString());
                var balance = Web3.Convert.FromWei(result);
                Debug.Log($"SendBalanceRequest - Balance: {balance}");
                return (float)balance;
            }
            catch (Exception ex)
            {
                Debug.LogError($"SendBalanceRequest - Error getting balance: {ex.Message}");
                Debug.LogException(ex);
            }

            return 0.0f;
        }
    }
}