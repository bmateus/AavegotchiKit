using Aavegotchi.AavegotchiDiamond.ContractDefinition;
using Aavegotchi.AavegotchiDiamond.Service;
using Nethereum.Unity.Rpc;
using Nethereum.Web3;
using PortalDefender.AavegotchiKit.Blockchain;
using System.Collections.Generic;
using UnityEngine;

namespace PortalDefender.AavegotchiKit
{
    public class GotchiMeshAppearanceFetcher : MonoBehaviour
    {
        [SerializeField]
        int gotchiId_;

        [SerializeField]
        List<string> svgs_;

        [SerializeField]
        GotchiSvgStyling styling_;

        [SerializeField]
        bool removeBG_;

        [SerializeField]
        bool removeShadow_;

        [SerializeField]
        bool initOnStart_;

        [SerializeField]
        GotchiMeshAppearance appearanceMesh_;

        private void Start()
        {
            if (initOnStart_)
            {
                var gotchi = GetComponent<Gotchi>();
                gotchi.Init(new GotchiData { id = gotchiId_ });
                gotchi.State.Facing = GotchiFacing.FRONT;
            }            
        }

        //[Sirenix.OdinInspector.Button("Fetch")]
        [ContextMenu("Fetch")]
        public async void Refresh()
        {
            IWeb3 web3 = new Web3(new UnityWebRequestRpcTaskClient(new System.Uri(Web3Provider.DefaultPolygonRPC)));
            var service = new AavegotchiDiamondService(web3, Web3Provider.DefaultAavegotchiDiamondAddress);

            var request = new GetAavegotchiSideSvgsFunction { TokenId = gotchiId_ };

            svgs_ = await service.GetAavegotchiSideSvgsQueryAsync(request);
            
            for (int i = 0; i < svgs_.Count; i++)
            {
                //styling pipeline
                GotchiSvgStyling styling = new GotchiSvgStyling()
                {
                    removeShadow = removeShadow_,
                    removeBackground = removeBG_
                };

                var styledSvg = styling.CustomizeSVG(svgs_[(int)i]);

                appearanceMesh_.UpdateGotchiMesh(styledSvg, (GotchiFacing)i, "GotchiMesh_" + gotchiId_ + "_" + (GotchiFacing)i);
            }
        }

    }
}