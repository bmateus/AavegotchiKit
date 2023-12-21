using Aavegotchi.AavegotchiDiamond.Service;
using Nethereum.Unity.Rpc;
using Nethereum.Web3;
using PortalDefender.AavegotchiKit.Utils;
using System;
using UnityEngine;

namespace PortalDefender.AavegotchiKit.Blockchain
{
    // A component that provides an IWeb3 instance
    public class Web3Provider : SingletonBehaviour<Web3Provider>
    {
        public static string DefaultPolygonRPC = "https://rpc-mainnet.matic.quiknode.pro";

        public static string DefaultAavegotchiDiamondAddress = "0x86935F11C86623deC8a25696E1C19a8659CbF95d";

        [SerializeField]
        Web3Settings settings;

        string rpcUrl => settings ? settings.rpcUrl : DefaultPolygonRPC;

        string gotchiDiamondAddress => settings ? settings.gotchiDiamondAddress : DefaultAavegotchiDiamondAddress;

        private IWeb3 web3_ = null;

        public IWeb3 web3 => 
            web3_ ??= new Web3(new UnityWebRequestRpcTaskClient(new Uri(rpcUrl)));

        private AavegotchiDiamondService aavegotchiDiamond_ = null;

        public AavegotchiDiamondService GotchiDiamondService => 
            aavegotchiDiamond_ ??= new AavegotchiDiamondService(web3, gotchiDiamondAddress);
    }
}