using System;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace PortalDefender.AavegotchiKit
{
    [Serializable]
    public class BaseParts
    {
        public string[] body;
        public string[] hands;
        public string[] mouth_neutral;
        public string[] mouth_happy;
        public string[] eyes_mad;
        public string[] eyes_happy;
        public string[] eyes_sleepy;
        public string[] shadow;
    }

    [CreateAssetMenu(fileName = "BasePartsDB", menuName = "Aavegotchi/BasePartsDB")]
    public class BasePartsDB : ScriptableObject
    {
        [SerializeField]
        TextAsset source;

        [SerializeField]
        BaseParts baseParts;
        
        public Sprite GetBodySprite(Collateral collateral, GotchiFacing facing)
        {
            return SvgLoader.GetSvgLayerSprite($"body-{collateral.collateralType}-{facing}",
                baseParts.body[(int)facing],
                new SvgLoader.Options
                {
                    primary = collateral.PrimaryColor,
                    secondary = collateral.SecondaryColor,
                    cheeks = collateral.CheekColor,
                    //eyes
                    hideMouth = true,
                    hideShadow = true,
                });
        }

        public Sprite GetHandsSprite(Collateral collateral, GotchiHandPose pose, GotchiFacing facing)
        {
            return SvgLoader.GetSvgLayerSprite($"hands-{collateral.collateralType}-{pose}-{facing}",
                baseParts.hands[(int)facing],
                new SvgLoader.Options
                {
                    primary = collateral.PrimaryColor,
                    secondary = collateral.SecondaryColor,
                    hideSleevesUp = pose != GotchiHandPose.UP,
                    hideHandsUp = pose != GotchiHandPose.UP,
                    hideHandsDownClosed = pose != GotchiHandPose.DOWN_CLOSED,
                    hideHandsDownOpen = pose != GotchiHandPose.DOWN_OPEN
                });
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

            return SvgLoader.GetSvgLayerSprite($"mouth-{expression}-{collateral.collateralType}",
                mouth[0],
                new SvgLoader.Options
                {
                    primary = collateral.PrimaryColor,
                    secondary = collateral.SecondaryColor
                });
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

            return SvgLoader.GetSvgLayerSprite($"specialEyes-{expression}-{collateral.collateralType}",
                special[0],
                new SvgLoader.Options
                {
                    primary = collateral.PrimaryColor,
                    secondary = collateral.SecondaryColor
                }) ;
        }

        public Sprite GetShadowSprite(GotchiFacing facing)
        {
            int shadowIndex = 0;
            if (facing == GotchiFacing.LEFT || facing == GotchiFacing.RIGHT)
                shadowIndex = 1;


            return SvgLoader.GetSvgLayerSprite($"shadow-{shadowIndex}",
                baseParts.shadow[shadowIndex],
                new SvgLoader.Options
                { 
                    //customPivot = new Vector2(0.5f, 0.08f)
                });
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