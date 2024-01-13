using Cysharp.Threading.Tasks;
using SimpleGraphQL;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace PortalDefender.AavegotchiKit.GraphQL
{
    public class GetGotchiSvgs
    {
        private List<string> gotchiIds_;

        private Result result_;

        public GetGotchiSvgs(List<string> gotchiIds)
        {
            this.gotchiIds_ = gotchiIds;
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
                public List<GotchiSvgResult> aavegotchis;
            }

            public Inner data;

            public List<GotchiSvgResult> GotchiSvgs => data.aavegotchis;
        }


        public async UniTask<Result> Fetch(GraphQLClient graphClient, CancellationToken cancellationToken = default)
        {
            Query query = graphClient.FindQuery("GetGotchiSvgs");

            Debug.Log($"Getting gotchi svgs {gotchiIds_}");

            var resultJSON = await graphClient.Send(
                query.ToRequest(new Dictionary<string, object> {
                    { "ids" , gotchiIds_.ToArray() }
                }))
                .AsUniTask()
                .AttachExternalCancellation(cancellationToken);

            Debug.Log(resultJSON);

            result_ = JsonUtility.FromJson<Result>(resultJSON);

            return result_;
        }

    }
}