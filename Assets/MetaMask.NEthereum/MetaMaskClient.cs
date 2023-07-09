using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MetaMask.Models;
using Nethereum.JsonRpc.Client;
using Nethereum.JsonRpc.Client.RpcMessages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MetaMask.NEthereum
{
    public class MetaMaskClient : ClientBase
    {
        private MetaMaskWallet _metaMask;

        public MetaMaskClient(MetaMaskWallet metaMask)
        {
            this._metaMask = metaMask;
        }
        
        private static readonly Random rng = new Random();
        private static readonly DateTime UnixEpoch =
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long GenerateRpcId()
        {
            var date = (long)((DateTime.UtcNow - UnixEpoch).TotalMilliseconds) * (10L * 10L * 10L);
            var extra = (long)Math.Floor(rng.NextDouble() * (10.0 * 10.0 * 10.0));
            return date + extra;
        }

        protected override async Task<RpcResponseMessage> SendAsync(RpcRequestMessage message, string route = null)
        {
            // Regenerate the NEthereum json-rpc id
            var id = GenerateRpcId();

            RpcRequestMessage rpcRequestMessage = null;

            if (message.Method == "eth_signTypedData_v4" || message.Method == "personal_sign")
            {
                var arrayParameters = message.RawParameters as object[];
                var parameters = new object[] { _metaMask.SelectedAddress, arrayParameters[0] };
                rpcRequestMessage = new RpcRequestMessage(id, message.Method, parameters);
            }
            else
            {
                var mapParameters = message.RawParameters as Dictionary<string, object>;
                var arrayParameters = message.RawParameters as object[];
                var rawParameters = message.RawParameters;

                rpcRequestMessage = mapParameters != null
                    ? new RpcRequestMessage(id, message.Method, mapParameters)
                    : arrayParameters != null
                        ? new RpcRequestMessage(id, message.Method, arrayParameters)
                        : new RpcRequestMessage(id, message.Method, rawParameters);
            }


            try
            {
                var response = await _metaMask.Request(new MetaMaskEthereumRequest()
                {
                    Id = rpcRequestMessage.Id.ToString(),
                    Method = rpcRequestMessage.Method,
                    Parameters = rpcRequestMessage.RawParameters
                });

                return new RpcResponseMessage(rpcRequestMessage.Id, response.ToString());

            }
            catch (Exception e)
            {
                throw new RpcClientUnknownException("Error occurred when trying to send rpc requests(s)", e);
            }


        }

        protected override Task<RpcResponseMessage[]> SendAsync(RpcRequestMessage[] requests)
        {
            return Task.WhenAll(requests.Select(r => SendAsync(r)));
        }
    }
}