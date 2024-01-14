using Aavegotchi.AavegotchiDiamond.ContractDefinition;
using Cysharp.Threading.Tasks;
using PortalDefender.AavegotchiKit.Blockchain;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PortalDefender.AavegotchiKit
{
    // Fetches the SVGs for a gotchi from the blockchain and caches them
    public class GotchiChainSvgProvider : MonoBehaviour, ISvgProvider
    {
        GotchiSvgData gotchiSvgData_;
        public bool ForceRefresh { get; set; } = true;

        private async UniTask Fetch(GotchiData gotchiData)
        {
            //check for availability of Web3Provider.Instance
            if (!Web3Provider.IsInitialized)
            {
                Debug.LogError("Can't Use GotchiSvgChainProvider! Requires a Web3Provider in the scene.");
                return;
            }

            //using PreviewSideAavegotchi to get the svgs allows customizing the appearance
            List<ushort> equippedWearables = null;
            if (gotchiData.equippedWearables.Length == 8)
                equippedWearables = gotchiData.equippedWearables
                    .Concat(new ushort[8]).ToList(); //pad it out to 16
            else
                equippedWearables = gotchiData.equippedWearables.ToList();

            //fetch the gotchi appearance from on chain
            var previewAavegotchi = new PreviewSideAavegotchiFunction
            {
                HauntId = gotchiData.hauntId,
                CollateralType = gotchiData.collateral,
                NumericTraits = gotchiData.numericTraits.ToList(),
                EquippedWearables = equippedWearables
            };

            var svgs = await Web3Provider.Instance.GotchiDiamondService.PreviewSideAavegotchiQueryAsync(previewAavegotchi).AsUniTask();

            gotchiSvgData_ = new GotchiSvgData()
            {
                front = svgs[0],
                left = svgs[1],
                right = svgs[2],
                back = svgs[3]
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