using Aavegotchi.AavegotchiDiamond.ContractDefinition;
using Aavegotchi.AavegotchiDiamond.Service;
using Cysharp.Threading.Tasks;
using Nethereum.Unity.Rpc;
using Nethereum.Web3;
using System;
using System.Linq;
using System.Text;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;

namespace PortalDefender.AavegotchiKit.Examples
{
    public class TestNethereum : MonoBehaviour
    {
        [SerializeField]
        SVGImage[] images;

        [SerializeField]
        TMP_InputField gotchiId;

        [SerializeField]
        TMP_Dropdown collateralDropdown;

        [SerializeField]
        TMP_InputField[] traits;

        [SerializeField]
        TMP_InputField[] equippedWearables;

        [SerializeField]
        TMP_InputField itemId;

        private Web3 web3_ = null;

        Web3 web3 => web3_ ??= new Web3(new UnityWebRequestRpcTaskClient(new Uri(Constants.DefaultPolygonRPC)));

        private AavegotchiDiamondService svc_ = null;

        AavegotchiDiamondService svc => svc_ ??= new AavegotchiDiamondService(web3, Constants.AavegotchiDiamondAddress);

        GotchiSvgStyling styling_;

        private void Start()
        {
            gotchiId.text = "23202"; //Portal Defender

            collateralDropdown.ClearOptions();
            collateralDropdown.AddOptions(GotchiDataProvider.Instance.collateralDB.collaterals
                .Select(c => new TMP_Dropdown.OptionData() { text = c.name }).ToList());

            foreach (var trait in traits)
            {
                trait.text = "50";
            }

            styling_ = new GotchiSvgStyling() { RemoveShadow = true, RemoveBackground = true };

        }

        void ClearImages()
        {
            for (int i = 0; i < images.Length; i++)
            {
                images[i].sprite = null;
            }
        }

        public void GetAavegotchiSvg()
        {
            Debug.Log($"GetAavegotchiSvg({gotchiId.text})");
            GetAavegotchiSvgAsync().Forget();
        }

        async UniTaskVoid GetAavegotchiSvgAsync()
        {
            var tokenId = int.Parse(gotchiId.text);

            var getAavegotchiSvg = new GetAavegotchiSvgFunction { TokenId = tokenId };

            try
            {
                ClearImages();
                var svg = await svc.GetAavegotchiSvgQueryAsync(getAavegotchiSvg);
                //Debug.Log("Got SVG: " + svg);                 
                svg = styling_.CustomizeSVG(svg);
                var sprite = SvgLoader.CreateSvgSprite(svg, Vector2.zero);
                images[0].sprite = sprite;
            }
            catch (Exception e)
            {
                Debug.Log("Error: " + e.Message);
            }
        }

        public void PreviewAavegotchi()
        {
            Debug.Log($"PreviewAavegotchi()");
            PreviewAavegotchiAsync().Forget();
        }

        async UniTaskVoid PreviewAavegotchiAsync()
        {
            var previewAavegotchi = new PreviewAavegotchiFunction
            {
                HauntId = 1,
                CollateralType = GotchiDataProvider.Instance.collateralDB.collaterals[collateralDropdown.value].collateralType,
                NumericTraits = traits.Select(x => short.Parse(x.text)).ToList(),
                EquippedWearables = equippedWearables.Select(x => ushort.Parse(x.text)).Concat(new ushort[8]).ToList(), //pad it out to 16
            };

            try
            {
                ClearImages();
                var svg = await svc.PreviewAavegotchiQueryAsync(previewAavegotchi);
                //Debug.Log("Got SVG: " + svg);
                svg = styling_.CustomizeSVG(svg);
                var sprite = SvgLoader.CreateSvgSprite(svg, Vector2.zero);
                images[0].sprite = sprite;

            }
            catch (Exception e)
            {
                Debug.Log("Error: " + e.Message);
            }
        }

        //TODO: this is still pretty verbose, can we make it simpler?
        //Also, we should probably cache the SVGs so we don't have to keep fetching them
        //It's not my personal preference, but using the SVGs this way is do-able. 
        //We need a nice way to set the styles
        //Another thing to note is that Unity's SVG lib doesn't support all SVG features including animations

        public void GetAavegotchiSideSvgs()
        {
            Debug.Log($"GetAavegotchiSideSvgs({gotchiId.text})");
            GetAavegotchiSideSvgsAsync().Forget();
        }

        async UniTaskVoid GetAavegotchiSideSvgsAsync()
        {
            var tokenId = int.Parse(gotchiId.text);

            var getAavegotchiSideSvgs = new GetAavegotchiSideSvgsFunction { TokenId = tokenId };

            try
            {
                ClearImages();
                var svgs = await svc.GetAavegotchiSideSvgsQueryAsync(getAavegotchiSideSvgs);
                for (int i = 0; i < svgs.Count; i++)
                {
                    var svg = svgs[i];
                    svg = styling_.CustomizeSVG(svg);
                    var sprite = SvgLoader.CreateSvgSprite(svg, Vector2.zero);
                    images[i].sprite = sprite;
                }

            }
            catch (Exception e)
            {
                Debug.Log("Error: " + e.Message);
            }
        }

        public void PreviewSideAavegotchi()
        {
            Debug.Log($"PreviewAavegotchi()");
            PreviewSideAavegotchiAsync().Forget();
        }

        async UniTaskVoid PreviewSideAavegotchiAsync()
        {
            var previewAavegotchi = new PreviewSideAavegotchiFunction
            {
                HauntId = 1,
                CollateralType = GotchiDataProvider.Instance.collateralDB.collaterals[collateralDropdown.value].collateralType,
                NumericTraits = traits.Select(x => short.Parse(x.text)).ToList(),
                EquippedWearables = equippedWearables.Select(x => ushort.Parse(x.text)).Concat(new ushort[8]).ToList(), //pad it out to 16
            };

            try
            {
                ClearImages();
                var svgs = await svc.PreviewSideAavegotchiQueryAsync(previewAavegotchi);
                for (int i = 0; i < svgs.Count; i++)
                {
                    var svg = svgs[i];
                    svg = styling_.CustomizeSVG(svg);
                    var sprite = SvgLoader.CreateSvgSprite(svg, Vector2.zero);
                    images[i].sprite = sprite;
                }

            }
            catch (Exception e)
            {
                Debug.Log("Error: " + e.Message);
            }
        }

        public void GetSvg()
        {
            Debug.Log($"GetSvg()");
            GetSvgAsync().Forget();
        }

        async UniTaskVoid GetSvgAsync()
        {
            var getSvg = new GetSvgFunction { SvgType = Encoding.UTF8.GetBytes("portal-closed"), ItemId = 1 };

            try
            {
                ClearImages();
                var svg = await svc.GetSvgQueryAsync(getSvg);
                Debug.Log("Got SVG: " + svg);
                var sprite = SvgLoader.CreateSvgSprite(svg, Vector2.zero);
                images[0].sprite = sprite;
            }
            catch (Exception e)
            {
                Debug.Log("Error: " + e.Message);
            }
        }


        public void GetItemSvg()
        {
            Debug.Log($"GetItemSvg()");
            GetItemSvgAsync().Forget();
        }

        async UniTaskVoid GetItemSvgAsync()
        {
            var getItemSvg = new GetItemSvgFunction { ItemId = int.Parse(itemId.text) };

            try
            {
                ClearImages();
                var svg = await svc.GetItemSvgQueryAsync(getItemSvg);
                Debug.Log("Got SVG: " + svg);
                var sprite = SvgLoader.CreateSvgSprite(svg, Vector2.zero);
                images[0].sprite = sprite;
            }
            catch (Exception e)
            {
                Debug.Log("Error: " + e.Message);
            }
        }

        public void GetItemSvgs()
        {
            Debug.Log($"GetItemSideSvgs()");
            GetItemSvgsAsync().Forget();
        }

        async UniTaskVoid GetItemSvgsAsync()
        {
            var getItemSvgs = new GetItemSvgsFunction { ItemId = int.Parse(itemId.text) };

            try
            {
                ClearImages();
                var svgs = await svc.GetItemSvgsQueryAsync(getItemSvgs);
                for (int i = 0; i < svgs.Count; i++)
                {
                    var svg = svgs[i];
                    Debug.Log("Got SVG: " + svg);
                    var sprite = SvgLoader.CreateSvgSprite(svg, Vector2.zero);
                    images[i].sprite = sprite;
                }
            }
            catch (Exception e)
            {
                Debug.Log("Error: " + e.Message);
            }
        }

    }
}
