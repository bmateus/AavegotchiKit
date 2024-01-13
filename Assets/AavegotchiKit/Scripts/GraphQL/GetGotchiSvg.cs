using Cysharp.Threading.Tasks;
using SimpleGraphQL;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace PortalDefender.AavegotchiKit.GraphQL
{
    public class GetGotchiSvg
    {
        private string gotchiId_;

        private Result result_;

        public GetGotchiSvg(string gotchiId)
        {
            this.gotchiId_ = gotchiId;
        }


        [System.Serializable]
        public class GotchiSvgResult
        {    
            public string id;
            public string svg;
            public string left;
            public string right;
            public string back;
        }

        [System.Serializable]
        public class Result
        {
            [System.Serializable]
            public class Inner
            {
                public GotchiSvgResult aavegotchi;
            }

            public Inner data;

            public GotchiSvgResult GotchiSvg => data.aavegotchi;
        }


        public async UniTask<Result> Fetch(GraphQLClient graphClient, CancellationToken cancellationToken = default)
        {
            Query query = graphClient.FindQuery("GetGotchiSvg");

            Debug.Log($"Getting gotchi svg {gotchiId_}");

            var resultJSON = await graphClient.Send(
                query.ToRequest(new Dictionary<string, object> {
                    { "id" , gotchiId_ }
                }))
                .AsUniTask()
                .AttachExternalCancellation(cancellationToken);

            Debug.Log(resultJSON);

            result_ = JsonUtility.FromJson<Result>(resultJSON);

            return result_;
        }

    }
}