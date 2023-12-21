using Cysharp.Threading.Tasks;
using SimpleGraphQL;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace PortalDefender.AavegotchiKit.GraphQL
{
    public class GetGotchiInfo
    {
        private string gotchiId_;

        private Result result_;

        public GetGotchiInfo(string gotchiId)
        {
            this.gotchiId_ = gotchiId;
        }

        [System.Serializable]
        public class Result
        {
            [System.Serializable]
            public class Inner
            {
                public GotchiData aavegotchi;
            }

            public Inner data;

            public GotchiData Gotchi => data.aavegotchi;
        }


        public async UniTask<Result> Fetch(GraphQLClient graphClient, CancellationToken cancellationToken = default)
        {
            Query query = graphClient.FindQuery("GetGotchiInfo");

            Debug.Log($"Getting gotchi {gotchiId_}");

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