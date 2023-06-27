using Aavegotchi.AavegotchiDiamond.ContractDefinition;
using Aavegotchi.AavegotchiDiamond.Service;
using Nethereum.Web3;
using System;
using UnityEditor;
using UnityEngine;

namespace PortalDefender.AavegotchiKit.Examples
{

    public class EditorTestNethereum : MonoBehaviour
    {
        [MenuItem("AavegotchiKit/Examples/Test/EditorTestNethereum")]
        static async void DoStuff()
        {
            Debug.Log("EditorTestNethereum.DoStuff()");

            var web3 = new Web3("https://rpc-mainnet.matic.quiknode.pro");

            var getAavegotchiSvg = new GetAavegotchiSvgFunction { TokenId = 100 };

            var AAVEGOTCHI_DIAMOND_ADDRESS = "0x86935F11C86623deC8a25696E1C19a8659CbF95d";

            var svc = new AavegotchiDiamondService(web3, AAVEGOTCHI_DIAMOND_ADDRESS);

            try
            {
                var svg = await svc.GetAavegotchiSvgQueryAsync(getAavegotchiSvg);
                Debug.Log("SVG: " + svg);
            }
            catch (Exception e)
            {
                Debug.Log("Error: " + e.Message);
            }

        }
    }
}