using Aavegotchi.AavegotchiDiamond.ContractDefinition;
using Aavegotchi.AavegotchiDiamond.Service;
using Cysharp.Threading.Tasks;
using Nethereum.ABI;
using Nethereum.ABI.Decoders;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.Util;
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

            string[] svgs = new string[types.Length];
            for (int i = 0; i < types.Length; ++i)
            {
                svgs[i] = await GotchiDiamond.GetSvgQueryAsync(new GetSvgFunction() { SvgType = types[i], ItemId = svgId });
            }

            return svgs;
        }

        // this is pretty crazy, but I think it's the only way to get the side view offsets from the contract
        // (without digging in LibAppStorage)
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
            // mapping(uint256 => uint256)
            int SLEEVES_SLOT = 58;
            BigInteger position = new ABIEncode().GetSha3ABIEncoded(
                new ABIValue("int", wearableId), 
                new ABIValue("int", SLEEVES_SLOT)
                ).ToHex().HexToBigInteger(false);
            var result = await Web3.Eth.GetStorageAt.SendRequestAsync(
                Constants.AavegotchiDiamondAddress, position.ToHexBigInteger());

            return (int)new IntTypeDecoder().DecodeBigInteger(result);
        }

        //e.g. s.sideViewDimensions[10]["left"]
        //used in SgvViewsFacet.sol getItemSvgs, getBodySideWearable
        private async UniTask<Dimensions> GetSideViewDimensions(int wearableId, string sideName)
        {
        // struct Dimensions {
        //    uint8 x;
        //    uint8 y;
        //    uint8 width;
        //    uint8 height;
        // }
        // mapping(uint256 => mapping(bytes => Dimensions)) sideViewDimensions;            
        int SIDE_VIEW_DIMENSIONS_SLOT = 62;
            var position = new ABIEncode().GetSha3ABIEncoded(
                new ABIValue("int", wearableId), 
                new ABIValue("int", SIDE_VIEW_DIMENSIONS_SLOT)
                ).ToHex().HexToBigInteger(false);

            // important that we use packed here!
            var position2 = new ABIEncode().GetSha3ABIEncodedPacked(
                new ABIValue("bytes", Encoding.UTF8.GetBytes(sideName)),
                new ABIValue("uint256", position)
                ).ToHex().HexToBigInteger(false);

            var resultBytes = await Web3.Eth.GetStorageAt.SendRequestAsync(
                Constants.AavegotchiDiamondAddress, position2.ToHexBigInteger());

            Dimensions dimensions = new Dimensions()
            {
                X = Convert.ToByte(resultBytes.Substring(66 - 2, 2), 16),
                Y = Convert.ToByte(resultBytes.Substring(66 - 4, 2), 16),
                Width = Convert.ToByte(resultBytes.Substring(66 - 6, 2), 16),
                Height = Convert.ToByte(resultBytes.Substring(66 - 8, 2), 16)
            };

            return dimensions;
        }


        // Wearable Exceptions modify the ordering of the layers
        private async UniTask<bool> GetWearableExceptions(int wearableId, GotchiFacing facing, GotchiEquipmentSlot equipmentSlot)
        {
            byte[][] facingKeys = new byte[][]
            {
                Encoding.UTF8.GetBytes("wearables-front"),
                Encoding.UTF8.GetBytes("wearables-left"),
                Encoding.UTF8.GetBytes("wearables-right"),
                Encoding.UTF8.GetBytes("wearables-back")
            };

            Debug.Log("===> GetWearableExceptions");
            
            // mapping(bytes32 => mapping(uint256 => mapping(uint256 => bool)))                        
            int WEARABLE_EXCEPTIONS_SLOT = 79;
            var position = new ABIEncode().GetSha3ABIEncoded(
                new ABIValue("bytes32", facingKeys[(int)facing]),
                new ABIValue("int", WEARABLE_EXCEPTIONS_SLOT)
                ).ToHex().HexToBigInteger(false);

            var position2 = new ABIEncode().GetSha3ABIEncoded(
                new ABIValue("int", wearableId),
                new ABIValue("uint256", position)
                ).ToHex().HexToBigInteger(false);
            
            var position3 = new ABIEncode().GetSha3ABIEncoded(
                new ABIValue("int", (int)equipmentSlot),
                new ABIValue("uint256", position2)
                ).ToHex().HexToBigInteger(false);

            var resultBytes = await Web3.Eth.GetStorageAt.SendRequestAsync(Constants.AavegotchiDiamondAddress, position3.ToHexBigInteger());
            var result = new BoolTypeDecoder().Decode<bool>(resultBytes);
            return result;
        }

        // Just a test to get svg's using getStorageAt() (bypassing SvgFacet and LibSvg)
        // The SvgLayer contains an address to a contract whose bytecode contains the SVG
        // this is used in a cool way in SvgFacet - the contract uses the assembly code 
        // "extcodecopy" to copy the SVG from that target contract
        // -> the rationale is that accessing code is cheaper than accessing storage (i think)
        private async UniTask<string> GetSvg(string name, int idx)
        {
            // mapping(bytes32 => SvgLayer[]) svgLayers;
            int SVG_LAYERS_SLOT = 2;
            var position = new ABIEncode().GetSha3ABIEncoded(
                new ABIValue("bytes32", Encoding.UTF8.GetBytes(name)),
                new ABIValue("int", SVG_LAYERS_SLOT)
                ).ToHex().HexToBigInteger(false);

            // dereference the array:
            var position2 = new ABIEncode().GetSha3ABIEncoded(
                new ABIValue("uint256", position)
            ).ToHex().HexToBigInteger(false);

            //get the specified array entry
            position2 += idx;

            var resultBytes = await Web3.Eth.GetStorageAt.SendRequestAsync(Constants.AavegotchiDiamondAddress, position2.ToHexBigInteger());
            // need to unpack the slot manually (there's no *nice* way to do this AFAIK)
            // see discussion: https://github.com/ethers-io/ethers.js/discussions/2373
            // the slot line is a uint256 => 64 hex characters (+ 2 for the 0x prefix)
            // the structure:            
            //
            //  struct SvgLayer {
            //    address svgLayersContract;
            //    uint16 offset;
            //    uint16 size;
            //  }
            //
            // is packed into a single slot:
            // - address = 20 bytes = 40 hex characters
            // - uint16 = 2 bytes = 4 hex characters            
            // so unpack it from right to left:
            var address = "0x" + resultBytes.Substring(66 - 40, 40);
            var offset = Convert.ToUInt16(resultBytes.Substring(66 - 44, 4), 16);
            var size = Convert.ToUInt16(resultBytes.Substring(66 - 48, 4), 16);

            // grab the bytecode from the contract
            var code = await Web3.Eth.GetCode.SendRequestAsync(address);
            code = code.Substring(2 + offset * 2, size * 2);

            // convert the bytes to a SVG string
            var svg = "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 64 64\">" 
                + Encoding.UTF8.GetString(code.HexToByteArray())
                + "</svg>";

            return svg;
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

            string[] svgs = new string[types.Length];
            for (int i = 0; i < types.Length; ++i)
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
                //wearable.dimensions = new int[] { itemData.Dimensions.X, itemData.Dimensions.Y, itemData.Dimensions.Width, itemData.Dimensions.Height }; //TODO: make this nicer
                wearable.rarity = (GotchiWearableRarity)itemData.RarityScoreModifier;
                wearable.minLevel = itemData.MinLevel;
                //wearable.svgId = itemData.SvgId; //only used for grabbing svgs
                wearable.category = itemData.Category;
                wearable.svgs = await GetWearableSvgs(wearableId, itemData.SvgId);
                //wearable.offsets = await GetWearableOffsets(wearableId);

                bool isBodyWearable = itemData.SlotPositions[0];
                if (isBodyWearable)
                {
                    //try to get sleeves id
                    var sleevesSvgId = await GetSleevesSvgId(wearableId);
                    Debug.Log($"got sleeves for id: {wearableId} = {sleevesSvgId}");
                    if (sleevesSvgId != 0)
                    {
                        //Debug.Log("adding sleeves...");
                        wearable.sleeves = await GetSleevesSvgs(sleevesSvgId);
                    }
                }

                var dimensionsSource = new Dimensions[]{
                    itemData.Dimensions,
                    await GetSideViewDimensions(wearableId, "left"),
                    await GetSideViewDimensions(wearableId, "right"),
                    await GetSideViewDimensions(wearableId, "back")
                };
                
                wearable.dimensions = new GotchiWearableDimensions[4];
                for (int i = 0; i < 4; ++i)
                {
                    var dim = dimensionsSource[i];
                    wearable.dimensions[i] = new GotchiWearableDimensions()
                    {
                        X = dim.X,
                        Y = dim.Y,
                        Width = dim.Width,
                        Height = dim.Height
                    };
                }

                //var exceptions = await GetWearableExceptions(353, GotchiFacing.BACK, GotchiEquipmentSlot.PET);
                //Debug.Log($"exceptions: {exceptions}");

                //var layer = await GetSvg("aavegotchi", 0);

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