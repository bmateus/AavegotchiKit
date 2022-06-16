using Cysharp.Threading.Tasks;
using SimpleGraphQL;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace PortalDefender.AavegotchiKit
{
    /**
     * This class is responsible for handling all queries to theGraph
     */
    public class GraphManager : MonoBehaviour
    {
        static GraphManager instance;
        public static GraphManager Instance => instance;

        [SerializeField]
        GraphQLConfig graphConfig;

        GraphQLClient graphClient;

        private void Awake()
        {
            instance = this;

            graphClient = new GraphQLClient(graphConfig);
        }

        public async UniTask<UserAccount> GetUser(string userId)
        {
            Query query = graphClient.FindQuery("GetUserAavegotchis");
            string result = await graphClient.Send(
                query.ToRequest(new Dictionary<string, object> {
                    { "id" , userId }
                }));

            var resultObj = JsonUtility.FromJson<LoadPlayerResult>(result);

            //todo: cache result
            //userAccounts.Add(resultObj.User.id, resultObj.User);

            return resultObj.User;
        }

        public async UniTask<GotchiData> GetGotchi(string gotchiId, CancellationToken cancellationToken = default)
        {
            Query query = graphClient.FindQuery("GetGotchiInfo");

            Debug.Log($"Getting gotchi {gotchiId}");

            var result = await graphClient.Send(
                query.ToRequest(new Dictionary<string, object> {
                    { "id" , gotchiId }
                }))
                .AsUniTask()
                .AttachExternalCancellation(cancellationToken);

            Debug.Log(result);

            var resultObj = JsonUtility.FromJson<LoadGotchiResult>(result);

            return resultObj.Gotchi;
        }

        #region Classes to handle results from Graph Queries

        [System.Serializable]
        class LoadPlayerResult
        {
            [System.Serializable]
            public class Inner
            {
                public UserAccount user;
            }

            public Inner data;

            public UserAccount User => data.user;
        }

        [System.Serializable]
        class LoadGotchiResult
        {
            [System.Serializable]
            public class Inner
            {
                public GotchiData aavegotchi;
            }

            public Inner data;

            public GotchiData Gotchi => data.aavegotchi;
        }

        #endregion

    }
}
