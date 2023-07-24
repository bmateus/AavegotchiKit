using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace PortalDefender.AavegotchiKit
{
    [CreateAssetMenu(fileName = "BasePartsDB", menuName = "Aavegotchi/BasePartsDB")]
    public class BasePartsDB : ScriptableObject
    {
        [SerializeField]
        TextAsset source;

        [SerializeField]
        BaseParts baseParts;
        
        public Sprite GetBodySprite(Collateral collateral, GotchiFacing facing)
        {
            var sprite = SvgLoader.GetSvgLayerSprite($"body-{collateral.collateralType}-{facing}",
                baseParts.body[(int)facing],
                new SvgLoader.Options
                {
                    size = new Vector2(64, 64),
                    primary = collateral.PrimaryColor,
                    secondary = collateral.SecondaryColor,
                    cheeks = collateral.CheekColor,
                    //eyes
                    hideMouth = true,
                    hideShadow = true,
                });

            if (sprite == null)
                Debug.LogError($"Failed to load body sprite for body-{collateral.collateralType}-{facing}!");

            return sprite;
        }

        public Sprite GetHandsSprite(Collateral collateral, GotchiHandPose handPose, GotchiFacing facing)
        {
            if (handPose == GotchiHandPose.DOWN_CLOSED && facing == GotchiFacing.BACK)
                return null;

            var sprite = SvgLoader.GetSvgLayerSprite($"hands-{collateral.collateralType}-{handPose}-{facing}",
                baseParts.hands[(int)facing],
                new SvgLoader.Options
                {
                    size = new Vector2(64, 64),
                    primary = collateral.PrimaryColor,
                    secondary = collateral.SecondaryColor,
                    handPose = handPose,
                });

            if (sprite == null)
                Debug.LogError($"Failed to load hands sprite for hands-{collateral.collateralType}-{handPose}-{facing}!");

            return sprite;
        }

        //mouths are only visible from the front
        public Sprite GetMouthSprite(Collateral collateral, GotchiMouthExpression expression = GotchiMouthExpression.HAPPY)
        {
            string[] mouth = null;
            switch (expression)
            {
                case GotchiMouthExpression.HAPPY:
                    mouth = baseParts.mouth_happy;
                    break;
                case GotchiMouthExpression.NEUTRAL:
                    mouth = baseParts.mouth_neutral;
                    break;
            }

            var sprite = SvgLoader.GetSvgLayerSprite($"mouth-{expression}-{collateral.collateralType}",
                mouth[0],
                new SvgLoader.Options
                {
                    size = new Vector2(64, 64),
                    primary = collateral.PrimaryColor,
                    secondary = collateral.SecondaryColor
                });

            if (sprite == null)
                Debug.LogError($"Failed to load mouth sprite for mouth-{expression}-{collateral.collateralType}!");

            return sprite;
        }

        public Sprite GetSpecialEyes(Collateral collateral, GotchiEyeExpression expression)
        {
            string[] special = null;
            switch(expression)
            {
                case GotchiEyeExpression.NONE:
                    return null;
                case GotchiEyeExpression.HAPPY:
                    special = baseParts.eyes_happy;
                    break;
                case GotchiEyeExpression.MAD:
                    special = baseParts.eyes_mad;
                    break;
                case GotchiEyeExpression.SLEEPING:
                    special = baseParts.eyes_sleepy;
                    break;
            }

            var sprite = SvgLoader.GetSvgLayerSprite($"specialEyes-{expression}-{collateral.collateralType}",
                special[0],
                new SvgLoader.Options
                {
                    size = new Vector2(64, 64),
                    primary = collateral.PrimaryColor,
                    secondary = collateral.SecondaryColor
                }) ;

            if (sprite == null)
                Debug.LogError($"Failed to load special eyes sprite for specialEyes-{expression}-{collateral.collateralType}!");

            return sprite;
        }

        public Sprite GetShadowSprite(GotchiFacing facing)
        {
            int shadowIndex = 0;
            if (facing == GotchiFacing.LEFT || facing == GotchiFacing.RIGHT)
                shadowIndex = 1;


            var sprite = SvgLoader.GetSvgLayerSprite($"shadow-{shadowIndex}",
                baseParts.shadow[shadowIndex],
                new SvgLoader.Options
                {
                    size = new Vector2(64, 64),
                    //customPivot = new Vector2(0.5f, 0.08f)
                });

            if (sprite == null)
                Debug.LogError($"Failed to load shadow sprite for shadow-{shadowIndex}!");

            return sprite;
        }

        [ContextMenu("Import Data")]
        void Import()
        {
#if UNITY_EDITOR
            Debug.Log("Importing aavegotchi base parts...");

            var tmp = JsonUtility.FromJson<BaseParts>(source.text);
            baseParts = tmp;

            AssetDatabase.SaveAssetIfDirty(this);
#endif
        }
    }
}