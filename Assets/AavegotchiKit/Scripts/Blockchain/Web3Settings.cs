using UnityEngine;

namespace PortalDefender.AavegotchiKit.Blockchain
{
    [CreateAssetMenu(fileName = "Web3Settings", menuName = "AavegotchiKit/Blockchain/Web3Settings")]
    public class Web3Settings : ScriptableObject
    {
        public string gotchiDiamondAddress = "0x86935F11C86623deC8a25696E1C19a8659CbF95d";

        public string rpcUrl = "https://rpc-mainnet.matic.quiknode.pro";
        
    }
}