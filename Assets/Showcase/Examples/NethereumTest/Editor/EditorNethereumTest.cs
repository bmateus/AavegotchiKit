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

            var web3 = new Web3(Constants.DefaultPolygonRPC);

            var getAavegotchiSvg = new GetAavegotchiSvgFunction { TokenId = 100 };

            var svc = new AavegotchiDiamondService(web3, Constants.AavegotchiDiamondAddress);

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