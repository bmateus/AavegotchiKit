using Aavegotchi.AavegotchiDiamond.ContractDefinition;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using PortalDefender.AavegotchiKit.Blockchain;

namespace PortalDefender.AavegotchiKit
{
    public class GotchiAppearanceChain : MonoBehaviour, IGotchiAppearance
    {
        Gotchi gotchi;

        [SerializeField]
        SpriteRenderer body;

        [SerializeField]
        SpriteRenderer shadow;

        [SerializeField]
        Sprite[] shadowSprites;

        private void Awake()
        {            
            // hide appearance until it's done loading
            body.sprite = null;
            shadow.sprite = null;
        }

        Sprite[] svgSprites = new Sprite[4];

        public void Init(Gotchi gotchi)
        {
            this.gotchi = gotchi;

            this.gotchi.State.PropertyChanged -= State_PropertyChanged;

            try
            {
                //use PreviewSideAavegotchi to get the svgs so we can customize the appearance if we want

                //fetch the gotchi appearance from on chain
                var previewAavegotchi = new PreviewSideAavegotchiFunction
                {
                    HauntId = gotchi.Data.hauntId,
                    CollateralType = gotchi.Data.collateral,
                    NumericTraits = gotchi.Data.numericTraits.ToList(),
                    EquippedWearables = gotchi.Data.equippedWearables
                        .Concat(new ushort[8]).ToList(), //pad it out to 16
                };

                Web3Provider.Instance.GotchiDiamondService.PreviewSideAavegotchiQueryAsync(previewAavegotchi)
                    .AsUniTask()
                    .ContinueWith((svgs) => {
                        
                        var styling = new GotchiSvgStyling() { RemoveShadow = true, RemoveBackground = true };

                        for (int i = 0; i < svgs.Count; i++)
                        {
                            var svg = svgs[i];
                            Debug.Log("Got SVG: " + svg);
                            svg = styling.CustomizeSVG(svg);
                            var svgSprite = SvgLoader.CreateSvgSprite(svg, Vector2.zero);
                            svgSprite.name = "Gotchi-" + (GotchiFacing)i;
                            svgSprites[i] = svgSprite;
                        }

                        Refresh();
                    });

                this.gotchi.State.PropertyChanged += State_PropertyChanged;
            }
            catch(System.Exception e)
            {
                Debug.Log("Error Initializing Appearance: " + e.Message);
            }            
        }

        private void State_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Refresh();
        }

        public void Refresh()
        {
            body.sprite = svgSprites[(int)gotchi.State.Facing];
            shadow.sprite = shadowSprites[(int)gotchi.State.Facing];
        }
    }
}