using Aavegotchi.AavegotchiDiamond.ContractDefinition;
using Aavegotchi.AavegotchiDiamond.Service;
using Cysharp.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

namespace PortalDefender.AavegotchiKit
{
    public class PetGotchiRequest
    {
        int gotchiId_;

        public PetGotchiRequest(int gotchiId)
        {
            gotchiId_ = gotchiId;
        }

        public async UniTaskVoid Send(IWeb3 web3)
        {
            try
            {       
                var diamond = new AavegotchiDiamondService(web3, Constants.AavegotchiDiamondAddress);
                var result = await diamond.InteractRequestAndWaitForReceiptAsync(new InteractFunction()
                {
                    TokenIds = new List<BigInteger>() { gotchiId_ },

                });

                //Errors can happen, so check for them
                if (result.Succeeded())
                {
                    Debug.Log("Success!");
                }
                else
                {
                    Debug.Log("Error: " + result.ToString());
                }

            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }


    }
}