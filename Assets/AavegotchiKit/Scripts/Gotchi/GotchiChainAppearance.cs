using Aavegotchi.AavegotchiDiamond.ContractDefinition;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using PortalDefender.AavegotchiKit.Blockchain;
using System.Collections.Generic;

namespace PortalDefender.AavegotchiKit
{
    /// <summary>
    /// Uses the provided GotchiData (from graph, blockchain, local) to request the SVGs from the blockchain
    /// using the PreviewSideAavegotchi function.  The SVGs are then loaded into sprites.
    /// </summary>
    /// 
    /// <remarks>
    /// For most usecases, this is the best way to get the appearance of a gotchi.<br/>
    /// Things to note:<br/>
    /// - It requires a Web3Provider in the Scene<br/>
    /// - It allows for customization of wearables<br/>
    /// - It doesn't work in the editor (placeholder sprites are used instead)<br/>
    /// </remarks>
    public class GotchiChainAppearance : MonoBehaviour, IGotchiAppearance
    {
        Gotchi gotchi;

        [SerializeField]
        SpriteRenderer body;

        [SerializeField]
        SpriteRenderer shadow;

        [SerializeField]
        Sprite[] shadowSprites;

        [SerializeField]
        GotchiSvgStyling styling;

        ISvgProvider svgProvider;

        private void Awake()
        {            
            // hide appearance until it's done loading
            body.sprite = null;
            shadow.sprite = null;

            //look for an svg provider on this gameobject
            svgProvider = GetComponent<ISvgProvider>();
            if (svgProvider == null)
            {
                //if there isn't one, add the default one:
                svgProvider = gameObject.AddComponent<GotchiChainSvgProvider>();
            }

        }

        Sprite[] svgSprites = new Sprite[4];

        public void Init(Gotchi gotchi)
        {
            this.gotchi = gotchi;

            this.gotchi.State.PropertyChanged -= State_PropertyChanged;

            try
            {                
                svgProvider.GetSvg(gotchi.Data).ContinueWith((gotchiSvgData) =>
                {
                    svgSprites[(int)GotchiFacing.FRONT] = SvgLoader.CreateSvgSprite(styling.CustomizeSVG(gotchiSvgData.front), Vector2.zero);
                    svgSprites[(int)GotchiFacing.LEFT] = SvgLoader.CreateSvgSprite(styling.CustomizeSVG(gotchiSvgData.left), Vector2.zero);
                    svgSprites[(int)GotchiFacing.RIGHT] = SvgLoader.CreateSvgSprite(styling.CustomizeSVG(gotchiSvgData.right), Vector2.zero);
                    svgSprites[(int)GotchiFacing.BACK] = SvgLoader.CreateSvgSprite(styling.CustomizeSVG(gotchiSvgData.back), Vector2.zero);

                    Refresh();
                });

                this.gotchi.State.PropertyChanged += State_PropertyChanged;
            }
            catch(System.Exception e)
            {
                Debug.LogError("Error Initializing Appearance: " + e.Message);
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