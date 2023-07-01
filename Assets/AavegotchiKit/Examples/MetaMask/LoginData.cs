using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace PortalDefender.AavegotchiKit.Examples.MetaMask
{
    [CreateAssetMenu(fileName = "LoginData", menuName = "AavegotchiKit/Examples/MetaMask/LoginData")]
    public class LoginData : ScriptableObject
    {
        [SerializeField]
        string DomainName = "Aavegotchi Kit";
        
        [SerializeField]
        string DomainVersion = "1";

        [SerializeField]
        string ActionMessage = "Authentication with my account.";

        public string GetSignMessageJson(string receiverWallet, string chainId)
        {
            var fullMessage = new Dictionary<string, object>
            {
                {
                    "domain", new Dictionary<string, object>
                    {
                        { "name", DomainName },
                        { "version", DomainVersion },
                        { "chainId", chainId }
                    }
                },
                {
                    "message", new Dictionary<string, object>()
                    {
                        { "action", ActionMessage },
                        { "wallet", receiverWallet },
                    }
                },
                { "primaryType", "Mail" },
                { "types", new Dictionary<string, object>()
                {
                    {
                        "EIP712Domain", new []
                        {
                            new Dictionary<string, string>()
                            {
                                {"name", "name"},
                                {"type", "string"},
                            },
                            new Dictionary<string, string>()
                            {
                                {"name", "version"},
                                {"type", "string"},
                            },
                            new Dictionary<string, string>()
                            {
                                {"name", "chainId"},
                                {"type", "uint256"},
                            }
                        }
                    },
                    {
                        "Mail", new []
                        {
                            new Dictionary<string, string>()
                            {
                                {"name", "action"},
                                {"type", "string"},
                            },
                            new Dictionary<string, string>()
                            {
                                {"name", "wallet"},
                                {"type", "address"},
                            }
                        }
                    }
                } }
            };

            return JsonConvert.SerializeObject(fullMessage);
        }


    }
}
