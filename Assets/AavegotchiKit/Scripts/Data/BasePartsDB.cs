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

        [SerializeField]
        Sprite[] body;

        public Sprite GetBodySprite(Collateral collateral, GotchiFacing facing)
        {
#if USE_VECTOR_GFX
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
#else
            return body[(int)facing];
#endif
        }

        [SerializeField]
        Sprite[] handsUp;

        [SerializeField]
        Sprite[] handsDownClosed;

        [SerializeField]
        Sprite[] handsDownOpen;

        public Sprite GetHandsSprite(Collateral collateral, GotchiHandPose pose, GotchiFacing facing)
        {
#if USE_VECTOR_GFX
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

#else
            switch (pose)
            {
                case GotchiHandPose.DOWN_CLOSED: return handsDownClosed[(int)facing];
                case GotchiHandPose.DOWN_OPEN: return handsDownOpen[(int)facing];
                case GotchiHandPose.UP: return handsUp[(int)facing];
                default: return null;
            }
#endif
        }

        [SerializeField]
        Sprite[] mouths;

        //mouths are only visible from the front
        public Sprite GetMouthSprite(Collateral collateral, GotchiMouthExpression expression = GotchiMouthExpression.HAPPY)
        {
#if USE_VECTOR_GFX
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
#else
            return mouths[(int)expression];
#endif
        }

        [SerializeField]
        Sprite[] specialEyes;

        public Sprite GetSpecialEyes(Collateral collateral, GotchiEyeExpression expression)
        {
#if USE_VECTOR_GFX

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
#else
            return specialEyes[(int)expression];
#endif
        }


        [SerializeField]
        Sprite[] shadows;

        public Sprite GetShadowSprite(GotchiFacing facing)
        {
            int shadowIndex = 0;
            if (facing == GotchiFacing.LEFT || facing == GotchiFacing.RIGHT)
                shadowIndex = 1;

#if USE_VECTOR_GFX
            return SvgLoader.GetSvgLayerSprite($"shadow-{shadowIndex}",
                baseParts.shadow[shadowIndex],
                new SvgLoader.Options
                { 
                    //customPivot = new Vector2(0.5f, 0.08f)
                });
#else
            return shadows[shadowIndex];       
#endif
        }

#if UNITY_EDITOR
        Sprite GetSpriteAsset(string spritename)
        {
            try
            {
                var guid = AssetDatabase.FindAssets(spritename).First();
                return AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(guid));
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return null;
            }
        }
#endif

        [ContextMenu("Import Data")]
        void Import()
        {
#if UNITY_EDITOR
            Debug.Log("Importing aavegotchi base parts...");

            var tmp = JsonUtility.FromJson<BaseParts>(source.text);
            baseParts = tmp;

            body = new Sprite[4];
            body[(int)GotchiFacing.FRONT] = GetSpriteAsset("gotchi_base");
            body[(int)GotchiFacing.LEFT] = GetSpriteAsset("gotchi_base_left");
            body[(int)GotchiFacing.RIGHT] = GetSpriteAsset("gotchi_base_right");
            body[(int)GotchiFacing.BACK] = GetSpriteAsset("gotchi_base_back");

            handsUp = new Sprite[4];
            handsUp[(int)GotchiFacing.FRONT] = GetSpriteAsset("gotchi_hands_up");
            handsUp[(int)GotchiFacing.LEFT] = GetSpriteAsset("gotchi_hands_up_left");
            handsUp[(int)GotchiFacing.RIGHT] = GetSpriteAsset("gotchi_hands_up_right");
            handsUp[(int)GotchiFacing.BACK] = GetSpriteAsset("gotchi_hands_up_back");

            handsDownOpen = new Sprite[4];
            handsDownOpen[(int)GotchiFacing.FRONT] = GetSpriteAsset("gotchi_hands_down_open");
            handsDownOpen[(int)GotchiFacing.LEFT] = GetSpriteAsset("gotchi_hands_down_open_left");
            handsDownOpen[(int)GotchiFacing.RIGHT] = GetSpriteAsset("gotchi_hands_down_open_right");
            handsDownOpen[(int)GotchiFacing.BACK] = GetSpriteAsset("gotchi_hands_down_open_back");

            handsDownClosed = new Sprite[4];
            handsDownClosed[(int)GotchiFacing.FRONT] = GetSpriteAsset("gotchi_hands_down_closed");
            handsDownClosed[(int)GotchiFacing.LEFT] = GetSpriteAsset("gotchi_hands_down_closed_left");
            handsDownClosed[(int)GotchiFacing.RIGHT] = GetSpriteAsset("gotchi_hands_down_closed_right");
            handsDownClosed[(int)GotchiFacing.BACK] = GetSpriteAsset("gotchi_hands_down_closed_back");

            mouths = new Sprite[2];
            mouths[(int)GotchiMouthExpression.NEUTRAL] = GetSpriteAsset("gotchi_mouth_happy");
            mouths[(int)GotchiMouthExpression.HAPPY] = GetSpriteAsset("gotchi_mouth_neutral");

            specialEyes = new Sprite[4];
            specialEyes[(int)GotchiEyeExpression.HAPPY] = GetSpriteAsset("gotchi_eyes_happy");
            specialEyes[(int)GotchiEyeExpression.MAD] = GetSpriteAsset("gotchi_eyes_mad");
            specialEyes[(int)GotchiEyeExpression.SLEEPING] = GetSpriteAsset("gotchi_eyes_sleepy");

            shadows = new Sprite[2];
            shadows[0] = GetSpriteAsset("gotchi_shadow");
            shadows[1] = GetSpriteAsset("gotchi_shadow_side");

            AssetDatabase.SaveAssetIfDirty(this);
#endif
        }
    }
}