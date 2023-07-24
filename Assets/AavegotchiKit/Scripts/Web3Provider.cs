using Aavegotchi.AavegotchiDiamond.Service;
using Nethereum.Unity.Rpc;
using Nethereum.Web3;
using System;
using UnityEngine;

namespace PortalDefender.AavegotchiKit
{
    // A component that provides a Web3 instance
    public class Web3Provider : MonoBehaviour
    {
        private Web3 web3_ = null;

        public Web3 web3 => 
            web3_ ??= new Web3(new UnityWebRequestRpcTaskClient(new Uri(Constants.DefaultPolygonRPC)));

        private AavegotchiDiamondService aavegotchiDiamond_ = null;

        public AavegotchiDiamondService GotchiDiamondSvc => 
            aavegotchiDiamond_ ??= new AavegotchiDiamondService(web3, Constants.AavegotchiDiamondAddress);
    }
}