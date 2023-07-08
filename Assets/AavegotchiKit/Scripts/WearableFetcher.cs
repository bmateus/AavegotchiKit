using Aavegotchi.AavegotchiDiamond.ContractDefinition;
using Aavegotchi.AavegotchiDiamond.Service;
using Cysharp.Threading.Tasks;
using Nethereum.ABI;
using Nethereum.ABI.Decoders;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using UnityEngine;

namespace PortalDefender.AavegotchiKit
{
    using Vector2 = UnityEngine.Vector2;

    public class WearableFetcher
    {
        Web3 web3_;

        Web3 Web3 => web3_ ??= new Web3(Constants.DefaultPolygonRPC);

        AavegotchiDiamondService diamondSvc_;
        AavegotchiDiamondService GotchiDiamond => diamondSvc_ ??= new AavegotchiDiamondService(Web3, Constants.AavegotchiDiamondAddress);

        private async UniTask<string[]> GetWearableSvgs(int wearableId, uint svgId)
        {
            byte[][] types = new byte[][]
            {
                Encoding.UTF8.GetBytes("wearables"),
                Encoding.UTF8.GetBytes("wearables-left"),
                Encoding.UTF8.GetBytes("wearables-right"),
                Encoding.UTF8.GetBytes("wearables-back")
            };

            //Method A:

            string[] svgs = new string[4];
            for (int i = 0; i < 4; ++i)
            {
                svgs[i] = await GotchiDiamond.GetSvgQueryAsync(new GetSvgFunction() { SvgType = types[i], ItemId = svgId });
            }


            // Method B:

            //svgs = (await GotchiDiamond.GetItemSvgsQueryAsync(new GetItemSvgsFunction() { ItemId = wearableId })).ToArray();

            //Method B is only 1 call so it's more efficient, but it adds a box around the SVG.

            return svgs;
        }

        // this is pretty crazy, but I think it's the only way to get the side view offsets from the contract
        private async UniTask<Vector2[]> GetWearableOffsets(int wearableId)
        {
            //first get the previewSideAavegotchi of some default gotchi
            //i don't think it matters what slot it's in, it should return the same offset;
            //there is some magic though if the item is in the WEARABLE_SLOT_HAND_LEFT slot, 
            //it gets flipped and shifted:
            //'<g transform="scale(-1, 1) translate(-', 64 - (dimensions.x * 2)),', 0)">' <[wearable]> </g>

            var result = await GotchiDiamond.PreviewSideAavegotchiQueryAsync(
                new PreviewSideAavegotchiFunction()
                {
                    HauntId = 1,
                    CollateralType = "0xE0b22E0037B130A9F56bBb537684E6fA18192341",
                    NumericTraits = new List<short> { 50, 50, 50, 50, 50, 50 },
                    EquippedWearables = new List<ushort> { (ushort)wearableId, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
                });

            var offsets = new List<Vector2>();

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(new NameTable());
            nsmgr.AddNamespace("svg", "http://www.w3.org/2000/svg");

            for (int side = 0; side < 4; ++side)
            {
                XDocument xml = XDocument.Parse(result[side]);

                //select first node in path svg/g that has class="gotchi-wearable"
                //then get the x and y attributes of the svg node
                //and add them to the offsets list

                var element = xml.XPathSelectElement("//svg:g[contains(concat(' ', normalize-space(@class), ' '), 'gotchi-wearable')]", nsmgr);
                //select the first descendant named svg
                element = element.XPathSelectElement("svg:svg", nsmgr);

                offsets.Add(new Vector2((float)element.Attribute("x"), (float)element.Attribute("y")));
            }

            return offsets.ToArray();
        }

        // reads from sleeves in AavegotchiDiamond's LibAppStorage
        //  mapping(uint256 => uint256) sleeves;
        private async UniTask<int> GetSleevesSvgId(int wearableId)
        {
            int SLEEVES_SLOT = 58;
            BigInteger position = new ABIEncode().GetSha3ABIEncoded(new ABIValue("int", wearableId), new ABIValue("int", SLEEVES_SLOT)).ToHex().HexToBigInteger(false);
            var result = await Web3.Eth.GetStorageAt.SendRequestAsync(Constants.AavegotchiDiamondAddress, position.ToHexBigInteger());
            return (int)new IntTypeDecoder().DecodeBigInteger(result);
        }

        private async UniTask<string[]> GetSleevesSvgs(int sleevedSvgId)
        {
            byte[][] types = new byte[][]
            {
                Encoding.UTF8.GetBytes("sleeves"),
                Encoding.UTF8.GetBytes("sleeves-left"),
                Encoding.UTF8.GetBytes("sleeves-right"),
                Encoding.UTF8.GetBytes("sleeves-back")
            };

            string[] svgs = new string[4];
            for (int i = 0; i < 4; ++i)
            {
                svgs[i] = await GotchiDiamond.GetSvgQueryAsync(new GetSvgFunction() { SvgType = types[i], ItemId = sleevedSvgId });
            }

            return svgs;
        }

        public async UniTask<Wearable> GetWearable(int wearableId)
        {
            try
            {
                Wearable wearable = new Wearable();

                var response = await GotchiDiamond.GetItemTypeQueryAsync(new GetItemTypeFunction() { ItemId = wearableId });
                var itemData = response.Itemtype;

                wearable.id = wearableId;
                wearable.name = itemData.Name;
                wearable.description = itemData.Description;
                wearable.author = itemData.Author;
                wearable.traitModifiers = Array.ConvertAll(itemData.TraitModifiers.ToArray(), Convert.ToInt32);
                wearable.slotPositions = itemData.SlotPositions.ToArray();
                //wearable.allowedCollaterals = itemData.AllowedCollaterals.ToArray(); //TODO:??investigate
                wearable.dimensions = new int[] { itemData.Dimensions.X, itemData.Dimensions.Y, itemData.Dimensions.Width, itemData.Dimensions.Height }; //TODO: make this nicer
                wearable.rarity = (GotchiWearableRarity)itemData.RarityScoreModifier;
                wearable.minLevel = itemData.MinLevel;
                //wearable.svgId = itemData.SvgId; //only used for grabbing svgs
                wearable.category = itemData.Category;
                wearable.svgs = await GetWearableSvgs(wearableId, itemData.SvgId);
                wearable.offsets = await GetWearableOffsets(wearableId);

                bool isBodyWearable = itemData.SlotPositions[0];
                if (isBodyWearable)
                {
                    //try to get sleeves id
                    var sleevesSvgId = await GetSleevesSvgId(wearableId);
                    Debug.Log($"got sleeves for id: {wearableId} = {sleevesSvgId}");
                    if (sleevesSvgId != 0
                        || wearableId == 8 //what's this for? was there a bug?
                    )
                    {
                        //Debug.Log("adding sleeves...");
                        wearable.sleeves = await GetSleevesSvgs(sleevesSvgId);
                    }
                }

                return wearable;
            }
            catch (Exception e)
            {
                Debug.Log("Error: " + e.Message);
            }

            return null;
        }

    }
}