using System.Linq;
using UnityEngine;

namespace PortalDefender.AavegotchiKit
{
    public partial class AavegotchiData : MonoBehaviour
    {
        static AavegotchiData instance_;
        public static AavegotchiData Instance => instance_;

        [SerializeField]
        Sprite[] body;

        public Sprite GetBodySprite(GotchiFacing facing)
        {
            return body[(int)facing];
        }

        [SerializeField]
        Sprite[] handsUp;
        [SerializeField]
        Sprite[] handsDownClosed;
        [SerializeField]
        Sprite[] handsDownOpen;

        public Sprite GetHandsSprite(GotchiHandPose pose, GotchiFacing facing)
        {
            switch (pose)
            {
                case GotchiHandPose.DOWN_CLOSED: return handsDownClosed[(int)facing];
                case GotchiHandPose.DOWN_OPEN: return handsDownOpen[(int)facing];
                case GotchiHandPose.UP: return handsUp[(int)facing];
                default: return null;
            }
        }

        [SerializeField]
        Sprite[] mouths;

        //mouths are only visible from the front
        public Sprite GetMouthSprite(GotchiMouthExpression expression = GotchiMouthExpression.HAPPY)
        {
            return mouths[(int)expression];
        }


        public static Color MythicalLow     = new Color32(0xFF, 0x00, 0xFF, 0xFF);
        public static Color RareLow         = new Color32(0x00, 0x64, 0xFF, 0xFF);
        public static Color UncommonLow     = new Color32(0x5D, 0x24, 0xBF, 0xFF);
        public static Color UncommonHigh    = new Color32(0x36, 0x81, 0x8E, 0xFF);
        public static Color RareHigh        = new Color32(0xEA, 0x8C, 0x27, 0xFF);
        public static Color MythicalHigh    = new Color32(0x51, 0xFF, 0xA8, 0xFF);

        public Color GetEyeColor(int eyeColorTrait, Collateral collateral)
        {
            if (eyeColorTrait < 2) return MythicalLow;
            if (eyeColorTrait < 10) return RareLow;
            if (eyeColorTrait < 25) return UncommonLow;
            if (eyeColorTrait < 75) return collateral.PrimaryColor;
            if (eyeColorTrait < 90) return UncommonHigh;
            if (eyeColorTrait < 98) return RareHigh;
            return MythicalHigh;
        }

        int[] eyeShapeTraitTraitRange = { 0, 1, 2, 5, 7, 10, 15, 20, 25, 42, 58, 75, 80, 85, 90, 93, 95, 98 };

        public Sprite GetEyeSprite(int eyeShapeTrait, Collateral collateral, GotchiFacing facing)
        {
            if (facing == GotchiFacing.BACK)
                return null;

            Sprite eyeSprite = null;

            if (eyeShapeTrait < 0)
            {
                eyeSprite = eyeShapesDB.eyeShapes[0].sprites[(int)facing];
            }
            else if (eyeShapeTrait > 97)
            {
                eyeSprite = collateral.eyeSprites[(int)facing];
            }
            else
            {
                for (int i=0; i < eyeShapeTraitTraitRange.Length - 1; i++)
                {
                    if (eyeShapeTrait >= eyeShapeTraitTraitRange[i] 
                        && eyeShapeTrait < eyeShapeTraitTraitRange[i + 1])
                    {
                        eyeSprite = eyeShapesDB.eyeShapes[i].sprites[(int)facing];
                        break;
                    }
                }
            }
            return eyeSprite;
        }

        [SerializeField]
        Sprite[] specialEyes;

        public Sprite GetSpecialEyesSprite(int eyeShapeTrait, Collateral collateral, GotchiFacing facing, GotchiEyeExpression expression = GotchiEyeExpression.NONE)
        {
            if (expression == GotchiEyeExpression.NONE || facing != GotchiFacing.FRONT)
                return GetEyeSprite(eyeShapeTrait, collateral, facing);
            return specialEyes[(int)expression];
        }

        [SerializeField]
        Sprite[] shadows;

        public Sprite GetShadowSprite(GotchiFacing facing)
        {
            switch (facing)
            {
                case GotchiFacing.FRONT:
                case GotchiFacing.BACK:
                default:
                    return shadows[0];
                case GotchiFacing.LEFT:
                case GotchiFacing.RIGHT:
                    return shadows[1];
            }
        }


        public WearableDB wearableDB;

        public Wearable GetWearable(int wearableID)
        {
            return wearableDB.wearables.Where(x => x.id == wearableID).FirstOrDefault();
        }


        public CollateralDB collateralDB;

        public Collateral GetCollateral(string collateral)
        {
            return collateralDB.collaterals.Where(x => x.collateralType.Equals(collateral, 
                System.StringComparison.CurrentCultureIgnoreCase )).FirstOrDefault();
        }


        public EyeShapesDB eyeShapesDB;


        public WearableSetDB wearableSetDB;


        private void Awake()
        {
            instance_ = this;
        }
    }
}
