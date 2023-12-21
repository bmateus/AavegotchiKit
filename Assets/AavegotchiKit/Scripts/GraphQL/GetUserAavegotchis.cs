using Cysharp.Threading.Tasks;
using SimpleGraphQL;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace PortalDefender.AavegotchiKit.GraphQL
{
    public class GetUserAavegotchis
    {
        private string userId_;

        private Result result_;

        public GetUserAavegotchis(string userId)
        {
            this.userId_ = userId;
        }

        [System.Serializable]
       public class Result
        {
            [System.Serializable]
            public class Inner
            {
                public UserAccount user;
            }

            public Inner data;

            public UserAccount User => data.user;
        }


        public async UniTask<Result> Fetch(GraphQLClient graphClient, CancellationToken cancellationToken = default)
        {
            Query query = graphClient.FindQuery("GetUserAavegotchis");
            string resultJSON = await graphClient.Send(
                query.ToRequest(new Dictionary<string, object> {
                    { "id" , userId_ }
                }));

            result_ = JsonUtility.FromJson<Result>(resultJSON);

            return result_;
        }

    }
}
