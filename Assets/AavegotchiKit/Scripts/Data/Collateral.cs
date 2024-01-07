using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PortalDefender.AavegotchiKit
{
    [Serializable]
    public class Collateral
    {
        public string name;
        public string collateralType;
        public short[] modifiers;
        public string primaryColor;
        public string secondaryColor;
        public string cheekColor;
        public int haunt;

        public int svgId;
        public string[] svgs;

        public int eyeShapeSvgId;
        public string[] eyeShapeSvgs;

        public Sprite GetCollateralSprite(GotchiFacing facing)
        {
            if (facing == GotchiFacing.BACK)
                return null;

            var sprite = SvgLoader.GetSvgLayerSprite($"collateral-{collateralType}-{facing}",
                svgs[(int)facing],
                new SvgLoader.Options
                {
                    size = new Vector2(64, 64),
                    primary = PrimaryColor,
                    secondary = SecondaryColor,
                    cheeks = CheekColor
                });

            if (sprite == null)
                Debug.LogError($"Failed to load collateral sprite for collateral-{collateralType}-{facing}!");

            return sprite;
        }

        public Sprite GetCollateralEyeSprite(Color eyeColor, GotchiFacing facing)
        {
            if (facing == GotchiFacing.BACK)
                return null;

            var sprite = SvgLoader.GetSvgLayerSprite($"eye-{collateralType}-{facing}",
                eyeShapeSvgs[(int)facing],
                new SvgLoader.Options
                {
                    size = new Vector2(64, 64),
                    primary = PrimaryColor,
                    secondary = SecondaryColor,
                    cheeks = CheekColor,
                    eyes = eyeColor
                });

            if (sprite == null)
                Debug.LogError($"Failed to load eye sprite for eye-{collateralType}-{facing}!");

            return sprite;
        }

        static Color parseColor(string color)
        {
            ColorUtility.TryParseHtmlString(color.Replace("0x", "#"), out var c);
            return c;
        }

        public Color PrimaryColor => parseColor(primaryColor);
        public Color SecondaryColor => parseColor(secondaryColor);

        public Color CheekColor => parseColor(cheekColor);


        //[Sirenix.OdinInspector.Button]
        private async void RefreshData()
        {
            if (!string.IsNullOrEmpty(collateralType) && haunt > 0)
            {
                CollateralFetcher fetcher = new CollateralFetcher();
                var c = await fetcher.GetCollateral(collateralType, haunt);
                if (c != null)
                {
                    modifiers = c.modifiers;
                    primaryColor = c.primaryColor;
                    secondaryColor = c.secondaryColor;
                    cheekColor = c.cheekColor;
                    svgId = c.svgId;
                    svgs = c.svgs;
                    eyeShapeSvgId = c.eyeShapeSvgId;
                    eyeShapeSvgs = c.eyeShapeSvgs;
                }
            }
        }

    }
}
