using Cysharp.Threading.Tasks;
using PortalDefender.AavegotchiKit.Utils;
using SimpleGraphQL;
using System.Threading;
using UnityEngine;

namespace PortalDefender.AavegotchiKit.GraphQL
{
    /**
     * This class is responsible for handling all queries to the Graph
     */
    public class GraphManager : SingletonBehaviour<GraphManager>
    {
        [SerializeField]
        GraphQLConfig graphConfig;

        GraphQLClient graphClient;

        public override void Awake()
        {
            base.Awake();

            graphClient = new GraphQLClient(graphConfig);
        }

        public async UniTask<UserAccount> GetUserAccount(string userId, CancellationToken cancellationToken = default)
        {
            GetUserAavegotchis request = new GetUserAavegotchis(userId);
            GetUserAavegotchis.Result result = await request.Fetch(graphClient, cancellationToken);
            if (result != null)
            {
                return result.User;
            }
            return null;
        }

        public async UniTask<GotchiData> GetGotchiData(string gotchiId, CancellationToken cancellationToken = default)
        {
            GetGotchiInfo request = new GetGotchiInfo(gotchiId);
            GetGotchiInfo.Result result = await request.Fetch(graphClient, cancellationToken);
            if (result != null)
            {
                return result.Gotchi;
            }            
            return null;
        }

    }
}
