using Cysharp.Threading.Tasks;
using PortalDefender.AavegotchiKit.GraphQL;
using UnityEngine;

namespace PortalDefender.AavegotchiKit
{
    public class GotchiGraphSvgProvider : MonoBehaviour, ISvgProvider
    {
        GotchiSvgData gotchiSvgData_;
        public bool ForceRefresh { get; set; } = true;

        private async UniTask Fetch(GotchiData gotchiData)
        {
            //check for availability of Web3Provider.Instance
            if (!GraphManager.IsInitialized)
            {
                Debug.LogError("Can't Use GotchiSvgGraphProvider! Requires a GraphManager in the scene.");
                return;
            }

            var result = await GraphManager.Instance.GetGotchiSvg(gotchiData.id.ToString());

            Debug.Log("GotchiGraphSvgProvider: " + result.svg);

            gotchiSvgData_ = new GotchiSvgData()
            {
                front = result.svg,
                left = result.left,
                right = result.right,
                back = result.back
            };

        }

        public async UniTask<GotchiSvgData> GetSvg(GotchiData gotchiData)
        {
            if (ForceRefresh || gotchiSvgData_ == null)
            {
                await Fetch(gotchiData);
            }
            return gotchiSvgData_;
        }

        public async UniTask<string> GetSvg(GotchiData gotchiData, GotchiFacing facing)
        {
            return (await GetSvg(gotchiData)).GetFacing(facing);
        }
    }
}
